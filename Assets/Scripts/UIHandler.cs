using System.Collections.Generic;
using System.Linq;
using HexLibrary;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler
{
	private UIDocument hud;
	private VisualElement _rootVisualElement;
	private Label _cashLabel;
	private Label _livesLabel;
	private VisualElement _gameOverScreen;
	private VisualElement _levelFinishedScreen;

	private List<VisualElement> _elementsBlockingMouse = new List<VisualElement>();

	public void Init(UIDocument hud)
	{
		this.hud = hud;
		ReadUIDocument();

		GameState.Get.OnCashChanged += (oldValue, newValue) => UpdateCashLabel(newValue);
		GameState.Get.OnLivesChanged += (oldValue, newValue) => UpdateLivesLabel(newValue);

		GameState.Get.OnGameOver += OnGameOver;

		GameState.Get.OnLevelFinished += OnLevelFinished;

		UpdateCashLabel(GameState.Get.CurrentCash);
		UpdateLivesLabel(GameState.Get.CurrentLives);
	}

	public void ReadUIDocument()
	{
		_rootVisualElement = hud.rootVisualElement;
		_cashLabel = _rootVisualElement.Q<Label>("Cash");
		_livesLabel = _rootVisualElement.Q<Label>("Lives");
		_gameOverScreen = _rootVisualElement.Q<VisualElement>("GameOverScreen");
		_levelFinishedScreen = _rootVisualElement.Q<VisualElement>("LevelFinishedScreen");
	}

	public void UpdateCashLabel(int lives)
	{
		_cashLabel.text = "" + lives;
	}

	public void UpdateLivesLabel(int lives)
	{
		_livesLabel.text = lives + "/" + GameState.Get.MaxLives;
	}

	public void OnGameOver()
	{
		_gameOverScreen.style.display = DisplayStyle.Flex;
	}

	public void OnLevelFinished()
	{
		_levelFinishedScreen.style.display = DisplayStyle.Flex;
	}

	public void SetElementBlockingMouse(VisualElement elem, bool block)
	{
		if (block)
		{
			_elementsBlockingMouse.Add(elem);
		}
		else
		{
			_elementsBlockingMouse.Remove(elem);
		}
	}

	public bool IsMouseBlocked(Vector2 mousePos)
	{
		return _elementsBlockingMouse.Any(elem => elem.ContainsPoint(elem.WorldToLocal(mousePos)));
	}
}