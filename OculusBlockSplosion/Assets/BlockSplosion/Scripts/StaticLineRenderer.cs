using UnityEngine;
using System.Collections.Generic;

public class StaticLineRenderer : MonoBehaviour
{

	[SerializeField]
	private Color		_startColor		= Color.white;

	[SerializeField]
	private Color		_endColor		= new Color(1, 1, 1, 0);

	[SerializeField]
	private float		_startWidth		= 1;

	[SerializeField]
	private float		_endWidth;

	[SerializeField]
	private Material	_material;

	[SerializeField]
	private bool		_useWorldCoordinates	= true;

	[SerializeField]
	private bool		_transformRightAndUp	= true;

	private MeshFilter	_filter;

	private Mesh		_mesh;

	private Vector3 _right	= Vector3.right;

	private Vector3	_up		= Vector3.up;

	public Vector3 Right
	{
		get { return _right; }
		set
		{
			if (_right != value)
			{
				_right	= value;
				Dirty	= true;
			}
		}
	}

	public Vector3 Up
	{
		get { return _up; }
		set
		{
			if (_up != value)
			{
				_up		= value;
				Dirty	= true;
			}
		}
	}

	public bool Dirty { get; private set; }

	private List<Vector3> _positions = new List<Vector3>();

	public int PositionCount
	{
		get
		{
			return _positions.Count;
		}
		set
		{
			if (_positions.Count != value)
			{
				if (_positions.Count < value)
				{
					for (int i = _positions.Count; i < value; i++)
						_positions.Add(Vector3.zero);
				}
				else
				{
					if (value > 0)
						_positions.RemoveRange(value, _positions.Count - value);
					else
						_positions.Clear();
				}
				Dirty = true;
			}
		}
	}

	private int[]		_indices;
	private Vector3[]	_vertices;
	private Vector3[]	_normals;
	private Vector2[]	_uvs;
	private Color[]		_colors;

	public Vector3 this[int index]
	{
		get
		{
			if (index >= 0 && index < _positions.Count)
				return _positions[index];
			else
				Debug.LogError(string.Format("Index {0} out of range ({1})!", index, _positions.Count));

			return Vector3.zero;
		}
		set
		{
			if (index >= 0 && index < _positions.Count)
			{
				_positions[index] = value;
				Dirty = true;
			}
			else
				Debug.LogError(string.Format("Index {0} out of range ({1})!", index, _positions.Count));
		}
	}

	public void Rebuild()
	{
		if (!enabled)
			return;
		
		Dirty = false;
		int numVerts	= _positions.Count*2;
		int numIndices	= (_positions.Count - 1)*6;

		Vector3 realRight	= _useWorldCoordinates && _transformRightAndUp ? transform.TransformDirection(Right)	: Right;
		Vector3 realUp		= _useWorldCoordinates && _transformRightAndUp ? transform.TransformDirection(Up)		: Up;

		if (numVerts > 2)
		{
			int verti				= 0;
			int indi				= 0;

			if (_vertices == null || _vertices.Length != numVerts)
			{
				_indices 	= new int[numIndices];
				_vertices	= new Vector3[numVerts];
				_normals	= new Vector3[numVerts];
				_uvs		= new Vector2[numVerts];
				_colors		= new Color[numVerts];
			}

			for (int i = 0; i < _positions.Count; i++)
			{
				if (i < _positions.Count - 1)
				{
					_indices[indi++]	= verti;
					_indices[indi++]	= verti + 2;
					_indices[indi++]	= verti + 3;
					_indices[indi++]	= verti + 3;
					_indices[indi++]	= verti + 1;
					_indices[indi++]	= verti;
				}

				float r = i/(float)(_positions.Count - 1);
				float radius		= Mathf.Lerp(_startWidth, _endWidth, r)*0.5f;
				Color color			= Color.Lerp(_startColor, _endColor, r);
				Vector3 pos			= _positions[i];

				if (_useWorldCoordinates)
					pos = transform.InverseTransformPoint(pos);

				_vertices[verti]	= pos - realRight*radius;
				_uvs[verti]			= new Vector2(r, 0);
				_colors[verti]		= color;
				_normals[verti]		= realUp;

				verti++;

				_vertices[verti]	= pos + realRight*radius;
				_uvs[verti]			= new Vector2(r, 1);
				_colors[verti]		= color;
				_normals[verti]		= realUp;
				verti++;
			}
			if (_mesh.vertexCount > 0)
				_mesh.Clear();
			_mesh.vertices			= _vertices;
			_mesh.uv				= _uvs;
			_mesh.colors			= _colors;
			_mesh.normals			= _normals;
			_mesh.SetIndices(_indices, MeshTopology.Triangles, 0);

			if (!GetComponent<Renderer>().enabled)
				GetComponent<Renderer>().enabled = true;
		}
		else if (GetComponent<Renderer>().enabled)
			GetComponent<Renderer>().enabled = false;

		Dirty = false;
	}

	private void Start()
	{
		if (GetComponent<Renderer>() == null)
			gameObject.AddComponent<MeshRenderer>();

		_filter = gameObject.GetComponent<MeshFilter>();
		if (_filter == null)
			_filter = gameObject.AddComponent<MeshFilter>();

		_mesh					= new Mesh();
		_filter.sharedMesh		= _mesh;
		
		GetComponent<Renderer>().sharedMaterial	= _material;
	}

	private void Update()
	{
		if (_useWorldCoordinates || Dirty)
			Rebuild();
	}
}
