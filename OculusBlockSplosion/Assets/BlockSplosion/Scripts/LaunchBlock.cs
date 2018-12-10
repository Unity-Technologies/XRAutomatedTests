using UnityEngine;
using System.Collections.Generic;

public class LaunchBlock : MonoBehaviour
{
	[SerializeField]
	private List<GameObject> _blocks;

	private List<GameObject> _launched = new List<GameObject>();

	[SerializeField]
	private float 		_yOffset			= -2;

	[SerializeField]
	private float 		_zOffset			= 2;

	[SerializeField]
	private float 		_xOffset			= 1;

	[SerializeField]
	private float 		_launchSpeed		= 10;

	[SerializeField]
	private float 		_angularSpeed		= 1;

	private int 		_index = 0;

	private GameObject	_shot;

	private int 		_hackUpdateCount;

	public void Launch()
	{
		if ( _shot == null )
		{
			// this can happen if the level resets between instantiating a shot and firing it
			return;	
		}	
		GameObject bgm = GameObject.Find( "BlockGameManager" );
		if ( bgm != null ) 
		{
			bgm.GetComponent< BlockGameManager >().AddBlock( _shot );
		}				
		// play using the shot object's audio source, before _shot is added to _launched and cleared
		AudioManager.Instance.Play( _shot.GetComponent<AudioSource>(), "Launch", 1.0f );
		_shot.transform.parent			= null;
		_launched.Add(_shot);
		// We must break the constraint first, because in Unity 5 this will reset the velocity.
		_shot.GetComponent<Rigidbody>().constraints		= RigidbodyConstraints.None;
		_shot.GetComponent<Rigidbody>().velocity		= Camera.main.transform.forward*_launchSpeed;
		_shot.GetComponent<Rigidbody>().useGravity		= true;
		_shot.GetComponent<Collider>().isTrigger		= false;
		_shot							= null;
		Score.ShotsFired++;
	}
	
	public void Update()
	{
		if ( Score.Completed || !BlockGameManager.IsActive )
			return;

		if ( _hackUpdateCount < 2 ) //Parenting transform is broken on camera if done in the first 2 frames(?)
		{
			_hackUpdateCount++;
			return;
		}

		if ( _shot == null )
		{
			_shot = Instantiate(_blocks[_index], Camera.main.transform.position + Camera.main.transform.up*_yOffset + Camera.main.transform.forward*_zOffset + Camera.main.transform.right*_xOffset, Random.rotation) as GameObject;

			_shot.transform.parent			= Camera.main.transform;
			_shot.GetComponent<Rigidbody>().useGravity		= false;
			_shot.GetComponent<Rigidbody>().constraints		= RigidbodyConstraints.FreezePosition;
			_shot.GetComponent<Rigidbody>().angularVelocity	= Random.insideUnitSphere*_angularSpeed;
			_shot.GetComponent<Collider>().isTrigger		= true;
			_index = ++_index % _blocks.Count;
		}
	}
	
	public void DestroyLaunchedProjectiles()
	{
		foreach ( var projectile in _launched )
		{
			if (projectile != null)
				Destroy(projectile);
		}
		_launched.Clear();	
	}
}
