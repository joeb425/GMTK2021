using System;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

//3.3

public class Game : MonoBehaviour
{
	private MouseInput mouseInput;

	public static Game SharedGame;

	[SerializeField]
	Vector2Int boardSize = new Vector2Int(11, 11);

	[SerializeField]
	GameBoard board = default;

	[SerializeField]
	GameTileContentFactory tileContentFactory = default;

	[SerializeField]
	EnemyFactory enemyFactory = default;

	[SerializeField]
	SpawnerHandler spawnerHandler = default;

	[SerializeField]
	BulletPool bulletPool = default;

	[SerializeField]
	public Canvas BuildMenu = default;

	[SerializeField]
	public Canvas TowerUI = default;

	[SerializeField, Range(0.1f, 10f)]
	float spawnSpeed = 1f;

	float spawnProgress;

	public GameTile selectedTile;
	public GameTileContent selectedTileContent;
	public GameTile hoveredTile;

	private bool IsGUIEnabled = false;

	public EnemyCollection enemies = new EnemyCollection();
	Ray touchRay;

	private bool linkAttempt = false;

	[SerializeField]
	public UIDocument uiDocument;

	public Label cashLabel;
	public Label livesLabel;

	void Awake()
	{
		board.Initialize(boardSize, tileContentFactory);
		board.ShowGrid = true;

		bulletPool.Initialize();

		SharedGame = this;

		SetBuildMenuEnabled(false);

		mouseInput = new MouseInput();
		mouseInput.Enable();

		mouseInput.Mouse.MouseClick.performed += ctx => MouseClick();

		BindButton(board.PlaceBasicTower, board.BasicTowerPrefab, "SpawnBasicBtn", "Basic");
		BindButton(board.PlaceDoubleTower, board.DoubleTowerPrefab, "SpawnDoubleBtn", "Double");
		BindButton(board.PlaceRocketTower, board.RocketTowerPrefab, "SpawnRocketBtn", "Rocket");
		BindButton(board.PlaceSniperTower, board.SniperTowerPrefab, "SpawnSniperBtn", "Sniper");
		BindButton(board.PlaceSMGTower, board.SMGTowerPrefab, "SpawnSMGBtn", "SMG");

		var rootVisualElement = uiDocument.rootVisualElement;
		cashLabel = rootVisualElement.Q<Label>("Cash");
		livesLabel = rootVisualElement.Q<Label>("Lives");
		
		SetCash(board.cash);
	}

	void BindButton(Action action, Tower towerPrefab, string btnName, string btnText)
	{
		var rootVisualElement = uiDocument.rootVisualElement;
		var spawnBasicTower = rootVisualElement.Q(btnName);
		spawnBasicTower.RegisterCallback<ClickEvent>(ev => action());
		spawnBasicTower.Q<Label>("Cost").text = "" + towerPrefab.Cost;
		spawnBasicTower.Q<Button>("Button").text = btnText;
	}
	
	void MouseClick()
	{
		HandleTouch();
		Debug.Log("Click");
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

		spawnProgress += spawnSpeed * Time.deltaTime;
		//		while (spawnProgress >= 1f)
		//		{
		//			spawnProgress -= 1f;
		//			SpawnEnemy();
		//		}
		spawnerHandler.GameUpdate(board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount)));
		enemies.GameUpdate();
		board.GameUpdate();
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

		spawnProgress += spawnSpeed * Time.deltaTime;
//		while (spawnProgress >= 1f)
//		{
//			spawnProgress -= 1f;
//			SpawnEnemy();
//		}
	}

//	void SpawnEnemy()
//	{
//		GameTile spawnPoint =
//			board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount));
//		Enemy enemy = enemyFactory.Get();
//		enemy.SpawnOn(spawnPoint);
//		enemies.Add(enemy);
//	}

	void HandleTouch()
	{
		if (IsGUIEnabled)
		{
			return;
		}

		GameTile tile = board.GetTile(touchRay);
		if (tile != null)
		{
			// if (Input.GetKey(KeyCode.LeftShift))
			// {
			// 	board.ToggleTower(tile);
			// }
			// else
			{
				SelectTile();
			}
		}
	}

	private void SelectTile()
	{
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

		bool showBuildMenu = selectedTile.Content.Type == GameTileContentType.Build;
		SetBuildMenuEnabled(showBuildMenu);

		bool showTowerUI = selectedTile.Content.Type == GameTileContentType.Tower;
		SetTowerUIEnabled(showTowerUI);
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

	public void SetBuildMenuEnabled(bool menuEnabled)
	{
		CanvasGroup canvasGroup = BuildMenu.GetComponent<CanvasGroup>();
		canvasGroup.alpha = menuEnabled ? 1.0f : 0.0f;
		canvasGroup.interactable = menuEnabled;
		IsGUIEnabled = menuEnabled;
	}

	public void SetTowerUIEnabled(bool menuEnabled)
	{
		CanvasGroup canvasGroup = TowerUI.GetComponent<CanvasGroup>();
		canvasGroup.alpha = menuEnabled ? 1.0f : 0.0f;
		canvasGroup.interactable = true;
		// IsGUIEnabled = menuEnabled;
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
		cashLabel.text = "" + cash;
	}

	public void SetLives(int lives)
	{
		livesLabel.text = "" + lives;
	}
}