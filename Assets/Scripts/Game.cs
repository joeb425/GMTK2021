using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//3.3

public class Game : MonoBehaviour
	, IPointerClickHandler
{
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
	BulletPool bulletPool = default;

	[SerializeField]
	public Canvas BuildMenu = default;

	[SerializeField]
	public Canvas TowerUI = default;

	[SerializeField, Range(0.1f, 10f)]
	float spawnSpeed = 1f;

	float spawnProgress;

	public GameTile selectedTile;
	public GameTile hoveredTile;

	private bool IsGUIEnabled = false;

	EnemyCollection enemies = new EnemyCollection();
	Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

	private bool linkAttempt = false;

	void Awake()
	{
		board.Initialize(boardSize, tileContentFactory);
		board.ShowGrid = true;

		bulletPool.Initialize();

		SharedGame = this;

		SetBuildMenuEnabled(false);
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
		hoveredTile = board.GetTile(TouchRay);

		if (Input.GetButtonDown("Click"))
		{
			HandleTouch();
		}
		else if (Input.GetMouseButtonDown(1))
		{
			HandleAlternativeTouch();
		}

		if (Input.GetKeyDown(KeyCode.V))
		{
			board.ShowPaths = !board.ShowPaths;
		}

		if (Input.GetKeyDown(KeyCode.G))
		{
			board.ShowGrid = !board.ShowGrid;
		}

		spawnProgress += spawnSpeed * Time.deltaTime;
		while (spawnProgress >= 1f)
		{
			spawnProgress -= 1f;
			SpawnEnemy();
		}

		enemies.GameUpdate();
		board.GameUpdate();
	}

	void HandleAlternativeTouch()
	{
		GameTile tile = board.GetTile(TouchRay);
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
		while (spawnProgress >= 1f)
		{
			spawnProgress -= 1f;
			SpawnEnemy();
		}
	}

	void SpawnEnemy()
	{
		GameTile spawnPoint =
			board.GetSpawnPoint(Random.Range(0, board.SpawnPointCount));
		Enemy enemy = enemyFactory.Get();
		enemy.SpawnOn(spawnPoint);
		enemies.Add(enemy);
	}

	void HandleTouch()
	{
		if (IsGUIEnabled)
		{
			return;
		}

		GameTile tile = board.GetTile(TouchRay);
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
		selectedTile = board.GetTile(TouchRay);
		
		bool showBuildMenu = selectedTile.Content.Type == GameTileContentType.Build;
		SetBuildMenuEnabled(showBuildMenu);

		bool showTowerUI = selectedTile.Content.Type == GameTileContentType.Tower;
		SetTowerUIEnabled(showTowerUI);

		if (selectedTile.Content.Type == GameTileContentType.Tower)
		{
			Tower tower = selectedTile.Content as Tower;
			Renderer shaders = tower.GetComponentInChildren<Renderer>();
			shaders.sharedMaterial.SetFloat("_OutlineWidth", 1.05f);
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
				((Tower)selectedTile.Content).LinkTower(sourceTower);
				linkAttempt = false;
				return;
			}
			else
			{
				linkAttempt = true;
				sourceTower = (Tower)selectedTile.Content;
			}
		}
	}
	public void OnPointerClick(PointerEventData eventData)
	{
		Debug.Log("Pointer click");
	}
}