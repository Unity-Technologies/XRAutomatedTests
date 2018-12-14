using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode]
public class BlockPrefabManager : MonoBehaviour
{
	private static BlockPrefabManager _instance;

	[System.Serializable]
	public class BlockPrefabInfo
	{
		public string		Name = ".";
		public GameObject	Prefab;
	}

	[SerializeField]
	private List<BlockPrefabInfo>	_blockPrefabs					= new List<BlockPrefabInfo>();

	public static GameObject GetPrefab(string name)
	{
		if (_instance != null)
			return _instance.GetPrefabInternal(name);
		return null;
	}

	public static Color GetBlockColor(string name)
	{
		if (_instance != null)
			return _instance.GetBlockColorInternal(name);
		return Color.magenta;
	}

	private Color GetBlockColorInternal(string name)
	{
		var prefab = GetPrefabInternal(name);

		if (prefab != null)
		{
			var block = prefab.GetComponent<Block>();
			if (block != null)
				return block.EditorColor;
		}

		return Color.magenta;
	}

	private GameObject GetPrefabInternal(string name)
	{
		foreach (var info in _blockPrefabs)
		{
			if (string.Equals(info.Name, name, System.StringComparison.InvariantCultureIgnoreCase))
				return info.Prefab;
		}

		return null;
	}

	private void Awake()
	{
		_instance = this;
	}

	private void Update()
	{
		if (_instance == null)
			_instance = this;

		foreach (var info in _blockPrefabs)
		{
			if (info.Name == "." && info.Prefab != null)
				info.Name = info.Prefab.name;
		}
	}
}
