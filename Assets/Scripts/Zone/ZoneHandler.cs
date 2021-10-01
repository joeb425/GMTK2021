using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using HexLibrary;
using Misc;
using Random = UnityEngine.Random;

public class ZoneHandler
{
	public List<Zone> zones = new List<Zone>();

	public bool isSpreadingZone = false;
	private GroundTileComponent sourceTile = null;

	public void CreateZone(GroundTileComponent groundTile, ZoneData zoneData)
	{
		Zone zone = new Zone(zoneData);
		zone.AddTile(groundTile);
		zones.Add(zone);
	}

	public void StartSpreadingZone(GroundTileComponent sourceTile)
	{
		if (sourceTile.zone != null)
			{
			var buildNeighbors = sourceTile.zone.GetZoneNeighbors().Where(neighbor => neighbor.TileType == HexTileType.Build);
			foreach (var neighbor in buildNeighbors)
			{
				var highlight = Game.Get.tileHighlighter.SetHexHighlighted(neighbor.hex, true, new Color(.5f, .5f, 0.0f, 0.75f));
				// if (highlight != null)
				// {
				// 	var pulse = highlight.AddComponent<MaterialPulse>();
				// 	pulse.minAlpha = 0.0f;
				// 	pulse.maxAlpha = 0.3f;
				// }
			}

			this.sourceTile = sourceTile;
			isSpreadingZone = true;
		}
	}

	public bool TrySpreadZoneToLocation(Hex targetHex)
	{
		if (!isSpreadingZone)
		{
			return false;
		}

		var groundLayer = GameState.Get.Board.groundLayer;

		if (!groundLayer.GetTileAtHex(targetHex, out GroundTileComponent targetTile))
		{
			CancelZoneSpread();
			return false;
		}

		if (targetTile.TileType != HexTileType.Build)
		{
			CancelZoneSpread();
			return false;
		}

		if (!sourceTile.zone.GetZoneNeighbors().Contains(targetTile))
		{
			CancelZoneSpread();
			return false;
		}

		CancelZoneSpread();

		SpreadZone(sourceTile, targetTile);
		return true;
	}

	private void CancelZoneSpread()
	{
		isSpreadingZone = false;

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