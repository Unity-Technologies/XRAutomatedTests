using UnityEngine;
using System.Collections;

public class ExplodeBlockText : MonoBehaviour
{
	[SerializeField]
	private string		ExplodeSound = "BigExplosion";

	[SerializeField]
	private GameObject	ExplodeFX = null;
	
	[SerializeField]
	private string		UnitPrefix = "Unit";

	[SerializeField]
	private	GameObject	ExplodeOrigin = null;
	
	[SerializeField]
	private	float		ExplodeDelay = -1.0f;
	
	[SerializeField]
	private float		MinVelocity = 0.25f;
	
	[SerializeField]
	private float		MaxVelocity = 0.5f;
	
	[SerializeField]
	private float		MinRotation = -Mathf.PI;
	
	[SerializeField]
	private float		MaxRotation = Mathf.PI;	
	
	private float		ExplodeTime = -1.0f;
	
	public void Trigger( float delaySeconds )
	{
		ExplodeTime = Time.time + delaySeconds;
	}
	
	void InitUnits( GameObject obj, string unitPrefix )
	{
		//print( "InitUnits( " + obj.name + " )" );
		foreach ( Transform child in obj.transform )
		{
			if ( child.gameObject.name.IndexOf( unitPrefix ) != 0 )
			{
				InitUnits( child.gameObject, unitPrefix );
			}
			else
			{
				//print( "Adding MeshCollider and RigidBody to " + child.gameObject.name + "." );
				child.gameObject.AddComponent<MeshCollider>();
				MeshCollider mc = child.gameObject.GetComponent<MeshCollider>();
				mc.convex = true;
				Rigidbody rb = child.gameObject.AddComponent<Rigidbody>() as Rigidbody;
				// TODO: may need to set the mesh on the mesh collider
				rb.constraints = RigidbodyConstraints.FreezeAll;
				// add the component to remove the block once it settles
				child.gameObject.AddComponent<RemoveObjectOnceSettled>(  );
			}
		}
	}
	
	void Explode( GameObject obj, Vector3 explodeOrigin, string unitPrefix )
	{
		//print( "Explode( " + obj.name + " );" );
		
		foreach ( Transform child in obj.transform )
		{
			if ( child.gameObject.name.IndexOf( unitPrefix ) != 0 )
			{
				Explode( child.gameObject, explodeOrigin, unitPrefix );
			}
			else
			{
				// this is one of the unit pieces, break it's parent transform and
				// give it a velocity
				Vector3 worldPos = child.parent.position + ( child.parent.rotation * child.position );
				
				Rigidbody rb = child.gameObject.GetComponent<Rigidbody>();
				// release all constraints
				rb.constraints = RigidbodyConstraints.None;				
				
				// impart velocity
				Vector3 dir = new Vector3();
				dir = worldPos - explodeOrigin;
				dir.Normalize();
				rb.velocity = dir * Random.Range( MinVelocity, MaxVelocity );	
				rb.angularVelocity = new Vector3( Random.Range( -1.0f, 1.0f ), Random.Range( -1.0f, 1.0f ), 
						Random.Range( -1.0f, 1.0f ) ) * Random.Range( MinRotation, MaxRotation );
				
			}
		}	
		obj.transform.DetachChildren();
	}
	
	private void Start()
	{
		InitUnits( gameObject, UnitPrefix );
		if ( ExplodeDelay >= 0.0f )
		{
			ExplodeTime = Time.time + ExplodeDelay;
		}
	}
	
	void Update()
	{
		if ( ExplodeTime >= 0.0f && Time.time >= ExplodeTime )
		{
			AudioManager.Instance.PlayAt( ExplodeSound, ExplodeOrigin.transform.position, 1.0f, 0.0f );
			if ( ExplodeFX != null )
			{
				Instantiate( ExplodeFX, ExplodeOrigin.transform.position, ExplodeOrigin.transform.rotation );
			}
			Explode( gameObject, ExplodeOrigin.transform.position, UnitPrefix );
			ExplodeTime = -1.0f;
		}
	}
	
};