using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexLibrary;
using Random = UnityEngine.Random;

public class ZoneHandler
{
    public Dictionary<string, Material> zoneMats = new Dictionary<string, Material>();
    public Dictionary<string, List<Hex>> zoneLocs = new Dictionary<string, List<Hex>>();
    public Dictionary<string, List<TowerData>> zoneStats = new Dictionary<string, List<TowerData>>();
    public List<Zone> zones = new List<Zone>();

    public void InitializeSpread(string spread)
    {
        List<Material> zoneMats = GlobalData.GetAssetBindings().gameAssets.spreadMaterials;

        if (this.zoneMats.ContainsKey(spread))
        {
            return;
        }
        this.zoneMats[spread] = zoneMats[this.zoneMats.Count];
    }

    public void AddOrCreateZone(GroundTileComponent groundTile, ZoneData zoneData)
    {
        if (Game.Get.gameState == null)
        {
            Debug.Log("Gamestate null");
        }

        if (Game.Get.gameState.Board == null)
        {
            Debug.Log("Board is null");
        }

        
        var groundLayer = Game.Get.gameState.Board.groundLayer;

 
        Debug.Log("test");
        for (int i = 0; i < 6; i++)
        {
            Hex neighbor = groundTile.hex.Neighbor(i);

            if (groundLayer.GetTile(neighbor, out var neighborTile))
            {
                Zone neighborZone = neighborTile.GetComponent<GroundTileComponent>().zone;
                if (neighborZone != null)
                {
                    // Debug.Log($"Found neighbor {neighborZone.ZoneId}");
                    neighborZone.AddTile(groundTile);
                    return;
                }
            }
        }

        Zone zone = new Zone(zoneData);
        zone.AddTile(groundTile);
        // GlobalData.GetAssetBindings().gameAssets.testZoneData;
        zones.Add(zone);
        // Debug.Log($"Made zone {zone.ZoneId}");
    }

    public void CreateZone(GroundTileComponent groundTile)
    {
        
    }

    public void AddToZone(GroundTileComponent groundTile)
    {
        
    }

    public void AddSpread(string spreadName, Hex HexLoc, Tower tower = null)
    {
        // Add the spread location to the location
        if (zoneLocs.ContainsKey(spreadName))
        {
            if (!zoneLocs[spreadName].Contains(HexLoc))
            {
                zoneLocs[spreadName].Add(HexLoc);
            }
        }
        
        if (tower!=null)
        {
            zoneStats[spreadName].Add(tower.towerData);
        }
    }

    public Material GetSpreadMat(string spread)
    {
        return zoneMats[spread];
    }

}
