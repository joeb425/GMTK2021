using System.Collections.Generic;
using Mantis.Engine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UIElements;

public class TowerBuildMenu : VisualElement
{
	private VisualTreeAsset _iconButton;
	private Button _confirmButton;
	private Button _cancelButton;
	private Dictionary<Button, Tower> _buttonPrefabs = new Dictionary<Button, Tower>();

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
	}

	private void OnAttach(AttachToPanelEvent evt)
	{
		EngineStatics.OnGameInit += OnGameInit;

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

	private void OnCashChanged(int currentCash)
	{
		Tower tower = GameState.Get().Board.towerToBePlaced;
		if (tower != null)
		{
			UpdateConfirmButton(currentCash);
		}
		
		UpdateTowerButtons(currentCash);
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

	private void CreateButton(VisualElement rootContainer, Tower towerPrefab, string btnText)
	{
		VisualElement spawnBasicTower = _iconButton.CloneTree();
		rootContainer.Add(spawnBasicTower);

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
		button.text = btnText;
		button.style.backgroundImage = towerPrefab.towerData.towerIcon;

		_buttonPrefabs.Add(button, towerPrefab);
	}
}