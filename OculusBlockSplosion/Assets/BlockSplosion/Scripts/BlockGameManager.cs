using UnityEngine;
using System.Collections.Generic;

public class BlockGameManager : MonoBehaviour
{
	private static BlockGameManager _instance;
	
	[SerializeField]
	private int					_startIndex = 0;
	
	[SerializeField]
	private List<BlockLevel>	_levels = new List<BlockLevel>();

	[SerializeField]
	private float				_startDelay = 5.0f;
	
	private	float				_startTime = 0.0f;
	private bool				_started = false;
	
	private int					_index;

	public static bool IsActive { get { return _instance != null; } }
	
	private List< GameObject >	_allBlocks = new List< GameObject >();
	
	[SerializeField]
	private float				_DelayBeforeNextLevel;
	
	private float				NextLevelStartTime = -1.0f;

	public LaunchBlock 			LaunchBlockInstance;
	public MouseLookBS 			MouseLookInstance;
	
	public void AddBlock( GameObject b ) {
		_allBlocks.Add( b );
	}
	
	public void RemoveBlock( GameObject b ) {
		_allBlocks.Remove( b );
	}
	
	private void CaptureMouse()
	{
		bool capture = !MouseLookInstance.GetCaptureMouseCursor();
		MouseLookInstance.SetCaptureMouseCursor( capture );
	}
	
	private void LaunchBlock()
	{
		if ( LaunchBlockInstance != null )
		{
			LaunchBlockInstance.Launch();
		}
	}	
	
	private void Start()
	{
		Score.TitleScreen = true;
		_started = false;
		_startTime = Time.time + _startDelay;
	}
	
	private void ReallyStart()
	{
		Score.TitleScreen = false;
		_started = true;
		_index = _startIndex;
		_instance		= this;
		for (int i = 0; i < _levels.Count; i++)
		{
			var level	= _levels[i];
			var obj		= Instantiate(level.gameObject) as GameObject;
			_levels[i]	= obj.GetComponent<BlockLevel>();
			obj.SetActive(false);
		}
		SpawnLevel();
	}

	public void SpawnLevel()
	{
		// restart the current level
		if ( LaunchBlockInstance != null )
		{
			LaunchBlockInstance.DestroyLaunchedProjectiles();
		}	

		_allBlocks.Clear();		
		
		BlockLevel.DeleteAllTheThings();
		
		var level = _levels[_index];
		level.gameObject.SetActive(true);
		level.SpawnLevel( _allBlocks );
		Score.ShotsFired	= 0;
		Score.MaxStars		= Block.GetCount(Block.Type.Special);
		TimeController.ResumeTime();
		NextLevelStartTime = -1.0f;
	}

	public void NextLevel()
	{
		//print( "NextLevel" );
		var level	= _levels[_index];
		level.gameObject.SetActive(false);
		
		_index++;
		if ( _index >= _levels.Count )
		{
			_index = _startIndex;
		}
		SpawnLevel();
	}

	private void Update()
	{
		if ( Time.time < _startTime )
		{
			return;
		}
		if ( !_started )
		{
			ReallyStart();
		}
		Score.StarsLeft		= Block.GetCount(Block.Type.Special);
		if ( Score.StarsLeft == 0 && NextLevelStartTime < 0.0f )
		{
			NextLevelStartTime = Time.time + _DelayBeforeNextLevel;
		} 
		
		if ( NextLevelStartTime >= 0.0f )
		{
			if ( Time.time >= NextLevelStartTime )
			{
				//print( "Calling NextLevel from Update" );
				NextLevel();
			}
		}

		HandleInput();
	}

	private void HandleInput()
	{
#if UNITY_EDITOR
		// keyboard for testing in Unity editor
		if (Input.GetKeyDown(KeyCode.L))
		{
			CaptureMouse();
		}

		if (Input.GetKeyDown(KeyCode.Space))
		{
			LaunchBlock();
		}

		if (Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.Mouse1))
		{
			NextLevel();
		}

		// mouse for testing in Unity editor (touchpad maps to mouse, but we only
		// want this functionality for testing)
#endif

        // touchpad
        if (OVRInput.GetActiveController() == OVRInput.Controller.Touchpad)
        {

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                LaunchBlock();
            }
        }
        else
        {
            // gamepad
            if (OVRInput.GetDown(OVRInput.Button.One))
            {
                LaunchBlock();
            }

            if (OVRInput.GetDown(OVRInput.Button.Two))
            {
                SpawnLevel();
            }

            if (OVRInput.GetDown(OVRInput.Button.PrimaryShoulder | OVRInput.Button.PrimaryIndexTrigger))
            {
                NextLevel();
            }
        }
    }
	
	private void DrawPoint(Vector3 pos, float size, Color c, float duration, bool depth)
	{
		Vector3 xaxis = new Vector3( size * 0.5f, 0, 0 );
		Vector3 yaxis = new Vector3( 0, size * 0.5f, 0 );
		Vector3 zaxis = new Vector3( 0, 0, size * 0.5f );
		Debug.DrawLine( pos - xaxis, pos + xaxis, c, duration, depth );
		Debug.DrawLine( pos - yaxis, pos + yaxis, c, duration, depth );
		Debug.DrawLine( pos - zaxis, pos + zaxis, c, duration, depth );		
	}
	
	private void LateUpdate() 
	{
		const int MAX_POSITIONS = 20;
		const float DIST_CAP = 1000.0f;
		// go through all blocks and calculate their distance from the ground plane
		GameObject HugePlane = GameObject.Find( "HugePlane" );
		List< int > blockOrder = new List< int >();
		for ( int i = 0; i < _allBlocks.Count; ++i ) 
		{
			GameObject go = _allBlocks[i];
			if ( go == null || !go.GetComponent<Renderer>().isVisible ) 
			{
				continue;
			}
			float dist = Mathf.Abs( go.transform.position.y - HugePlane.transform.position.y );
			if ( dist > DIST_CAP ) 
			{
				continue;
			}
			Block b = go.GetComponent<Block>();
			if ( b == null )
			{
				Debug.LogWarning( "GameObject '" + go.name + "' has no Block behavior." );
				continue;
			}
			b.SortDistance = dist;
			int j = 0;
			for ( ; j < blockOrder.Count; ++j ) 
			{			
				if ( j == i ) 
				{
					continue;
				}
				int idx = blockOrder[j];
				if ( _allBlocks[idx].GetComponent<Block>().SortDistance > dist ) 
				{
					blockOrder.Insert( j, i );
					break;
				}
			}
			if ( j >= blockOrder.Count ) 
			{
				blockOrder.Add( i );
			}
		}
		
		// set the position vector the shader 
		for ( int i = 0; i < blockOrder.Count && i < MAX_POSITIONS; ++i ) 
		{
			int blockIndex = blockOrder[i];
			string globalName = "_Position" + i.ToString();
			Shader.SetGlobalVector( globalName, _allBlocks[blockIndex].transform.position );
			DrawPoint( _allBlocks[blockIndex].transform.position, 3.0f, Color.white, 0.1f, true );
			//float dist = _allBlocks[blockIndex].GetComponent<Block>().SortDistance;
			//print( globalName + " - " + _allBlocks[blockIndex].name + " = " + wspos );
		}
		Shader.SetGlobalFloat( "_NumPositions", blockOrder.Count );
		for ( int i = blockOrder.Count; i < MAX_POSITIONS; ++i ) 
		{
			Vector3 pos = new Vector3( 9999.0f, 9999.0f, 9999.0f );
			Shader.SetGlobalVector( "_Position" + i.ToString(), pos );
		}	
	}
}
