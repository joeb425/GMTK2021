using System;
using UnityEngine;
public class Game : MonoBehaviour
{
	public static Game SharedGame;

	[SerializeField]
	public GameState gameState;

	public UIHandler uiHandler;

	public InputHandler inputHandler;

	[SerializeField]
	public int StartingCash = 10;

	[SerializeField]
	public int MaxLives = 25;

	[SerializeField]
	Vector2Int boardSize = new Vector2Int(11, 11);

	// Make these all singletons?
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
		board.Initialize(boardSize, tileContentFactory);

		// spawnerHandler.board = board;

		gameState = new GameState
		{
			StartingCash = StartingCash, 
			MaxLives = MaxLives, 
			Board = board
		};
		
		gameState.Init();

		bulletPool.Initialize();

		SharedGame = this;
		
		uiHandler.Init();

		inputHandler = new InputHandler();
		inputHandler.Init();
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
		inputHandler.GameUpdate();

		if (Input.GetKeyDown(KeyCode.Z))
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
}