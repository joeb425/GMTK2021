using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexLibrary
{
	[Serializable]
	public class HexTileSpawnData
	{
		[SerializeField]
		public GameObject tilePrefab;

		[SerializeField]
		public KeyCode spawnKeyCode;
	}
	
	[Serializable]
	public class HexTileLayerPalette
	{
		[SerializeField]
		public string layerName;

		[SerializeField]
		public List<HexTileSpawnData> spawnDatas;
	}

	[CreateAssetMenu(menuName = "Data/HexTilePalette")]
	public class HexTilePalette : ScriptableObject
	{
		[SerializeField]
		public List<HexTileLayerPalette> tilePalette;

		public HexTileLayerPalette GetLayerPalette(string layerName)
		{
			return tilePalette.FirstOrDefault(palette => palette.layerName == layerName);
		}

		public Dictionary<string, List<GameObject>> GetPaletteAsDictionary()
		{
			Dictionary<string, List<GameObject>> dictionary = new Dictionary<string, List<GameObject>>();
			foreach (HexTileLayerPalette layerPalette in tilePalette)
			{
				List<GameObject> objectsByIndex = layerPalette.spawnDatas.Select(spawnData => spawnData.tilePrefab).ToList();
				dictionary.Add(layerPalette.layerName, objectsByIndex);
			}

			return dictionary;
		}
	}
}