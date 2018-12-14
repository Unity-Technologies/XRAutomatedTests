using UnityEngine;
using System.Collections;

public class CopyText : MonoBehaviour
{
	[SerializeField]
	private TextMesh _src;

	[SerializeField]
	private TextMesh _dest;

	public TextMesh Src
	{
		get { return _src; } 
		set { _src = value; }
	}

	public TextMesh Dest
	{
		get { return _dest; }
		set { _dest = value; }
	}

	private void Update()
	{
		if (_dest.text != _src.text)
			_dest.text = _src.text;
	}
}
