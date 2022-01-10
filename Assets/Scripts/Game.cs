using System;
using System.Collections;
using System.Collections.Generic;
using Mantis.Engine;
using Mantis.GameplayTags;
using Mantis.Utils;
using Misc;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
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

	[SerializeField]
	public GameplayTag myTagA;

	[SerializeField]
	public GameplayTag myTagB;

	protected override void PreInit()
	{
		Application.targetFrameRate = 120;
		AsyncOperationHandle<GameplayTag> op = Addressables.LoadAssetAsync<GameplayTag>("Assets/Data/Tags/Hex.asset");
		GameplayTag loadTag = op.WaitForCompletion();
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

		// if (Input.GetKeyDown(KeyCode.Z))
		// {
		// 	GlobalData.GetAssetBindings().playerSaveManager.FinishLevel(0);
		// 	// NextLevel();
		//
		// }
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
		Addressables.LoadSceneAsync("Assets/Scenes/Game.unity", LoadSceneMode.Single);
	}

	public void GoToMainMenu()
	{
		Addressables.LoadSceneAsync("Assets/Scenes/MainMenu.unity", LoadSceneMode.Single);
	}
}