using System;
using System.Collections;
using System.Collections.Generic;
using Mantis.Engine;
using Mantis.Utils;
using Misc;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class Game : BaseGame
{
	public static Game Get;

	public InputHandler input;

	public TileHighlighter tileHighlighter;

	[SerializeField]
	public SpawnerHandler spawnerHandler;

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
		input.GameUpdate();

		if (Input.GetKeyDown(KeyCode.Z))
		{
			NextLevel();
		}
	}

	protected override void GameDestroyed()
	{
		input.Disable();
		Get = null;
	}

	public bool NextLevel()
	{
		if (GlobalData.GetAssetBindings().gameData.IsLastLevel(GlobalData.CurrentLevel))
		{
			return false;
		}

		GlobalData.CurrentLevel += 1;
		RestartGame();
		return true;
	}

	public void RestartGame()
	{
		SceneManager.LoadScene("Assets/Scenes/Game.unity", LoadSceneMode.Single);
	}

	public void GoToMainMenu()
	{
		SceneManager.LoadScene("Assets/Scenes/MainMenu.unity", LoadSceneMode.Single);
	}
}