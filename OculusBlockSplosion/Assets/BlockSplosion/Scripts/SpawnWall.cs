using UnityEngine;
using System.Collections.Generic;

public class SpawnWall : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> _blockPrefabs;

	[SerializeField]
	private float _blockScale;

	[SerializeField]
	private int _width;

	[SerializeField]
	private int _height;

	private List<GameObject> _spawned = new List<GameObject>();

	private Vector3 CalcPosition(int x, int y)
	{
		return transform.position + transform.right*(x + 0.5f - _width*0.5f)*_blockScale + transform.up*(y + 0.5f)*_blockScale;
	}

	private void Spawn()
	{
		if (_blockPrefabs.Count == 0)
		{
			Debug.LogError("No block prefabs!");
			return;
		}

		int _blockType = Random.Range(0, _blockPrefabs.Count);

		for (int y = 0; y < _height; y++)
		{
			for (int x = 0; x < _width; x++)
			{
				var block = Instantiate(_blockPrefabs[_blockType], CalcPosition(x, y), transform.rotation) as GameObject;
				_spawned.Add(block);
				_blockType = ++_blockType % _blockPrefabs.Count;
			}
		}
	}

	private void Update()
	{
		if (Input.GetButtonDown("Toggle"))
		{
			Spawn();
		}
		if (Input.GetButtonDown("Cancel"))
		{
			foreach (var block in _spawned)
			{
				if (block != null)
					Destroy(block);
			}
		}
	}

	private void Start()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit))
		{
			transform.position = hit.point;
		}

		_blockPrefabs.RemoveAll((GameObject obj) => { return obj == null; });

		Spawn();

	}

	private void OnDrawGizmos()
	{
		RaycastHit hit;
		if (Physics.Raycast(transform.position, Vector3.down, out hit))
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, hit.point);
		}
	}
}
