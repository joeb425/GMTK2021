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
	private TowerBuildMenu _towerBuildMenu;
	private TowerInfoMenu _towerInfoMenu;
	private VisualElement _towerBuildMenuContainer;
	private VisualElement _towerInfoMenuContainer;
	private VisualElement _gameOverScreen;
	private VisualElement _levelFinishedScreen;

	private List<VisualElement> _elementsBlockingMouse = new List<VisualElement>();

	public void Init(UIDocument hud)
	{
		this.hud = hud;
		Debug.Log("UIHandler Init");

		ReadUIDocument();

		GameState.Get.OnCashChanged += (oldValue, newValue) => UpdateCashLabel(newValue);
		GameState.Get.OnLivesChanged += (oldValue, newValue) => UpdateLivesLabel(newValue);

		GameState.Get.Board.OnSelectedTileChanged += (oldTile, newTile) => OnSelectedTileChanged(newTile);
		GameState.Get.Board.OnTowerPlaced += (hex, tower) => SetTowerInfoEnabled(true);

		GameState.Get.OnGameOver += OnGameOver;

		GameState.Get.OnLevelFinished += OnLevelFinished;

		UpdateCashLabel(GameState.Get.CurrentCash);
		UpdateLivesLabel(GameState.Get.CurrentLives);

		SetBuildMenuEnabled(false);
		UpdateTowerInfoEnabled(null);
	}

	public void ReadUIDocument()
	{
		_rootVisualElement = hud.rootVisualElement;
		_cashLabel = _rootVisualElement.Q<Label>("Cash");
		_livesLabel = _rootVisualElement.Q<Label>("Lives");
		_towerBuildMenu = hud.rootVisualElement.Q<TowerBuildMenu>();
		_towerBuildMenuContainer = _rootVisualElement.Q<VisualElement>("TowerBuildMenuContainer");
		_towerInfoMenu = hud.rootVisualElement.Q<TowerInfoMenu>();
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

	public void SetTowerInfoEnabled(bool menuEnabled)
	{
		_towerInfoMenuContainer.style.display = menuEnabled ? DisplayStyle.Flex : DisplayStyle.None;
		SetElementBlockingMouse(_towerInfoMenuContainer, menuEnabled);
	}

	public bool UpdateTowerInfoEnabled(Hex hex)
	{
		bool menuEnabled = false;

		if (GameState.Get.Board.towerLayer.GetTile(hex, out var content))
		{
			Tower tower = content.GetComponent<Tower>();
			if (tower != null)
			{
				_towerInfoMenu.BindToTower(tower);
				menuEnabled = true;
			}
		}

		_towerInfoMenuContainer.style.display = menuEnabled ? DisplayStyle.Flex : DisplayStyle.None;
		SetElementBlockingMouse(_towerInfoMenuContainer, menuEnabled);

		return menuEnabled;
	}

	void OnSelectedTileChanged(Hex selectedTile)
	{
		if (!GameState.Get.Board.groundLayer.GetTile(selectedTile, out var tileContent))
		{
			return;
		}

		if (UpdateTowerInfoEnabled(selectedTile))
		{
			return;
		}
		
		bool showBuildUI = false;
		var hexComponent = tileContent.GetComponent<HexComponent>();
		if (hexComponent != null)
		{
			showBuildUI = hexComponent.TileType == HexTileType.Build;
		}

		SetBuildMenuEnabled(showBuildUI);
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