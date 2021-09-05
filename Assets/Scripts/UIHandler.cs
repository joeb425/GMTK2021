using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UI;
using UI.MainMenu;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
	// [SerializeField]
	// public Canvas buildMenu = default;

	// [SerializeField]
	// public Canvas towerUI = default;

	public static UIHandler Get { get; private set; }

	[SerializeField]
	public UIDocument uiDocument;

	[SerializeField]
	public VisualTreeAsset gameOverScreen;

	private VisualElement _rootVisualElement;
	private Label _cashLabel;
	private Label _livesLabel;
	private TowerBuildMenu _towerBuildMenu;
	private TowerInfoMenu _towerInfoMenu;
	private VisualElement _towerBuildMenuContainer;
	private VisualElement _towerInfoMenuContainer;
	private VisualElement _gameOverScreen;
	private VisualElement _levelFinishedScreen;

	private List<VisualElement> _elementsBlockingMouse = new List<VisualElement>();

	public void Init()
	{
		Get = this;

		ReadUIDocument();

		GameState.Get.OnCashChanged += (oldValue, newValue) => UpdateCashLabel(newValue);
		GameState.Get.OnLivesChanged += (oldValue, newValue) => UpdateLivesLabel(newValue);

		// GameState.Get.Board.OnSelectedTileChanged += (oldTile, newTile) => OnSelectedTileChanged(newTile);

		GameState.Get.OnGameOver += OnGameOver;

		GameState.Get.OnLevelFinished += OnLevelFinished;

		UpdateCashLabel(GameState.Get.CurrentCash);
		UpdateLivesLabel(GameState.Get.CurrentLives);

		SetBuildMenuEnabled(false);
		SetTowerInfoEnabled(false, null);
	}

	public void ReadUIDocument()
	{
		_rootVisualElement = uiDocument.rootVisualElement;
		_cashLabel = _rootVisualElement.Q<Label>("Cash");
		_livesLabel = _rootVisualElement.Q<Label>("Lives");
		_towerBuildMenu = uiDocument.rootVisualElement.Q<TowerBuildMenu>();
		_towerBuildMenuContainer = _rootVisualElement.Q<VisualElement>("TowerBuildMenuContainer");
		_towerInfoMenu = uiDocument.rootVisualElement.Q<TowerInfoMenu>();
		_towerInfoMenuContainer = _rootVisualElement.Q<VisualElement>("TowerInfoMenuContainer");
		_gameOverScreen = _rootVisualElement.Q<VisualElement>("GameOverScreenContainer");
		_levelFinishedScreen = _rootVisualElement.Q<VisualElement>("LevelFinishedScreenContainer");
	}

	public void SetBuildMenuEnabled(bool menuEnabled)
	{
		_towerBuildMenuContainer.style.display = menuEnabled ? DisplayStyle.Flex : DisplayStyle.None;
		SetElementBlockingMouse(_towerBuildMenuContainer, menuEnabled);
		// Debug.Log("open" + menuEnabled);
	}

	public void SetTowerInfoEnabled(bool menuEnabled, [CanBeNull] GameTile tile)
	{
		// Debug.Log("pre ui" + menuEnabled);
		_towerInfoMenuContainer.style.display = menuEnabled ? DisplayStyle.Flex : DisplayStyle.None;
		SetElementBlockingMouse(_towerInfoMenuContainer, menuEnabled);
		if (menuEnabled)
		{
			_towerInfoMenu.BindToTower((Tower)tile.Content);
		}
	}

	void OnSelectedTileChanged(GameTile selectedTile)
	{
		bool showBuildUI = selectedTile.Content.Type == GameTileContentType.Build;
		SetBuildMenuEnabled(showBuildUI);
		bool showTowerUI = selectedTile.Content.Type == GameTileContentType.Tower;
		SetTowerInfoEnabled(showTowerUI, selectedTile);
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
		// _rootVisualElement.Add(gameOverScreen.CloneTree());
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