using System;
using UI;
using UI.MainMenu;
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

	public void Init()
	{
		ReadUIDocument();

		GameState.Get.OnCashChanged += (oldValue, newValue) => UpdateCashLabel(newValue);
		GameState.Get.OnLivesChanged += (oldValue, newValue) => UpdateLivesLabel(newValue);

		GameState.Get.Board.OnSelectedTileChanged += (oldTile, newTile) => OnSelectedTileChanged(newTile);

		UpdateCashLabel(GameState.Get.CurrentCash);
		UpdateLivesLabel(GameState.Get.CurrentLives);

		_towerBuildMenu.Setup();
		SetBuildMenuEnabled(false);
	}

	public void ReadUIDocument()
	{
		var rootVisualElement = uiDocument.rootVisualElement;
		_cashLabel = rootVisualElement.Q<Label>("Cash");
		_livesLabel = rootVisualElement.Q<Label>("Lives");
		_towerBuildMenu = uiDocument.rootVisualElement.Q<TowerBuildMenu>();
	}

	public void SetBuildMenuEnabled(bool menuEnabled)
	{
		_towerBuildMenu.style.display = menuEnabled ? DisplayStyle.Flex : DisplayStyle.None;
	}

	public void SetTowerUIEnabled(bool menuEnabled)
	{
		// TODO: Tower stat panel
	}

	public void UpdateCashLabel(int lives)
	{
		_cashLabel.text = "" + lives;
	}

	public void UpdateLivesLabel(int lives)
	{
		_livesLabel.text = lives + "/" + GameState.Get.MaxLives;
	}

	void OnSelectedTileChanged(GameTile selectedTile)
	{
		bool showTowerUI = selectedTile.Content.Type == GameTileContentType.Tower;
		SetTowerUIEnabled(showTowerUI);

		bool showBuildUI = selectedTile.Content.Type == GameTileContentType.Build;
		SetBuildMenuEnabled(showBuildUI);
	}
}