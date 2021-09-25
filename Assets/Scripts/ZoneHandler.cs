using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HexLibrary;

public class ZoneHandler
{
    public Dictionary<string, Material> zoneMats = new Dictionary<string, Material>();
    public Dictionary<string, List<Hex>> zoneLocs = new Dictionary<string, List<Hex>>();
    public Dictionary<string, List<TowerData>> zoneStats = new Dictionary<string, List<TowerData>>();

    public void InitializeSpread(string spread)
    {
        List<Material> zoneMats = GlobalData.GetAssetBindings().gameAssets.spreadMaterials;

        if (this.zoneMats.ContainsKey(spread))
        {
            return;
        }
        this.zoneMats[spread] = zoneMats[this.zoneMats.Count];
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
