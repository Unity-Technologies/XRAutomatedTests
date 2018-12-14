using UnityEngine;

public class ShadowText : MonoBehaviour
{
	[SerializeField]
	private float		_shadowRadius	= 0.02f;

	[SerializeField]
	private float		_zOffset		= 0.02f;

	[SerializeField]
	private Color		_shadowColor	= Color.black;

	[SerializeField]
	private TextMesh	_toShadow;

	private void Setup(float x, float y)
	{
		var obj						= new GameObject("TextShadow");
		obj.transform.parent		= _toShadow.transform;
		obj.transform.localPosition	= new Vector3(x, y, _zOffset);
		obj.transform.localRotation	= Quaternion.identity;
		obj.transform.localScale	= Vector3.one;

		var mesh			= obj.AddComponent<TextMesh>();
		mesh.color			= _shadowColor;
		mesh.font			= _toShadow.font;
		mesh.fontSize		= _toShadow.fontSize;
		mesh.fontStyle		= _toShadow.fontStyle;
		mesh.characterSize	= _toShadow.characterSize;
		mesh.alignment		= _toShadow.alignment;
		mesh.anchor			= _toShadow.anchor;
		mesh.GetComponent<Renderer>().sharedMaterial = _toShadow.GetComponent<Renderer>().sharedMaterial;

		var copy	= obj.AddComponent<CopyText>();
		copy.Src	= _toShadow;
		copy.Dest	= mesh;
	}

	private void Start()
	{
		if (_toShadow == null)
			_toShadow = GetComponent<TextMesh>();
		Setup(-_shadowRadius, 0);
		Setup(_shadowRadius, 0);
		Setup(0, -_shadowRadius);
		Setup(0, _shadowRadius);
	}
}
