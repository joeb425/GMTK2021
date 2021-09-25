using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Game : MonoBehaviour
{
	public static Game Get;

	public GameState gameState;

	public UIHandler uiHandler;

	public InputHandler input;

	[SerializeField]
	public AudioHandler audio;

	[SerializeField]
	public UIDocument hud;

	[SerializeField]
	public int StartingCash = 10;

	[SerializeField]
	public int MaxLives = 25;

	[SerializeField]
	Vector2Int boardSize = new Vector2Int(11, 11);

	[SerializeField]
	GameBoard board;

	[SerializeField]
	SpawnerHandler spawnerHandler;

	[SerializeField]
	BulletPool bulletPool;

	public EnemyCollection enemies = new EnemyCollection();

	private bool linkAttempt = false;

	[SerializeField]
	private GenericObjectPool[] ObjectPools;

	private void Start()
	{
		Debug.Log("Game Start");
	}

	void Awake()
	{
		Debug.Log("Game Awake");
		board.Initialize();

		// spawnerHandler.board = board;

		gameState = new GameState
		{
			StartingCash = StartingCash, 
			MaxLives = MaxLives, 
			Board = board
		};
		
		gameState.Init();

		bulletPool.Initialize();

		Get = this;

		uiHandler = new UIHandler();
		uiHandler.Init(hud);

		input = new InputHandler();
		input.Init();

		GlobalData.OnGameInit?.Invoke();
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
		spawnerHandler.GameUpdate();
		enemies.GameUpdate();
		board.GameUpdate();
		input.GameUpdate();

		if (UnityEngine.Input.GetKeyDown(KeyCode.Z))
		{
			GameState.Get.LoseLife();
		}
	}

	private void OnDrawGizmos()
	{
		// if (GameState.Get.Board.hoveredTile)
		// {
		// 	Gizmos.color = new Color(1, 1, 0, 0.5f);
		// 	Gizmos.DrawSphere(GameState.Get.Board.hoveredTile.transform.position, .25f);
		// }
		// if (selectedTile != null)
		// {
		// 	Gizmos.color = new Color(1, 1, 0, 0.5f);
		// 	Gizmos.DrawCube(selectedTile.Content.transform.position, new Vector3(1, 1, 1));
		// }
		//
		// if (hoveredTile != null && hoveredTile != selectedTile)
		// {
		// 	Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
		// 	Gizmos.DrawCube(hoveredTile.Content.transform.position, new Vector3(1, 1, 1));
		// }
	}

	// private Tower sourceTower;
	//
	// public void LinkSelect()
	// {
	// 	if (selectedTile.Content.Type == GameTileContentType.Tower)
	// 	{
	// 		if (linkAttempt)
	// 		{
	// 			Tower otherTower = (Tower) selectedTile.Content;
	// 			otherTower.LinkTower(sourceTower);
	// 			sourceTower.LinkTower(otherTower);
	// 			linkAttempt = false;
	// 			return;
	// 		}
	// 		else
	// 		{
	// 			linkAttempt = true;
	// 			sourceTower = (Tower) selectedTile.Content;
	// 		}
	// 	}
	// }

	private void OnDestroy()
	{
		Debug.Log("Game On Destroy");
		input.Disable();
		Get = null;
	}
}