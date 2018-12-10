using UnityEngine;
using System.Collections;

public class AttachToParent : MonoBehaviour
{

	[SerializeField]
	Transform	TransformParent = null;
	
	private void Awake()
	{
		print( "Attaching to parent!" );
		transform.parent = TransformParent;
	}
};