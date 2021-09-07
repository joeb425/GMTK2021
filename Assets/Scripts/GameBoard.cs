using System;
using UnityEngine;
using System.Collections.Generic;
using HexLibrary;
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

	List<Hex> spawnPoints = new List<Hex>();
	
	public event System.Action<Hex, Hex> OnSelectedTileChanged;
	public event System.Action<Hex, Hex> OnHoveredTileChanged;
	
	public event System.Action<Hex, Tower> OnTowerPlaced;

	public List<Hex> enemyPath = new List<Hex>();

	[HideInInspector]
	public Hex selectedTile;

	[HideInInspector]
	public Hex hoveredTile;

	private Tower towerToBePlaced;
	private Tower towerToBePlacedPrefab;

	public void Initialize()
	{
		OnSelectedTileChanged += SelectedTileChanged;

		grid.Init();

		groundLayer = grid.GetLayer("Ground");
		if (groundLayer == null)
		{
			Debug.Log("ground layer invalid");
		}
		Debug.Log(groundLayer.hexGrid.Count);
		towerLayer = grid.GetLayer("Tower");

		foliageLayer = grid.GetLayer("Foliage");


		if (towerLayer == null)
		{
			Debug.Log("tower layer invalid");
		}

		enemyPath = new HexPathFinding().FindPath2(grid);
	}

	public void ToggleSpawnPoint(Hex tile)
	{
		// if (tile.Content.Type == HexContentType.SpawnPoint)
		// {
		// 	if (spawnPoints.Count > 1)
		// 	{
		// 		spawnPoints.Remove(tile);
		// 		tile.Content = contentFactory.Get(HexContentType.Path);
		// 	}
		// }
		// else if (tile.Content.Type == HexContentType.Path)
		// {
		// 	tile.Content = contentFactory.Get(HexContentType.SpawnPoint);
		// 	spawnPoints.Add(tile);
		// }
	}

	public Hex GetSpawnPoint(int index)
	{
		return spawnPoints[index];
	}

	public int SpawnPointCount => spawnPoints.Count;

	public void ToggleDestination(Hex tile)
	{
		// if (tile.Content.Type == HexContentType.Destination)
		// {
		// 	tile.Content = contentFactory.Get(HexContentType.Path);
		// 	if (!FindPaths())
		// 	{
		// 		tile.Content =
		// 			contentFactory.Get(HexContentType.Destination);
		// 		FindPaths();
		// 	}
		// }
		// else if (tile.Content.Type == HexContentType.Path)
		// {
		// 	tile.Content = contentFactory.Get(HexContentType.Destination);
		// 	FindPaths();
		// }
	}
	public void ToggleBuildSpot(Hex tile)
	{
		// updatingContent.Remove(tile.Content);
		// tile.Content = contentFactory.Get(HexContentType.Build);
		// updatingContent.Add(tile.Content);

	}
	public void ToggleTower(Hex tile, Tower towerPrefab)
	{
		Tower tower = Instantiate(towerPrefab);
		towerLayer.AddTile(tile, tower.gameObject);
		foliageLayer.DeleteTile(tile);
		Debug.Log(tile  + ", " + towerPrefab);

		OnTowerPlaced?.Invoke(tile, tower);

		// // tower.transform.position = grid.flat.HexToWorld(tile);
		// towerLayer.AddTile(tile, tower.gameObject);


		// tile.Content = tower.gameObject;

		// if (tile.Content.Type == HexContentType.Tower)
		// {
		// 	updatingContent.Remove(tile.Content);
		// 	tile.Content = contentFactory.Get(HexContentType.Build);
		// 	FindPaths();
		// }
		// else if (tile.Content.Type == HexContentType.Wall)
		// {
		// 	tile.Content = contentFactory.Get(HexContentType.Tower);
		// 	updatingContent.Add(tile.Content);
		// }
		// else if (tile.Content.Type ==  HexContentType.Build)
		// {
		// 	if (BuyTower(towerPrefab))
		// 	{
		// 		tile.Content = contentFactory.Get(towerPrefab);
		// 		updatingContent.Add(tile.Content);
		//
		// 		// update hud
		// 	}
		// }
	}

	public void ToggleWall(Hex tile)
	{
		// if (tile.Content.Type == HexContentType.Wall)
		// {
		// 	tile.Content = contentFactory.Get(HexContentType.Path);
		// 	FindPaths();
		// }
		// else if (tile.Content.Type == HexContentType.Path)
		// {
		// 	tile.Content = contentFactory.Get(HexContentType.Wall);
		// 	if (!FindPaths())
		// 	{
		// 		tile.Content = contentFactory.Get(HexContentType.Path);
		// 		FindPaths();
		// 	}
		// }
	}

	public Hex GetTile(Ray ray)
	{
		grid.GetHexUnderRay(ray, out var hex);
		return hex;
	}

	public void GameUpdate()
	{
		// TODO optimize this
		foreach (GameObject tower in towerLayer.hexGrid.Values)
		{
			tower.GetComponent<Tower>().GameUpdate();
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

	public void Update()
	{
		// hoveredTile = GetTile(Game.Get.Input.touchRay);
	}

	public void PlaceTowerAtTile(Hex tile, Tower tower)
	{
		if (tile == null || tower == null)
			return;

		ToggleTower(tile, tower);

		Debug.Log("place tower?");
	}

	public void PlaceTower(Tower tower)
	{
		PlaceTowerAtTile(selectedTile, tower);
	}

	public bool BuyTower(Tower tower)
	{
		int towerCost = tower.towerData.towerCost;
		if (GameState.Get.CurrentCash >= towerCost)
		{
			GameState.Get.SetCash(GameState.Get.CurrentCash - towerCost);
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
		// if (PlaceCurrentTower())
		// {
		// 	return;
		// }

		Hex oldSelectedTile = selectedTile;
		// HexContent oldSelectedContent = selectedTileContent;

		selectedTile = newSelectedTile; //GetTile(touchRay);
		// selectedTileContent = selectedTile.Content;

		if (oldSelectedTile != selectedTile)
		{
			OnSelectedTileChanged?.Invoke(oldSelectedTile, selectedTile);
		}
		// content may have changed as well
		else if (oldSelectedTile != null && selectedTile != null) 
		                                 // &&
		         // oldSelectedContent != selectedTileContent)
		{
			OnSelectedTileChanged?.Invoke(oldSelectedTile, selectedTile);
		}

		// bool showBuildMenu = selectedTile.Content.Type == HexContentType.Build;
		// SetBuildMenuEnabled(showBuildMenu);

		// bool showTowerUI = selectedTile.Content.Type == HexContentType.Tower;
		// uiHandler.SetTowerUIEnabled(showTowerUI);
	}

	private void SetTileSelected(Hex tile, bool selected)
	{
		if (tile is null)
		{
			return;
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
			PlaceTowerAtTile(hoveredTile, towerToBePlacedPrefab);
			Destroy(towerToBePlaced.gameObject);
			return true;
		}

		return false;
	}

	public void SetTowerToBePlaced(Tower towerPrefab)
	{
		Debug.Log("Tessst");
		if (towerToBePlaced)
		{
			Destroy(towerToBePlaced);
		}

		towerToBePlaced = Instantiate(towerPrefab);
		towerToBePlacedPrefab = towerPrefab;
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
	}
}