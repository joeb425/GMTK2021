using System;
using Misc;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class Game : MonoBehaviour
{
	public static Game Get;

	public GameState gameState;

	public UIHandler uiHandler;

	public InputHandler input;

	public TileHighlighter tileHighlighter;

	[SerializeField]
	public AudioHandler audioHandler;

	[SerializeField]
	public UIDocument hud;

	[SerializeField]
	public int StartingCash = 10;

	[SerializeField]
	public int MaxLives = 25;

	[SerializeField]
	GameBoard board;

	[SerializeField]
	SpawnerHandler spawnerHandler;

	[SerializeField]
	BulletPool bulletPool;

	public EnemyCollection enemies = new EnemyCollection();

	public System.Action OnGameDestroyed;

	void Awake()
	{
		Get = this;

		gameState = new GameState
		{
			StartingCash = StartingCash, 
			MaxLives = MaxLives, 
			Board = board
		};

		gameState.Init();

		board.Initialize();

		bulletPool.Initialize();

		uiHandler = new UIHandler();
		uiHandler.Init(hud);

		input = new InputHandler();
		input.Init();

		tileHighlighter = Instantiate(GlobalData.GetAssetBindings().gameAssets.tileHighlighter);

		GlobalData.OnGameInit?.Invoke();
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

	private void OnDestroy()
	{
		Debug.Log("Game On Destroy");
		input.Disable();
		Get = null;

		GlobalData.Clear();

		OnGameDestroyed?.Invoke();
	}
}