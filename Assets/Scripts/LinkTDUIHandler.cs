using System.Collections.Generic;
using System.Linq;
using HexLibrary;
using Mantis.Engine;
using UnityEngine;
using UnityEngine.UIElements;

public class LinkTDUIHandler : UIHandler
{
	private Label _cashLabel;
	private Label _livesLabel;
	private VisualElement _gameOverScreen;
	private VisualElement _levelFinishedScreen;

	public override void Init()
	{
		base.Init();

		GameState.Get.OnCashChanged += (oldValue, newValue) => UpdateCashLabel(newValue);
		GameState.Get.OnLivesChanged += (oldValue, newValue) => UpdateLivesLabel(newValue);

		GameState.Get.OnGameOver += OnGameOver;

		GameState.Get.OnLevelFinished += OnLevelFinished;

		UpdateCashLabel(GameState.Get.CurrentCash);
		UpdateLivesLabel(GameState.Get.CurrentLives);
	}

	protected override void ReadUIDocument()
	{
		base.ReadUIDocument();
		_cashLabel = _rootVisualElement.Q<Label>("Cash");
		_livesLabel = _rootVisualElement.Q<Label>("Lives");
		_gameOverScreen = _rootVisualElement.Q<VisualElement>("GameOverScreen");
		_levelFinishedScreen = _rootVisualElement.Q<VisualElement>("LevelFinishedScreen");
	}

	private void UpdateCashLabel(int lives)
	{
		_cashLabel.text = "" + lives;
	}

	private void UpdateLivesLabel(int lives)
	{
		_livesLabel.text = lives + "/" + GameState.Get.MaxLives;
	}

	private void OnGameOver()
	{
		_gameOverScreen.style.display = DisplayStyle.Flex;
	}

	private void OnLevelFinished()
	{
		_levelFinishedScreen.style.display = DisplayStyle.Flex;
	}
}