using System.Collections.Generic;
using System.Linq;
using HexLibrary;
using Mantis.Engine;
using UnityEngine;
using UnityEngine.UIElements;

public class LinkTDUIHandler : UIHandler
{
	private VisualElement _gameOverScreen;
	private VisualElement _levelFinishedScreen;

	public override void Init()
	{
		base.Init();

		GameState.Get().OnGameOver += OnGameOver;

		GameState.Get().OnLevelFinished += OnLevelFinished;
	}

	protected override void ReadUIDocument()
	{
		base.ReadUIDocument();
		_gameOverScreen = _rootVisualElement.Q<VisualElement>("GameOverScreen");
		_levelFinishedScreen = _rootVisualElement.Q<VisualElement>("LevelFinishedScreen");
	}
	private void OnGameOver()
	{
		_gameOverScreen.style.display = DisplayStyle.Flex;  //Fix the uis
	}

	private void OnLevelFinished()
	{
		_levelFinishedScreen.style.display = DisplayStyle.Flex; //Fix the uis
		Game.Get.GetUIHandler().SetElementBlockingMouse(_levelFinishedScreen, true);
	}

	private void OpenSettingsMenu()
	{
		GameState.Get().PauseGame();
	}
}