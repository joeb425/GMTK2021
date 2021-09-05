using System;
using UnityEngine;
using System.Collections.Generic;
using DefaultNamespace.HexGrid;
using HexGrid;
using UnityEditor;

public class GameBoard : MonoBehaviour
{
	[SerializeField]
	Transform ground = default;

	[SerializeField]
	Hex tilePrefab = default;

	[SerializeField]
	Texture2D gridTexture = default;

	[SerializeField]
	TextAsset leveldesign;

	Vector2Int size;

	// Hex[] tiles;

	Queue<Hex> searchFrontier = new Queue<Hex>();

	// GameTileContentFactory contentFactory;

	bool showGrid, showPaths;

	List<Hex> spawnPoints = new List<Hex>();

	// List<HexContent> updatingContent = new List<HexContent>();
	
	private HexGridLayer _hexGridLayer;

	public event System.Action<Hex, Hex> OnSelectedTileChanged;
	public event System.Action<Hex, Hex> OnHoveredTileChanged;

	public bool ShowGrid
	{
		get => showGrid;
		set
		{
			showGrid = value;
			// Material m = ground.GetComponent<MeshRenderer>().material;
			// if (showGrid)
			// {
			// 	m.mainTexture = gridTexture;
			// 	m.SetTextureScale("_MainTex", size);
			// }
			// else
			// {
			// 	m.mainTexture = null;
			// }

//			Material b = ground.GetComponent<MeshRenderer>().material;
//			if (ShowGrid)
//			{
//				b.mainTexture = gridTexture;
//				b.SetTextureScale("_MainTex", size);
//			}
//			else
//			{
//				b.mainTexture = null;
//			}
		}
	}

	// public bool ShowPaths
	// {
	// 	get => showPaths;
	// 	set
	// 	{
	// 		showPaths = value;
	// 		if (showPaths)
	// 		{
	// 			foreach (Hex tile in tiles)
	// 			{
	// 				tile.ShowPath();
	// 			}
	// 		}
	// 		else
	// 		{
	// 			foreach (Hex tile in tiles)
	// 			{
	// 				tile.HidePath();
	// 			}
	// 		}
	// 	}
	// }
	[System.Serializable]
	public class LevelData
	{
		public int tier;
		public List<LevelRow> layout;
	}
	[System.Serializable]
	public class LevelRow
	{
		public List<int> row;
	}
	[System.Serializable]
	public class Levels
	{
		public LevelData[] level;
	}
	public void Initialize(
		Vector2Int size, GameTileContentFactory contentFactory
	)
	{
		// int spawn_loc = 1;
		// int end_loc = 20;
		//
		// Debug.Log("Try load level " + GlobalData.CurrentLevel);
		// TextAsset levelToLoad = GlobalData.CurrentLevel < 0 ? leveldesign : GlobalData.GetLevelData().levels[GlobalData.CurrentLevel];
		//
		// Levels levelsLoaded = JsonUtility.FromJson<Levels>(levelToLoad.text);
		//
		// foreach (LevelData level in levelsLoaded.level)
		// {
		// 	size.x = level.layout[0].row.Count;
		// 	size.y = level.layout.Count;
		// 	Debug.Log("Found Level: " + level.tier + " - " + level.layout.Count + "." + level.layout[1].row.Count + ".");
		// }
		//
		// this.size = size;
		// this.contentFactory = contentFactory;
		// ground.localScale = new Vector3(size.x, size.y, 1f);
		// //build.localScale = new Vector3(size.x, size.y, 1f);
		//
		// Vector2 offset = new Vector2(
		// 	(size.x - 1) * 0.5f, (size.y - 1) * 0.5f
		// );
		// tiles = new Hex[size.x * size.y];
		// for (int i = 0, y = 0; y < size.y; y++)
		// {
		// 	for (int x = 0; x < size.x; x++, i++)
		// 	{
		//
		// 		Hex tile = tiles[i] = Instantiate(tilePrefab);
		// 		tile.transform.SetParent(transform, false);
		// 		tile.transform.localPosition = new Vector3(
		// 			x - offset.x, 0f, y - offset.y
		// 		);
		//
		// 		if (x > 0)
		// 		{
		// 			Hex.MakeEastWestNeighbors(tile, tiles[i - 1]);
		// 		}
		//
		// 		if (y > 0)
		// 		{
		// 			Hex.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);
		// 		}
		//
		// 		tile.IsAlternative = (x & 1) == 0;
		// 		if ((y & 1) == 0)
		// 		{
		// 			tile.IsAlternative = !tile.IsAlternative;
		// 		}
		//
		// 		tile.Content = contentFactory.Get(HexContentType.Path);
		//
		// 		Debug.Log(x + ", " + y);
		// 		int TileType = levelsLoaded.level[0].layout[y].row[x];
		// 		switch (TileType)
		// 		{
		// 			case 0: //trees n shit, for looks
		// 				break;
		// 			case 1: //For building on no monsters
		// 				ToggleBuildSpot(tiles[i]);
		// 				break;
		// 			case 2: //For monster track
		// 				break;
		// 			case 3: //For spawn
		// 				spawn_loc = i;
		// 				break;
		// 			case 4: //For end
		// 				end_loc = i;
		// 				break;
		// 		}
		// 	}
		// }
		//
		// ToggleDestination(tiles[end_loc]);
		// ToggleSpawnPoint(tiles[spawn_loc]);
		// FindPaths();

		HexGridLayer hexGridLayerPrefab = AssetDatabase.LoadAssetAtPath<HexGridLayer>("Assets/Prefabs/Hex/HexGrid.prefab");
		_hexGridLayer = Instantiate(hexGridLayerPrefab);
		
		OnSelectedTileChanged += SelectedTileChanged;
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
		// return _hexGridLayer.GetTile(_hexGridLayer.GetHexUnderRay(ray));
		
		// if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1 << 7))
		// {
		// 	int x = (int) (hit.point.x + size.x * 0.5f);
		// 	int y = (int) (hit.point.z + size.y * 0.5f);
		// 	if (x >= 0 && x < size.x && y >= 0 && y < size.y)
		// 	{
		// 		return tiles[x + y * size.x];
		// 	}
		// }

		return null;
	}

	bool FindPaths()
	{
		// foreach (Hex tile in tiles)
		// {
		// 	if (tile.Content.Type == HexContentType.Destination)
		// 	{
		// 		tile.BecomeDestination();
		// 		searchFrontier.Enqueue(tile);
		// 	}
		// 	else
		// 	{
		// 		tile.ClearPath();
		// 	}
		// }
		//
		// if (searchFrontier.Count == 0)
		// {
		// 	return false;
		// }
		//
		// while (searchFrontier.Count > 0)
		// {
		// 	Hex tile = searchFrontier.Dequeue();
		// 	if (tile != null)
		// 	{
		// 		if (tile.IsAlternative)
		// 		{
		// 			searchFrontier.Enqueue(tile.GrowPathNorth());
		// 			searchFrontier.Enqueue(tile.GrowPathSouth());
		// 			searchFrontier.Enqueue(tile.GrowPathEast());
		// 			searchFrontier.Enqueue(tile.GrowPathWest());
		// 		}
		// 		else
		// 		{
		// 			searchFrontier.Enqueue(tile.GrowPathWest());
		// 			searchFrontier.Enqueue(tile.GrowPathEast());
		// 			searchFrontier.Enqueue(tile.GrowPathSouth());
		// 			searchFrontier.Enqueue(tile.GrowPathNorth());
		// 		}
		// 	}
		// }
		//
		// foreach (Hex tile in tiles)
		// {
		// 	if (!tile.HasPath)
		// 	{
		// 		return false;
		// 	}
		// }
		//
		// if (showPaths)
		// {
		// 	foreach (Hex tile in tiles)
		// 	{
		// 		tile.ShowPath();
		// 	}
		// }

		return true;
	}

	public void GameUpdate()
	{
		// for (int i = 0; i < updatingContent.Count; i++)
		// {
		// 	updatingContent[i].GameUpdate();
		// }

		hoveredTile = GetTile(InputHandler.Get.touchRay);

		// if (towerToBePlaced != null && hoveredTile != null)
		// {
		// 	towerToBePlaced.transform.position = hoveredTile.Content.transform.position;
		// }
	}

	public void PlaceTowerAtTile(Hex tile, Tower tower)
	{
		if (tile == null || tower == null)
			return;

		ToggleTower(tile, tower);
	}

	public void PlaceTower(Tower tower)
	{
		if (selectedTile == null)
			return;

		ToggleTower(selectedTile, tower);
		
		// TODO: this stupid
		Game.SharedGame.uiHandler.SetBuildMenuEnabled(false);
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

	public Hex selectedTile;
	// public HexContent selectedTileContent;
	public Hex hoveredTile;

	private Tower towerToBePlaced;
	private Tower towerToBePlacedPrefab;

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
		
		Debug.Log("Select tile?");

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
		// if (selectedTile != null)
		// {
		// 	Gizmos.DrawSphere(selectedTile.spawnedTile.transform.position, 1);
		// }
	}
}