﻿using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerBuildMenu : VisualElement
{
	private VisualTreeAsset _iconButton;
	private Button _confirmButton;
	private Button _cancelButton;

	public new class UxmlFactory : UxmlFactory<TowerBuildMenu, UxmlTraits>
	{
	}

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
	}

	public TowerBuildMenu()
	{
		RegisterCallback<AttachToPanelEvent>(OnAttach);
	}

	private void OnAttach(AttachToPanelEvent evt)
	{
		_confirmButton = this.Q<Button>("ConfirmButton");
		_confirmButton.RegisterCallback<ClickEvent>(ev => GameState.Get().Board.PlaceCurrentTower());
		_cancelButton = this.Q<Button>("CancelButton");
		_cancelButton.RegisterCallback<ClickEvent>(ev => GameState.Get().Board.CancelTowerToBePlaced());

		_iconButton = GlobalData.GetAssetBindings().gamePrefabs.towerBuildButton;
		GamePrefabs gamePrefabs = GlobalData.GetAssetBindings().gamePrefabs;

		VisualElement buttons = this.Q<VisualElement>("Buttons");
		if (buttons != null)
		{
			buttons.Clear();
			CreateButton(buttons, gamePrefabs.basicTowerPrefab, "Basic");
			CreateButton(buttons, gamePrefabs.doubleTowerPrefab, "Double");
			CreateButton(buttons, gamePrefabs.rocketTowerPrefab, "Rocket");
			CreateButton(buttons, gamePrefabs.sniperTowerPrefab, "Sniper");
			CreateButton(buttons, gamePrefabs.smgTowerPrefab, "SMG");
			CreateButton(buttons, gamePrefabs.supportTowerPrefab, "Support");
			CreateButton(buttons, gamePrefabs.recoupTowerPrefab, "Recoup"); 
			CreateButton(buttons, gamePrefabs.slowTowerPrefab, "Slow");
			CreateButton(buttons, gamePrefabs.tower9Prefab, "9");
			CreateButton(buttons, gamePrefabs.tower10Prefab, "10");
			CreateButton(buttons, gamePrefabs.tower11Prefab, "11");
			CreateButton(buttons, gamePrefabs.tower12Prefab, "12");
		}
	}

	private void CreateButton(VisualElement rootContainer, Tower towerPrefab, string btnText)
	{
		VisualElement spawnBasicTower = _iconButton.CloneTree();
		rootContainer.Add(spawnBasicTower);

		Button button = spawnBasicTower.Q<Button>("Button");
		var costLabel = spawnBasicTower.Q<Label>("Cost");

		if (!towerPrefab)
			return;

		// spawnBasicTower.RegisterCallback<ClickEvent>(ev => GameState.Get().Board.PlaceTowerAtSelectedTile(towerPrefab));
		spawnBasicTower.RegisterCallback<ClickEvent>(ev => GameState.Get().Board.SetTowerToBePlaced(towerPrefab));
		costLabel.text = "" + towerPrefab.towerData.towerCost;
		spawnBasicTower.name = towerPrefab.towerData.name;
		button.text = btnText;
		button.style.backgroundImage = towerPrefab.towerData.towerIcon;
	}
}