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
		{
			if (t.gameObject.name == "Base")
			{
				t.material.color = zoneData.zoneColor;
			}
		}

		foreach (GameplayEffect effect in zoneData.zoneEffects)
		{
			tile.AddTowerEffect(effect); 
		}
	}
}