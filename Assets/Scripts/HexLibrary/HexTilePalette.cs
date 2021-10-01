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
		public HexTileComponent tilePrefab;

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

		public Dictionary<string, List<HexTileComponent>> GetPaletteAsDictionary()
		{
			Dictionary<string, List<HexTileComponent>> dictionary = new Dictionary<string, List<HexTileComponent>>();
			foreach (HexTileLayerPalette layerPalette in tilePalette)
			{
				List<HexTileComponent> objectsByIndex = layerPalette.spawnDatas.Select(spawnData => spawnData.tilePrefab).ToList();
				dictionary.Add(layerPalette.layerName, objectsByIndex);
			}

			return dictionary;
		}
	}
}