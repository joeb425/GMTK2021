using UnityEngine;
using UnityEngine.UI;

//3.3

public class Game : MonoBehaviour
{
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
	Canvas BuildMenu = default;

	[SerializeField, Range(0.1f, 10f)]
	float spawnSpeed = 1f;

	float spawnProgress;

	private GameTile selectedTile;

	private GameTile hoveredTile;

	private bool IsGUIEnabled = false;

	EnemyCollection enemies = new EnemyCollection();
	Ray TouchRay => Camera.main.ScreenPointToRay(Input.mousePosition);

	void Awake()
	{
		board.Initialize(boardSize, tileContentFactory);
		board.ShowGrid = true;

		bulletPool.Initialize();
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

		if (Input.GetMouseButtonDown(0))
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
			if (Input.GetKey(KeyCode.LeftShift))
			{
				board.ToggleTower(tile);
			}
			else
			{
				SelectTile();
			}
		}
	}

	private void SelectTile()
	{
		selectedTile = board.GetTile(TouchRay);
		if (selectedTile.Content.Type == GameTileContentType.Build)
		{
			CanvasGroup canvasGroup = BuildMenu.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 1.0f;
			SetBuildMenuEnabled(true);
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
			Gizmos.color = new Color(1, 0, 0, 0.5f);
			Gizmos.DrawCube(hoveredTile.Content.transform.position, new Vector3(1, 1, 1));
		}
	}

	public void PlaceTower()
	{
		if (selectedTile != null)
		{
			board.ToggleTower(selectedTile);
			CanvasGroup canvasGroup = BuildMenu.GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0.0f;
			SetBuildMenuEnabled(false);
		}
	}

	public void SetBuildMenuEnabled(bool Enabled)
	{
		IsGUIEnabled = Enabled;
		Button[] buttons = BuildMenu.GetComponents<Button>();
		foreach (Button button in buttons)
		{
			button.interactable = Enabled;
		}
	}
}