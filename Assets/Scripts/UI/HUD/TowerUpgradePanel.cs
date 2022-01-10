using System.Collections.Generic;
using Mantis.Utils.UI;
using UI.HUD;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerUpgradePanel : GameVisualElement
{
	private VisualTreeAsset _ugradeButtonTemplate;
	private Dictionary<Button, Tower> _buttonPrefabs = new Dictionary<Button, Tower>();
	private VisualElement _buttons;

	private TwoStageButton _selectedButton;
	private TowerDescription _towerDescription;

	public new class UxmlFactory : UxmlFactory<TowerUpgradePanel, UxmlTraits>
	{
	}

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
	}

	protected override void OnAttach(AttachToPanelEvent evt)
	{
		_ugradeButtonTemplate = GlobalData.GetAssetBindings().gamePrefabs.upgradeTowerButton;
		_buttons = this.Q<VisualElement>("Buttons");
	}

	public override void OnGameInit()
	{
		GameState gs = GameState.Get();
		gs.OnCashChanged += (_, newCash) => UpdateTowerButtons(newCash);
		UpdateTowerButtons(gs.CurrentCash);
	}

	public void InitTowerDescription(TowerDescription towerDescription)
	{
		_towerDescription = towerDescription;
	}

	public void BindToTower(Tower tower)
	{
		_buttons.Clear();
		foreach (UpgradePath towerDataUpgradePath in tower.towerData.upgradePaths)
		{
			CreateButton(towerDataUpgradePath);
		}
	}

	private void UpdateTowerButtons(int currentCash)
	{
		foreach (var (button, tower) in _buttonPrefabs)
		{
			button.SetEnabled(currentCash >= tower.towerData.towerCost);
		}
	}

	private void CreateButton(UpgradePath upgradePath)
	{
		Tower towerPrefab = upgradePath.tower;
		VisualElement spawnBasicTower = _ugradeButtonTemplate.CloneTree();
		_buttons.Add(spawnBasicTower);

		TwoStageButton button = spawnBasicTower.Q<TwoStageButton>("Button");
		var costLabel = spawnBasicTower.Q<Label>("Cost");

		if (!towerPrefab)
			return;

		button.onSelected += () =>
		{
			if (_selectedButton != button)
			{
				_selectedButton?.ClearSelectedState();
				_selectedButton = button;
			}

			_towerDescription.BindToTower(towerPrefab);
			Debug.Log($"Select tower {towerPrefab}");
		};

		button.onConfirmed += () =>
		{
			GameBoard board = GameState.Get().Board;
			if (!board.GetTowerAtHex(board.selectedTile, out Tower tower))
				return;

			tower.UpgradeTower(upgradePath);
			_selectedButton = null;
		};

		costLabel.text = "" + towerPrefab.towerData.towerCost;
		spawnBasicTower.name = towerPrefab.towerData.name;
		button.text = towerPrefab.GetGameplayTag().GetLeafName();
		button.style.backgroundImage = towerPrefab.towerData.towerIcon;

		_buttonPrefabs.Add(button, towerPrefab);
	}
}