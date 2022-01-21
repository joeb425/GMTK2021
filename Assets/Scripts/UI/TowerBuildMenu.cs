using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Data;
using Mantis.Engine;
using Mantis.Utils.UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerBuildMenu : VisualElement
{
	private VisualTreeAsset _iconButton;
	private Dictionary<Button, Tower> _buttonPrefabs = new Dictionary<Button, Tower>();
	private VisualElement _buttons;

	private TwoStageButton _selectedButton;
	private TowerDescription _towerDescription;

	public new class UxmlFactory : UxmlFactory<TowerBuildMenu, UxmlTraits>
	{
	}

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
	}

	public TowerBuildMenu()
	{
		RegisterCallback<AttachToPanelEvent>(OnAttach);
		RegisterCallback((DetachFromPanelEvent evt) => EngineStatics.OnGameInit -= OnGameInit);
	}

	private void OnGameInit()
	{
		GameState gameState = GameState.Get();
		OnCashChanged(gameState.CurrentCash);
		gameState.OnCashChanged += (_, cash) => OnCashChanged(cash);
		gameState.Board.OnSetTowerToBePlaced += OnTowerToBePlacedChanged;

		_towerDescription.BindToTower(null);

		List<Tower> disabledTowers = GameData.Get().GetCurrentLevel().disabledTowers;

		if (disabledTowers.Count == 0)
		{
#if UNITY_EDITOR
			disabledTowers =
				AssetDatabase.FindAssets($"Tower:{nameof(Tower)}").ToList()
					.ConvertAll(guid => AssetDatabase.LoadAssetAtPath<Tower>(AssetDatabase.GUIDToAssetPath(guid)));
#endif
		}

		_buttons.Clear();
		foreach (Tower tower in AssetBindings.Get().gamePrefabs.baseTowers)
		{
			bool isAvailable = !disabledTowers.Contains(tower);
			CreateButton(tower, isAvailable);
		}
	}

	private void OnTowerToBePlacedChanged(Tower tower)
	{
		if (tower == null)
		{
			_selectedButton?.ClearSelectedState();
		}

		_towerDescription.BindToTower(tower);
	}

	private void OnAttach(AttachToPanelEvent evt)
	{
		EngineStatics.OnGameInit += OnGameInit;

		_iconButton = GlobalData.GetAssetBindings().gamePrefabs.towerBuildButton;
		GamePrefabs gamePrefabs = GlobalData.GetAssetBindings().gamePrefabs;

		_buttons = this.Q<VisualElement>("Buttons");
		_towerDescription = this.Q<TowerDescription>();

#if UNITY_EDITOR
		if (_buttons != null)
		{
			_buttons.Clear();
			foreach (Tower tower in AssetBindings.Get().gamePrefabs.baseTowers)
			{
				CreateButton(tower, true);
			}
		}
#endif
	}

	private void OnCashChanged(int currentCash)
	{
		UpdateTowerButtons(currentCash);
	}

	private void UpdateTowerButtons(int currentCash)
	{
		foreach (var (button, tower)  in _buttonPrefabs)
		{
			button.SetEnabled(currentCash >= tower.towerData.towerCost);
		}
	}

	private void CreateButton(Tower towerPrefab, bool isAvailable)
	{
		VisualElement spawnBasicTower = _iconButton.CloneTree();
		_buttons.Add(spawnBasicTower);

		TwoStageButton button = spawnBasicTower.Q<TwoStageButton>("Button");
		var costLabel = spawnBasicTower.Q<Label>("Cost");

		button.SetEnabled(isAvailable);

		if (!towerPrefab)
			return;

		button.onSelected += () =>
		{
			if (_selectedButton != button)
			{
				_selectedButton?.ClearSelectedState();
				_selectedButton = button;
			}

			GameState.Get().Board.SetTowerToBePlaced(towerPrefab);
		};

		button.onConfirmed += () =>
		{
			GameState.Get().Board.PlaceCurrentTower();
			_selectedButton = null;
		};

		costLabel.text = isAvailable ? "" + towerPrefab.towerData.towerCost : "";
		spawnBasicTower.name = towerPrefab.towerData.name;
		button.text = isAvailable ? towerPrefab.towerData.towerName : "";
		button.style.backgroundImage = towerPrefab.towerData.towerIcon;

		if (isAvailable)
		{
			_buttonPrefabs.Add(button, towerPrefab);
		}
	}
}