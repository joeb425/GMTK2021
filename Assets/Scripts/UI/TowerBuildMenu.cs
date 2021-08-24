﻿using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerBuildMenu : VisualElement
{
	private VisualTreeAsset _iconButton;

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
		_iconButton = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/SpawnTowerBtn.uxml");
		GamePrefabs gamePrefabs = AssetDatabase.LoadAssetAtPath<GamePrefabs>("Assets/Data/GamePrefabs.asset");

		VisualElement buttons = this.Q<VisualElement>("Buttons");
		if (buttons != null)
		{
			buttons.Clear();
			CreateButton(buttons, gamePrefabs.basicTowerPrefab, "Basic");
			CreateButton(buttons, gamePrefabs.doubleTowerPrefab, "Double");
			CreateButton(buttons, gamePrefabs.rocketTowerPrefab, "Rocket");
			CreateButton(buttons, gamePrefabs.sniperTowerPrefab, "Sniper");
			CreateButton(buttons, gamePrefabs.smgTowerPrefab, "SMG");
		}
	}

	private void CreateButton(VisualElement rootContainer, Tower towerPrefab, string btnText)
	{
		VisualElement spawnBasicTower = _iconButton.CloneTree();
		rootContainer.Add(spawnBasicTower);
		spawnBasicTower.name = btnText;

		if (towerPrefab)
		{
			spawnBasicTower.RegisterCallback<ClickEvent>(ev => GameState.Get.Board.PlaceTower(towerPrefab));
			spawnBasicTower.Q<Label>("Cost").text = "" + towerPrefab.Cost;
		}

		spawnBasicTower.Q<Button>("Button").text = btnText;
	}
}
