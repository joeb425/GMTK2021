using UnityEditor;
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

	public void Setup()
	{
		_iconButton = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/SpawnTowerBtn.uxml");

		VisualElement buttons = this.Q<VisualElement>("Buttons");
		if (buttons != null)
		{
			buttons.Clear();
			CreateButton(buttons, GameState.Get.Board.BasicTowerPrefab, "Basic");
			CreateButton(buttons, GameState.Get.Board.DoubleTowerPrefab, "Double");
			CreateButton(buttons, GameState.Get.Board.RocketTowerPrefab, "Rocket");
			CreateButton(buttons, GameState.Get.Board.SniperTowerPrefab, "Sniper");
			CreateButton(buttons, GameState.Get.Board.SMGTowerPrefab, "SMG");
		}
	}

	void CreateButton(VisualElement rootContainer, Tower towerPrefab, string btnText)
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
