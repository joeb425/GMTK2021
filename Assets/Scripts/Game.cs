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

	public InputHandler input;

	public TileHighlighter tileHighlighter;

	[SerializeField]
	SpawnerHandler spawnerHandler;

	public EnemyCollection enemies = new EnemyCollection();

	protected override void PreInit()
	{
		Get = this;
	}

	protected override void Init()
	{
		input = new InputHandler();
		input.Init();

		tileHighlighter = Instantiate(GlobalData.GetAssetBindings().gameAssets.tileHighlighter);
	}

	protected override void GameUpdate()
	{
		spawnerHandler.GameUpdate();
		enemies.GameUpdate();
		input.GameUpdate();

		if (Input.GetKeyDown(KeyCode.Z))
		{
			GameState.Get().LoseLife();
		}
	}

	protected override void GameDestroyed()
	{
		input.Disable();
		Get = null;
	}
}