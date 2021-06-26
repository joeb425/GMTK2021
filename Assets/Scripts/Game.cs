using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

//3.3

public class Game : MonoBehaviour
{
	private MouseInput mouseInput;

	public static Game SharedGame;

	public GameState gameState;

	public UIHandler uiHandler;

	[SerializeField]
	Vector2Int boardSize = new Vector2Int(11, 11);

	[SerializeField]
	GameBoard board = default;

	[SerializeField]
	GameTileContentFactory tileContentFactory = default;

	[SerializeField]
	EnemyFactory enemyFactory = default;

	[SerializeField]
	SpawnerHandler spawnerHandler;

	[SerializeField]
	BulletPool bulletPool = default;

	public GameTile selectedTile;
	public GameTileContent selectedTileContent;
	public GameTile hoveredTile;

	public EnemyCollection enemies = new EnemyCollection();
	Ray touchRay;

	private bool linkAttempt = false;
	
	[SerializeField]
	public int maxLives = 20;
	public int currentLives;
	
	private Tower towerToBePlaced;
	private Tower towerToBePlacedPrefab;

	void Awake()
	{
		board.Initialize(boardSize, tileContentFactory);
		board.ShowGrid = true;

		spawnerHandler.board = board;

		bulletPool.Initialize();

		SharedGame = this;

		mouseInput = new MouseInput();
		mouseInput.Enable();

		mouseInput.Mouse.MouseClick.performed += ctx => MouseClick();

		uiHandler.Init();
	}

	void SetTowerToBePlaced(Tower towerPrefab)
	{
		if (towerToBePlaced)
		{
			Destroy(towerToBePlaced);
		}

		towerToBePlaced = Instantiate(towerPrefab);
		towerToBePlacedPrefab = towerPrefab;
		towerToBePlaced.SetGhostTower();
	}
	
	void MouseClick()
	{
		HandleTouch();
		// Debug.Log("Click");
		// Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
		// Debug.Log(mousePosition);
	}

	void OnValidate()
	{
		if (boardSize.x < 2)
		{
			boardSize.x = 2;
		}

		if (boardSize.y < 2)
		{
			boardSize.y = 2;
		}
	}

	void Update()
	{
		Vector2 mousePosition = mouseInput.Mouse.MousePosition.ReadValue<Vector2>();
		touchRay = Camera.main.ScreenPointToRay(new Vector3(mousePosition.x, mousePosition.y, 0.0f));
		
		hoveredTile = board.GetTile(touchRay);

		spawnerHandler.GameUpdate();
		enemies.GameUpdate();
		board.GameUpdate();

		if (towerToBePlaced != null && hoveredTile != null)
		{
			towerToBePlaced.transform.position = hoveredTile.Content.transform.position;
		}
	}

	void HandleAlternativeTouch()
	{
		GameTile tile = board.GetTile(touchRay);
		if (tile != null)
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				board.ToggleDestination(tile);
			}
			else
			{
				board.ToggleSpawnPoint(tile);
			}
		}
	}

	void HandleTouch()
	{
		GameTile tile = board.GetTile(touchRay);
		if (tile != null)
		{
			SelectTile();
		}
	}

	private void SelectTile()
	{
		if (PlaceCurrentTower())
		{
			return;
		}
		
		GameTile oldSelectedTile = selectedTile;
		GameTileContent oldSelectedContent = selectedTileContent;

		selectedTile = board.GetTile(touchRay);
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

		bool showTowerUI = selectedTile.Content.Type == GameTileContentType.Tower;
		uiHandler.SetTowerUIEnabled(showTowerUI);
	}

	private void OnSelectedTileChanged(GameTile oldTile, GameTile newTile)
	{
		SetTileSelected(oldTile, false);
		SetTileSelected(newTile, true);
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

	private void OnDrawGizmos()
	{
		if (selectedTile != null)
		{
			Gizmos.color = new Color(1, 1, 0, 0.5f);
			Gizmos.DrawCube(selectedTile.Content.transform.position, new Vector3(1, 1, 1));
		}

		if (hoveredTile != null && hoveredTile != selectedTile)
		{
			Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
			Gizmos.DrawCube(hoveredTile.Content.transform.position, new Vector3(1, 1, 1));
		}
	}

	private Tower sourceTower;

	public void LinkSelect()
	{
		if (selectedTile.Content.Type == GameTileContentType.Tower)
		{
			if (linkAttempt)
			{
				Tower otherTower = (Tower) selectedTile.Content;
				otherTower.LinkTower(sourceTower);
				sourceTower.LinkTower(otherTower);
				linkAttempt = false;
				return;
			}
			else
			{
				linkAttempt = true;
				sourceTower = (Tower) selectedTile.Content;
			}
		}
	}

	public void SetCash(int cash)
	{
		uiHandler.SetCash(cash);
	}

	public void SetLives(int lives)
	{
		currentLives = currentLives - lives;
		uiHandler.SetLives(lives);
	}

	public bool PlaceCurrentTower()
	{
		// place tower
		if (towerToBePlaced != null)
		{
			board.PlaceTowerAtTile(hoveredTile, towerToBePlacedPrefab);
			Destroy(towerToBePlaced.gameObject);
			return true;
		}

		return false;
	}
}