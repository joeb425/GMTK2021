using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HexLibrary;
using Random = UnityEngine.Random;

public class ZoneHandler
{
	public List<Zone> zones = new List<Zone>();

	private bool IsSpreadingZone = false;
	private GroundTileComponent sourceTile = null;

	public void CreateZone(GroundTileComponent groundTile, ZoneData zoneData)
	{
		Zone zone = new Zone(zoneData);
		zone.AddTile(groundTile);
		zones.Add(zone);
	}

	public void StartSpreadingZone(GroundTileComponent sourceTile)
	{
		var buildNeighbors = sourceTile.zone.GetZoneNeighbors().Where(neighbor => neighbor.TileType == HexTileType.Build);
		foreach (var neighbor in buildNeighbors)
		{
			Game.Get.tileHighlighter.SetHexHighlighted(neighbor.hex, true, new Color(1.0f, 1.0f, 1.0f, 0.5f));
		}

		this.sourceTile = sourceTile;
		IsSpreadingZone = true;
	}

	public bool TrySpreadZoneToLocation(Hex targetHex)
	{
		if (!IsSpreadingZone)
		{
			return false;
		}

		var groundLayer = GameState.Get.Board.groundLayer;

		if (!groundLayer.GetHexComponent(targetHex, out GroundTileComponent targetTile))
		{
			CancelZoneSpread();
			return true;
		}

		if (!sourceTile.zone.GetZoneNeighbors().Contains(targetTile))
		{
			CancelZoneSpread();
			return true;
		}

		CancelZoneSpread();

		SpreadZone(sourceTile, targetTile);
		return true;
	}

	private void CancelZoneSpread()
	{
		IsSpreadingZone = false;

		foreach (Hex neighbor in sourceTile.zone.GetZoneNeighbors().Select(tile => tile.hex))
		{
			Game.Get.tileHighlighter.SetHexHighlighted(neighbor, false);
		}
	}

	private void SpreadZone(GroundTileComponent source, GroundTileComponent target)
	{
		Zone sourceZone = source.zone;
		if (sourceZone is null)
			return;

		target.zone = sourceZone;
		sourceZone.AddTile(target);
	}
}