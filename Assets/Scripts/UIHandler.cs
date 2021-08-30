using System;
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

	[SerializeField]
	public UIDocument uiDocument;

	private Label _cashLabel;
	private Label _livesLabel;
	private TowerBuildMenu _towerBuildMenu;
	private TowerInfoMenu _towerInfoMenu;
	private VisualElement _towerBuildMenuContainer;
	private VisualElement _towerInfoMenuContainer;

	public void Init()
	{
		ReadUIDocument();

		GameState.Get.OnCashChanged += (oldValue, newValue) => UpdateCashLabel(newValue);
		GameState.Get.OnLivesChanged += (oldValue, newValue) => UpdateLivesLabel(newValue);

		GameState.Get.Board.OnSelectedTileChanged += (oldTile, newTile) => OnSelectedTileChanged(newTile);

		UpdateCashLabel(GameState.Get.CurrentCash);
		UpdateLivesLabel(GameState.Get.CurrentLives);

		SetBuildMenuEnabled(false);
		SetTowerInfoEnabled(false, null);
	}

	public void ReadUIDocument()
	{
		var rootVisualElement = uiDocument.rootVisualElement;
		_cashLabel = rootVisualElement.Q<Label>("Cash");
		_livesLabel = rootVisualElement.Q<Label>("Lives");
		_towerBuildMenu = uiDocument.rootVisualElement.Q<TowerBuildMenu>();
		_towerBuildMenuContainer = rootVisualElement.Q<VisualElement>("TowerBuildMenuContainer");
		_towerInfoMenu = uiDocument.rootVisualElement.Q<TowerInfoMenu>();
		_towerInfoMenuContainer = rootVisualElement.Q<VisualElement>("TowerInfoMenuContainer");
		// Debug.Log(_towerInfoMenu);
		// Debug.Log("define");
	}

	public void SetBuildMenuEnabled(bool menuEnabled)
	{
		_towerBuildMenuContainer.style.display = menuEnabled ? DisplayStyle.Flex : DisplayStyle.None;
		// Debug.Log("open" + menuEnabled);
	}

	public void SetTowerInfoEnabled(bool menuEnabled, [CanBeNull] GameTile tile)
	{
		// Debug.Log("pre ui" + menuEnabled);
		_towerInfoMenuContainer.style.display = menuEnabled ? DisplayStyle.Flex : DisplayStyle.None;

		if (menuEnabled)
		{
			_towerInfoMenu.BindToTower((Tower) tile.Content);
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
}