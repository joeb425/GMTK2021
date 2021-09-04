using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace.HexGridEditor
{
	[Serializable]
	public class HexTileSpawnInfo
	{
		[SerializeField]
		public GameObject tilePrefab;

		[SerializeField]
		public HexTileType tileType;

		[SerializeField]
		public KeyCode spawnKeyCode;
	}


	[CreateAssetMenu(menuName = "Data/HexTileSpawnData")]
	public class HexTileSpawnData : ScriptableObject
	{
		public List<HexTileSpawnInfo> spawnData;
	}
}