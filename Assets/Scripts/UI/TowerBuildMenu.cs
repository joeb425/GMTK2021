using System.Collections.Generic;
using Mantis.Engine;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

public class TowerBuildMenu : VisualElement
{
	private VisualTreeAsset _iconButton;
	private Button _confirmButton;
	private Button _cancelButton;
	private Dictionary<Button, Tower> _buttonPrefabs = new Dictionary<Button, Tower>();
	private VisualElement _buttons;

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
		_confirmButton = this.Q<Button>("ConfirmButton");
		_cancelButton = this.Q<Button>("CancelButton");
		_confirmButton.RegisterCallback<ClickEvent>(ev => GameState.Get().Board.PlaceCurrentTower());
		_cancelButton.RegisterCallback<ClickEvent>(ev => GameState.Get().Board.CancelTowerToBePlaced());

		OnCashChanged(GameState.Get().CurrentCash);
		GameState.Get().OnCashChanged += (_, cash) => OnCashChanged(cash);
		GameState.Get().Board.OnSetTowerToBePlaced += tower => UpdateConfirmButton(GameState.Get().CurrentCash);

		List<Tower> availableTowers = GameData.Get().GetCurrentLevel().availableTowers;
		if (availableTowers.Count > 0)
		{
			_buttons.Clear();
			foreach (Tower tower in availableTowers)
			{
				CreateButton(tower, "Basic");
			}
		}
	}

	private void OnAttach(AttachToPanelEvent evt)
	{
		EngineStatics.OnGameInit += OnGameInit;

		_iconButton = GlobalData.GetAssetBindings().gamePrefabs.towerBuildButton;
		GamePrefabs gamePrefabs = GlobalData.GetAssetBindings().gamePrefabs;

		_buttons = this.Q<VisualElement>("Buttons");

		if (_buttons != null)
		{
			_buttons.Clear();
			CreateButton(gamePrefabs.basicTowerPrefab, "Basic");
			CreateButton(gamePrefabs.doubleTowerPrefab, "Double");
			CreateButton(gamePrefabs.rocketTowerPrefab, "Rocket");
			CreateButton(gamePrefabs.sniperTowerPrefab, "Sniper");
			CreateButton(gamePrefabs.smgTowerPrefab, "SMG");
			CreateButton(gamePrefabs.supportTowerPrefab, "Support");
			CreateButton(gamePrefabs.recoupTowerPrefab, "Recoup"); 
			CreateButton(gamePrefabs.slowTowerPrefab, "Slow");
			CreateButton(gamePrefabs.tower9Prefab, "9");
			CreateButton(gamePrefabs.tower10Prefab, "10");
			CreateButton(gamePrefabs.tower11Prefab, "11");
			CreateButton(gamePrefabs.tower12Prefab, "12");
		}
	}

	private void OnCashChanged(int currentCash)
	{
		UpdateConfirmButton(currentCash);
		// UpdateTowerButtons(currentCash);
	}

	private void UpdateConfirmButton(int currentCash)
	{
		Tower tower = GameState.Get().Board.towerToBePlaced;
		if (tower != null)
		{
			_confirmButton.SetEnabled(currentCash >= tower.towerData.towerCost);
		}
	}

	private void UpdateTowerButtons(int currentCash)
	{
		foreach (var (button, tower)  in _buttonPrefabs)
		{
			button.SetEnabled(currentCash >= tower.towerData.towerCost);
		}
	}

	private void CreateButton(Tower towerPrefab, string btnText)
	{
		VisualElement spawnBasicTower = _iconButton.CloneTree();
		_buttons.Add(spawnBasicTower);

		Button button = spawnBasicTower.Q<Button>("Button");
		var costLabel = spawnBasicTower.Q<Label>("Cost");

		if (!towerPrefab)
			return;

		spawnBasicTower.RegisterCallback<ClickEvent>(ev =>
		{
			GameState.Get().Board.SetTowerToBePlaced(towerPrefab);
		});

		costLabel.text = "" + towerPrefab.towerData.towerCost;
		spawnBasicTower.name = towerPrefab.towerData.name;
		button.text = towerPrefab.GetGameplayTag().GetLeafName();
		button.style.backgroundImage = towerPrefab.towerData.towerIcon;

		_buttonPrefabs.Add(button, towerPrefab);
	}
}