using System;
using System.Collections;
using System.Collections.Generic;
using Mantis.Engine;
using Mantis.Utils;
using Misc;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class Game : BaseGame
{
	public static Game Get;

	// public GameState gameState;

	// public UIHandler uiHandler;

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

	public EnemyCollection enemies = new EnemyCollection();

	// public System.Action OnGameDestroyed;

	protected override void PreInit()
	{
		Get = this;
	}

	protected override void Init()
	{
		board.Initialize();

		input = new InputHandler();
		input.Init();

		tileHighlighter = Instantiate(GlobalData.GetAssetBindings().gameAssets.tileHighlighter);

		GlobalData.OnGameInit?.Invoke();
	}

	// void Awake()
	// {
	//
	// 	// gameState = new GameState
	// 	// {
	// 	// 	StartingCash = StartingCash, 
	// 	// 	MaxLives = MaxLives, 
	// 	// 	Board = board
	// 	// };
	//
	// 	// gameState.Init();
	//
	// 	// uiHandler = new UIHandler();
	// 	// uiHandler.Init(hud);
	// }

	// void Update()
	// {
	//
	// }

	protected override void GameUpdate()
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

	// private new void OnDestroy()
	// {
	// 	// Debug.Log("Game On Destroy");
	// 	// input.Disable();
	// 	// Get = null;
	//
	// 	GlobalData.Clear();
	//
	// 	OnGameDestroyed?.Invoke();
	// }

	protected override void GameDestroyed()
	{
		Debug.Log("Game On Destroy");
		input.Disable();
		Get = null;

		GlobalData.Clear();

		OnGameDestroyed?.Invoke();
	}
}