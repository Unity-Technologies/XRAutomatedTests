using UnityEngine;
using System.Collections;

public class RemoveObjectOnceSettled : MonoBehaviour
{
	[SerializeField]
	private float	RemoveDelay = 0.1f;
	
	private	int		NumStaticFrames = 0;
	
	void Start()
	{
		StartCoroutine( "RemoveOnceSettled" );
	}
	
	IEnumerator RemoveOnceSettled()
	{
		while( true )
		{
			yield return new WaitForSeconds( RemoveDelay );
			if ( GetComponent<Rigidbody>().constraints != RigidbodyConstraints.None )
			{
				// never remove if constrained
				NumStaticFrames = 0;
				continue;
			}

			// test the velocity... we must not be moving for 2 checks in a row to remove
			if ( Mathf.Abs( GetComponent<Rigidbody>().velocity.magnitude ) > 0.001f )
			{
				NumStaticFrames = 0;
				continue;
			}
			else
			{
				NumStaticFrames++;
				if ( NumStaticFrames > 1 )
				{
					GameObject.Destroy( gameObject );
				}
			}
		}
	}
};