using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Zone;
using HexLibrary;
using Mantis.Hex;
using Mantis.Utils;
using UnityEditor;

public class GameBoard : MonoBehaviour
{
	[SerializeField]
	public HexGrid grid;

	[HideInInspector]
	public HexGridLayer groundLayer;

	[HideInInspector]
	public HexGridLayer towerLayer;

	[HideInInspector]
	public HexGridLayer foliageLayer;

	[HideInInspector]
	public HexGridLayer tempTowerLayer;

	public event System.Action<Hex, Hex> OnSelectedTileChanged;	
	public event System.Action<Hex, Hex> OnHoveredTileChanged;

	public event System.Action<Hex, Tower> OnTowerPlaced;
	public event System.Action<Tower> OnSetTowerToBePlaced;

	// Todo: enemy path should be somewhere else?
	public List<Hex> enemyPath = new List<Hex>();

	[HideInInspector]
	public Hex selectedTile;

	[HideInInspector]
	public Hex hoveredTile;

	public Tower towerToBePlaced;
	public Tower towerToBePlacedPrefab;

	public ZoneHandler zoneHandler = new ZoneHandler();

	public HexGridLayer GetGroundLayer()
	{
		if (groundLayer == null)
			groundLayer = grid.GetLayer("Ground");

		return groundLayer;
	}

	public HexGridLayer GetTowerLayer()
	{
		if (towerLayer == null)
			towerLayer = grid.GetLayer("Tower");

		return towerLayer;
	}


	public void Initialize()
	{
		OnSelectedTileChanged += SelectedTileChanged;

		// grid.Init();
		GameData gameData = GameData.Get();
		TextAsset levelToLoad = gameData.GetCurrentLevel().level;
		Debug.Log($"Load level {GlobalData.CurrentLevel} : {levelToLoad.name} : {levelToLoad.text}");
		grid.LoadLevelFromJson(levelToLoad.text);

		groundLayer = grid.GetLayer("Ground");
		if (groundLayer == null)
		{
			Debug.Log("ground layer invalid");
		}

		towerLayer = grid.GetLayer("Tower");

		foliageLayer = grid.GetLayer("Foliage");

		if (towerLayer == null)
		{
			Debug.Log("tower layer invalid");
		}

		tempTowerLayer = grid.AddLayer("tempTower");

		enemyPath = new HexPathFinding().FindPath2(grid);

		InitZones();
	}

	public void GameUpdate()
	{
		// TODO optimize this
		foreach (HexTileComponent tower in towerLayer.hexGrid.Values)
		{
			tower.GameUpdate();
			// tower.GetComponent<Tower>().GameUpdate();
		}

		// for (int i = 0; i < updatingContent.Count; i++)
		// {
		// 	updatingContent[i].GameUpdate();
		// }
		
		grid.GetHexUnderRay(Game.Get.input.touchRay, out hoveredTile);

		// if (towerToBePlaced != null && hoveredTile != null)
		// {
		// 	towerToBePlaced.transform.position = hoveredTile.Content.transform.position;
		// }
	}

	public bool PlaceTower(Hex tile, Tower towerPrefab)
	{
		if (!GameState.Get().SpendCash(towerPrefab.towerData.towerCost))
			return false;

		Tower tower = Instantiate(towerPrefab);
		towerLayer.AddTile(tile, tower);
		foliageLayer.DeleteTile(tile);
		tempTowerLayer.DeleteTile(tile);
		OnTowerPlaced?.Invoke(tile, tower);

		Game.Get.GetAudioHandler().PlaySfx(GlobalData.GetAssetBindings().gameAssets.placeTowerSfx);
		return true;
	}

	public bool PlaceTowerAtHex(Hex tile, Tower tower)
	{
		if (tile == null || tower == null)
			return false;

		return PlaceTower(tile, tower);
	}

	public void PlaceTowerAtSelectedTile(Tower tower)
	{
		PlaceTowerAtHex(selectedTile, tower);
	}

	public bool BuyTower(Tower tower)
	{
		int towerCost = tower.towerData.towerCost;
		if (GameState.Get().CurrentCash >= towerCost)
		{
			GameState.Get().SetCash(GameState.Get().CurrentCash - towerCost);
			return true;
		}

		return false;
	}
	
	private void SelectedTileChanged(Hex oldTile, Hex newTile)
	{
		SetTileSelected(oldTile, false);
		SetTileSelected(newTile, true);
	}

	public void SelectTile(Hex newSelectedTile)
	{
		// if (GetGroundLayer().GetTileAtHex(newSelectedTile, out GroundTileComponent groundTile))
		// {
		// 	Debug.Log($"Effects {groundTile.effectList.effects.Count} Active {groundTile.effectList.activeEffects.Count}");
		// }
		

		if (zoneHandler.isSpreadingZone)
		{
			zoneHandler.TrySpreadZoneToLocation(newSelectedTile);
			return;
		}
		
		Hex oldSelectedTile = selectedTile;
		selectedTile = newSelectedTile;

		if (oldSelectedTile != selectedTile)
		{
			OnSelectedTileChanged?.Invoke(oldSelectedTile, selectedTile);
		}
	}

	private void SetTileSelected(Hex tile, bool selected)
	{
		if (tile is null)
		{
			return;
		}

		Game.Get.tileHighlighter.SetHexHighlighted(tile, selected, new Color(.5f, .5f, .5f, 0.35f));

		if (towerToBePlaced != null && towerToBePlaced.hex != tile)
		{
			CancelTowerToBePlaced();
		}

		// if (tile.Content)
		// {
		// 	// MeshRenderer[] renderers = tile.Content.GetComponentsInChildren<MeshRenderer>();
		// 	// foreach (MeshRenderer renderer in renderers)
		// 	// {
		// 	// 	if (renderer != null)
		// 	// 	{
		// 	// 		renderer.material.SetFloat("_OutlineWidth", selected ? 1.05f : 1.0f);
		// 	// 	}
		// 	// }
		//
		// 	Tower tower = tile.Content.GetComponent<Tower>();
		// 	tower.OnSelected(selected);
		// }
	}

	public bool PlaceCurrentTower()
	{
		// place tower
		if (towerToBePlaced != null)
		{
			return PlaceTowerAtHex(towerToBePlaced.hex, towerToBePlacedPrefab);
		}

		return false;
	}

	public void CancelTowerToBePlaced()
	{
		SetTowerToBePlaced(null);
	}

	public void SetTowerToBePlaced(Tower towerPrefab)
	{
		if (towerToBePlaced != null)
		{
			tempTowerLayer.DeleteTile(towerToBePlaced.hex);
			towerToBePlaced = null;
		}

		if (towerPrefab != null)
		{
			towerToBePlaced = Instantiate(towerPrefab);
			tempTowerLayer.AddTile(selectedTile, towerToBePlaced);
		}

		towerToBePlacedPrefab = towerPrefab;

		OnSetTowerToBePlaced?.Invoke(towerToBePlaced);
	}

	private void OnDrawGizmos()
	{
		if (selectedTile != null)
		{
			Gizmos.color = new Color(1, 1, 0, 0.5f);
			Gizmos.DrawSphere(grid.flat.HexToWorld(selectedTile), .1f);
		}

		Gizmos.color = new Color(1, 0, 0, 0.5f);
		foreach (Hex hex in enemyPath)
		{
			var worldPos = grid.flat.HexToWorld(hex);
			Gizmos.DrawSphere(worldPos, 0.25f);
		}
		
		foreach (Zone zone in zoneHandler.zones)
		{
			Gizmos.color = zone.zoneData.zoneColor;
			Debug.Log(zone.groundTiles.Count);

			foreach (GroundTileComponent groundTile in zone.groundTiles)
			{
				Gizmos.DrawSphere(grid.flat.HexToWorld(groundTile.hex), 0.5f);
			}
		}
	}

	public void CreateLink(Tower tower)
	{
	}

	public void StartSelectZoneSpread(Hex tile)
	{
		if (groundLayer.GetTileAtHex(tile, out GroundTileComponent groundTile))
		{
			zoneHandler.StartSpreadingZone(groundTile);
		}
	}

	private void InitZones()
	{
		HexGridLayer zoneLayer = grid.GetLayer("Zone");
		foreach (var kvp in zoneLayer.hexGrid)
		{
			ZonePrefabComponent zonePrefabComponent = kvp.Value.GetComponent<ZonePrefabComponent>();
			if (groundLayer.GetTileAtHex(kvp.Key, out GroundTileComponent groundTile))
			{
				zoneHandler.CreateZone(groundTile, zonePrefabComponent.zoneData);
			}
		}
	}

	public bool GetTowerAtHex(Hex hex, out Tower tower)
	{
		if (towerLayer.GetTileAtHex(hex, out tower))
		{
			return true;
		}

		tower = null;
		return false;
	}

	public bool GetGroundTileAtHex(Hex hex, out GroundTileComponent tile)
	{
		if (groundLayer.GetTileAtHex(hex, out tile))
		{
			return true;
		}

		tile = null;
		return false;
	}
}