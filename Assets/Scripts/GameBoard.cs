using System;
using UnityEngine;
using System.Collections.Generic;

public class GameBoard : MonoBehaviour
{
	[SerializeField]
	Transform ground = default;

	[SerializeField]
	GameTile tilePrefab = default;

	[SerializeField]
	Texture2D gridTexture = default;

	[SerializeField]
	public Tower BasicTowerPrefab = default;

	[SerializeField]
	public Tower DoubleTowerPrefab = default;

	[SerializeField]
	public Tower SMGTowerPrefab = default;

	[SerializeField]
	public Tower SniperTowerPrefab = default;

	[SerializeField]
	public Tower RocketTowerPrefab = default;

	[SerializeField]
	TextAsset leveldesign;

	Vector2Int size;

	GameTile[] tiles;

	Queue<GameTile> searchFrontier = new Queue<GameTile>();

	GameTileContentFactory contentFactory;

	bool showGrid, showPaths;

	List<GameTile> spawnPoints = new List<GameTile>();

	List<GameTileContent> updatingContent = new List<GameTileContent>();

	public event System.Action<GameTile, GameTile> OnSelectedTileChanged;
	public event System.Action<GameTile, GameTile> OnHoveredTileChanged;

	public int cash = 25;

	public bool ShowGrid
	{
		get => showGrid;
		set
		{
			showGrid = value;
			Material m = ground.GetComponent<MeshRenderer>().material;
			if (showGrid)
			{
				m.mainTexture = gridTexture;
				m.SetTextureScale("_MainTex", size);
			}
			else
			{
				m.mainTexture = null;
			}

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

	public bool ShowPaths
	{
		get => showPaths;
		set
		{
			showPaths = value;
			if (showPaths)
			{
				foreach (GameTile tile in tiles)
				{
					tile.ShowPath();
				}
			}
			else
			{
				foreach (GameTile tile in tiles)
				{
					tile.HidePath();
				}
			}
		}
	}
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
		int spawn_loc = 1;
		int end_loc = 20;
		Levels LevelsLoaded = JsonUtility.FromJson<Levels>(leveldesign.text);
		foreach (LevelData level in LevelsLoaded.level)
		{

			size.x = level.layout.Count;
			size.y = level.layout[0].row.Count;
			//Debug.Log("Found Level: " + level.tier + " - " + level.layout.Count + "." + level.layout[1].row.Count + ".");
		}

		this.size = size;
		this.contentFactory = contentFactory;
		ground.localScale = new Vector3(size.x, size.y, 1f);
		//build.localScale = new Vector3(size.x, size.y, 1f);

		Vector2 offset = new Vector2(
			(size.x - 1) * 0.5f, (size.y - 1) * 0.5f
		);
		tiles = new GameTile[size.x * size.y];
		for (int i = 0, y = 0; y < size.y; y++)
		{
			for (int x = 0; x < size.x; x++, i++)
			{

				GameTile tile = tiles[i] = Instantiate(tilePrefab);
				tile.transform.SetParent(transform, false);
				tile.transform.localPosition = new Vector3(
					x - offset.x, 0f, y - offset.y
				);

				if (x > 0)
				{
					GameTile.MakeEastWestNeighbors(tile, tiles[i - 1]);
				}

				if (y > 0)
				{
					GameTile.MakeNorthSouthNeighbors(tile, tiles[i - size.x]);
				}

				tile.IsAlternative = (x & 1) == 0;
				if ((y & 1) == 0)
				{
					tile.IsAlternative = !tile.IsAlternative;
				}

				tile.Content = contentFactory.Get(GameTileContentType.Path);

				int TileType = LevelsLoaded.level[0].layout[y].row[x];
				switch (TileType)
				{
					case 0: //trees n shit, for looks
						break;
					case 1: //For building on no monsters
						ToggleBuildSpot(tiles[i]);
						break;
					case 2: //For monster track
						break;
					case 3: //For spawn
						spawn_loc = i;
						break;
					case 4: //For end
						end_loc = i;
						break;
				}
			}
		}

		ToggleDestination(tiles[end_loc]);
		ToggleSpawnPoint(tiles[spawn_loc]);
		FindPaths();

		OnSelectedTileChanged += SelectedTileChanged;
	}

	public void ToggleSpawnPoint(GameTile tile)
	{
		if (tile.Content.Type == GameTileContentType.SpawnPoint)
		{
			if (spawnPoints.Count > 1)
			{
				spawnPoints.Remove(tile);
				tile.Content = contentFactory.Get(GameTileContentType.Path);
			}
		}
		else if (tile.Content.Type == GameTileContentType.Path)
		{
			tile.Content = contentFactory.Get(GameTileContentType.SpawnPoint);
			spawnPoints.Add(tile);
		}
	}

	public GameTile GetSpawnPoint(int index)
	{
		return spawnPoints[index];
	}

	public int SpawnPointCount => spawnPoints.Count;

	public void ToggleDestination(GameTile tile)
	{
		if (tile.Content.Type == GameTileContentType.Destination)
		{
			tile.Content = contentFactory.Get(GameTileContentType.Path);
			if (!FindPaths())
			{
				tile.Content =
					contentFactory.Get(GameTileContentType.Destination);
				FindPaths();
			}
		}
		else if (tile.Content.Type == GameTileContentType.Path)
		{
			tile.Content = contentFactory.Get(GameTileContentType.Destination);
			FindPaths();
		}
	}
	public void ToggleBuildSpot(GameTile tile)
	{
		tile.Content = contentFactory.Get(GameTileContentType.Build);
	}
	public void ToggleTower(GameTile tile, Tower towerPrefab)
	{
		if (tile.Content.Type == GameTileContentType.Tower)
		{
			updatingContent.Remove(tile.Content);
			tile.Content = contentFactory.Get(GameTileContentType.Build);
			FindPaths();
		}
		else if (tile.Content.Type == GameTileContentType.Wall)
		{
			tile.Content = contentFactory.Get(GameTileContentType.Tower);
			updatingContent.Add(tile.Content);
		}
		else if (tile.Content.Type ==  GameTileContentType.Build)
		{
			if (BuyTower(towerPrefab))
			{
				tile.Content = contentFactory.Get(towerPrefab);
				updatingContent.Add(tile.Content);

				// update hud
				GameState.Get.SetCash(cash);
			}
		}
	}

	public void ToggleWall(GameTile tile)
	{
		if (tile.Content.Type == GameTileContentType.Wall)
		{
			tile.Content = contentFactory.Get(GameTileContentType.Path);
			FindPaths();
		}
		else if (tile.Content.Type == GameTileContentType.Path)
		{
			tile.Content = contentFactory.Get(GameTileContentType.Wall);
			if (!FindPaths())
			{
				tile.Content = contentFactory.Get(GameTileContentType.Path);
				FindPaths();
			}
		}
	}

	public GameTile GetTile(Ray ray)
	{
		if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1 << 7))
		{
			int x = (int) (hit.point.x + size.x * 0.5f);
			int y = (int) (hit.point.z + size.y * 0.5f);
			if (x >= 0 && x < size.x && y >= 0 && y < size.y)
			{
				return tiles[x + y * size.x];
			}
		}

		return null;
	}

	bool FindPaths()
	{
		foreach (GameTile tile in tiles)
		{
			if (tile.Content.Type == GameTileContentType.Destination)
			{
				tile.BecomeDestination();
				searchFrontier.Enqueue(tile);
			}
			else
			{
				tile.ClearPath();
			}
		}

		if (searchFrontier.Count == 0)
		{
			return false;
		}

		while (searchFrontier.Count > 0)
		{
			GameTile tile = searchFrontier.Dequeue();
			if (tile != null)
			{
				if (tile.IsAlternative)
				{
					searchFrontier.Enqueue(tile.GrowPathNorth());
					searchFrontier.Enqueue(tile.GrowPathSouth());
					searchFrontier.Enqueue(tile.GrowPathEast());
					searchFrontier.Enqueue(tile.GrowPathWest());
				}
				else
				{
					searchFrontier.Enqueue(tile.GrowPathWest());
					searchFrontier.Enqueue(tile.GrowPathEast());
					searchFrontier.Enqueue(tile.GrowPathSouth());
					searchFrontier.Enqueue(tile.GrowPathNorth());
				}
			}
		}

		foreach (GameTile tile in tiles)
		{
			if (!tile.HasPath)
			{
				return false;
			}
		}

		if (showPaths)
		{
			foreach (GameTile tile in tiles)
			{
				tile.ShowPath();
			}
		}

		return true;
	}

	public void GameUpdate()
	{
		for (int i = 0; i < updatingContent.Count; i++)
		{
			updatingContent[i].GameUpdate();
		}

		hoveredTile = GetTile(InputHandler.Get.touchRay);

		if (towerToBePlaced != null && hoveredTile != null)
		{
			towerToBePlaced.transform.position = hoveredTile.Content.transform.position;
		}
	}

	public void PlaceTowerAtTile(GameTile tile, Tower tower)
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
	
	public void PlaceBasicTower()
	{
		PlaceTower(BasicTowerPrefab);
	}

	public void PlaceDoubleTower()
	{
		PlaceTower(DoubleTowerPrefab);
	}

	public void PlaceSMGTower()
	{
		PlaceTower(SMGTowerPrefab);
	}

	public void PlaceSniperTower()
	{
		PlaceTower(SniperTowerPrefab);
	}

	public void PlaceRocketTower()
	{
		PlaceTower(RocketTowerPrefab);
	}

	public bool BuyTower(Tower tower)
	{
		if (cash >= tower.Cost)
		{
			cash -= tower.Cost;
			return true;
		}

		return false;
	}

	public GameTile selectedTile;
	public GameTileContent selectedTileContent;
	public GameTile hoveredTile;

	private Tower towerToBePlaced;
	private Tower towerToBePlacedPrefab;

	private void SelectedTileChanged(GameTile oldTile, GameTile newTile)
	{
		SetTileSelected(oldTile, false);
		SetTileSelected(newTile, true);
	}

	public void SelectTile(GameTile newSelectedTile)
	{
		if (PlaceCurrentTower())
		{
			return;
		}

		GameTile oldSelectedTile = selectedTile;
		GameTileContent oldSelectedContent = selectedTileContent;

		selectedTile = newSelectedTile; //GetTile(touchRay);
		selectedTileContent = selectedTile.Content;

		if (oldSelectedTile != selectedTile)
		{
			OnSelectedTileChanged(oldSelectedTile, selectedTile);
		}
		// content may have changed as well
		else if (oldSelectedTile != null && selectedTile != null &&
		         oldSelectedContent != selectedTileContent)
		{
			OnSelectedTileChanged(oldSelectedTile, selectedTile);
		}

		// bool showBuildMenu = selectedTile.Content.Type == GameTileContentType.Build;
		// SetBuildMenuEnabled(showBuildMenu);

		// bool showTowerUI = selectedTile.Content.Type == GameTileContentType.Tower;
		// uiHandler.SetTowerUIEnabled(showTowerUI);
	}

	private void SetTileSelected(GameTile tile, bool selected)
	{
		if (tile is null)
		{
			return;
		}

		if (tile.Content.Type == GameTileContentType.Tower)
		{
			MeshRenderer[] renderers = tile.Content.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer renderer in renderers)
			{
				if (renderer != null)
				{
					renderer.material.SetFloat("_OutlineWidth", selected ? 1.05f : 1.0f);
				}
			}

			Tower tower = tile.Content as Tower;
			tower.OnSelected(selected);
		}
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
		if (towerToBePlaced)
		{
			Destroy(towerToBePlaced);
		}

		towerToBePlaced = Instantiate(towerPrefab);
		towerToBePlacedPrefab = towerPrefab;
		towerToBePlaced.SetGhostTower();
	}
}