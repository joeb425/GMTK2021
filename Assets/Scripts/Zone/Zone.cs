using System.Collections.Generic;
using Attributes;
using HexLibrary;
using UnityEngine;

public class Zone
{
	public List<GroundTileComponent> groundTiles = new List<GroundTileComponent>();
	public ZoneData zoneData;

	public Zone(ZoneData zoneData)
	{
		this.zoneData = zoneData;
	}

	public void AddTile(GroundTileComponent tile)
	{
		tile.zone = this;
		groundTiles.Add(tile);

		MeshRenderer[] temp = tile.transform.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer t in temp)
			if (t.gameObject.name == "Outline")
				t.material = GlobalData.GetAssetBindings().gameAssets.spreadMaterials[0];

		// Probably will make something else to indicate but for now any mesh renderer
		//tileMaterial.material = zoneHandler.GetSpreadMat(tempSpread);

		foreach (GameplayEffect effect in zoneData.zoneEffects)
		{
			tile.AddTowerEffect(effect); 
		}
	}

	public void UpdateTowers()
	{
		// Debug.Log("Update towers");
		foreach (GroundTileComponent tile in groundTiles)
		{
			var towerLayer = GameState.Get.Board.towerLayer;
			if (towerLayer.GetTile(tile.hex, out var gameObject))
			{
				Tower tower = gameObject.GetComponent<Tower>();
				// Debug.Log("Apply effect");
				tower.Attributes.ApplyEffect(GlobalData.GetAssetBindings().gameAssets.testEffect);
			}
		}
	}
}