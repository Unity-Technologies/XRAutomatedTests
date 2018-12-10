using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
	using UnityEditor;
#endif

[ExecuteInEditMode]
public class BlockLevel : MonoBehaviour
{
	[System.Serializable]
	public class BlockSpawnInfo
	{
		public string		Name;
		public Vector3		Position;
		public Quaternion	Rotation;

		public BlockSpawnInfo(Block block, Transform parent)
		{
			if (block != null)
			{
				Name		= block.PrefabName;
				Position	= parent.InverseTransformPoint(block.transform.position);
				Rotation	= Quaternion.Inverse(parent.transform.rotation)*block.transform.rotation;
			}
		}
	}

	public static void DeleteAllTheThings()
	{
		foreach (var block in GameObject.FindObjectsOfType(typeof(Block)))
		{
#if UNITY_EDITOR
			Undo.DestroyObjectImmediate( ( block as Block ).gameObject );
#else
			DestroyImmediate((block as Block).gameObject);
#endif
		}
	}

	[SerializeField]
	private bool _shouldCopyAllTheThings;

	[SerializeField]
	private bool _shouldDeleteAllTheThings;

	[SerializeField]
	private bool _showLevel = true;

	[SerializeField]
	private bool _shouldSpawnLevel;

	[SerializeField]
	private List<BlockSpawnInfo> _levelData = new List<BlockSpawnInfo>();

	[SerializeField]
	private int _threeStarShots = 3;

	[SerializeField]
	private int _twoStarShots	= 5;

	[SerializeField]
	private bool _infinitePlay;
	
	[SerializeField]
	private string	_newLevelSound;
	
	public void CopyAllTheThings()
	{
		_levelData.Clear();
		foreach (var block in GameObject.FindObjectsOfType(typeof(Block)))
		{
			var realBlock = block as Block;
			if (realBlock.gameObject.activeInHierarchy)
				_levelData.Add(new BlockSpawnInfo(realBlock, transform));
		}

		BeDirty();
	}

	public void SpawnLevel( List< GameObject > spawnedBlocks )
	{
		spawnedBlocks.Clear();

		Score.ThreeStars	= _threeStarShots;
		Score.TwoStars		= _twoStarShots;
		Score.InfinitePlay	= _infinitePlay;
		foreach (var spawn in _levelData)
		{
			var prefab = BlockPrefabManager.GetPrefab(spawn.Name);
			if ( prefab != null ) {
				//print( Time.time + " | Spawning prefab " + prefab.name );
				GameObject b = Instantiate(prefab, transform.TransformPoint(spawn.Position), transform.rotation*spawn.Rotation) as GameObject;
				spawnedBlocks.Add( b );
			}
		}

		if ( !string.IsNullOrEmpty( _newLevelSound ) )
		{
			AudioManager.Instance.PlayAt( _newLevelSound, transform.position, 1.0f );
		}		
	}

	private void BeDirty()
	{
#if UNITY_EDITOR
		if (!Application.isPlaying)
			EditorUtility.SetDirty(this);
#endif
	}

	private void Update()
	{
		if (_shouldCopyAllTheThings)
		{
#if UNITY_EDITOR
			//Undo.SetSnapshotTarget(this, "Changed BlockLevel");
			//Undo.CreateSnapshot();
			Undo.RecordObject( this, "Changed Blocklevel" );
			BeDirty();
#endif
			_shouldCopyAllTheThings = false;
			CopyAllTheThings();
		}
		// _shouldSpawnLevel is never true?
/*
		if (_shouldSpawnLevel)
		{
			_shouldSpawnLevel = false;
			#if UNITY_EDITOR
				// Nelno: There doesn't seem to be an equivalent to this in 4.5.0b4
				//Undo.RegisterSceneUndo("Spawned level " + name);
			#endif
			SpawnLevel();
			BeDirty();
		}
*/
		if (_shouldDeleteAllTheThings)
		{
			_shouldDeleteAllTheThings = false;
			#if UNITY_EDITOR
				//Undo.RegisterSceneUndo("Destroyed all blocks.");
			#endif
			DeleteAllTheThings();
			BeDirty();
		}
	}

	private void OnDrawGizmos()
	{
		if (_showLevel && !Application.isPlaying)
		{
			foreach (var spawn in _levelData)
			{
				Gizmos.color		= BlockPrefabManager.GetBlockColor(spawn.Name);
				Gizmos.matrix		= Matrix4x4.TRS(transform.TransformPoint(spawn.Position), transform.rotation*spawn.Rotation, Vector3.one);
				Gizmos.DrawWireCube(Vector3.zero, Vector3.one*2);
				Gizmos.DrawSphere(Vector3.zero, 0.5f);
			}
		}
	}
}
