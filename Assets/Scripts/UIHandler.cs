using UnityEngine;

using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
	[SerializeField]
	public Canvas BuildMenu = default;

	[SerializeField]
	public Canvas TowerUI = default;

	[SerializeField]
	public UIDocument uiDocument;

	public Label cashLabel;
	public Label livesLabel;

	public void Init()
	{
		SetBuildMenuEnabled(false);
		//
		// BindButton(board.PlaceBasicTower, board.BasicTowerPrefab, "SpawnBasicBtn", "Basic");
  //       BindButton(board.PlaceDoubleTower, board.DoubleTowerPrefab, "SpawnDoubleBtn", "Double");
  //       BindButton(board.PlaceRocketTower, board.RocketTowerPrefab, "SpawnRocketBtn", "Rocket");
  //       BindButton(board.PlaceSniperTower, board.SniperTowerPrefab, "SpawnSniperBtn", "Sniper");
  //       BindButton(board.PlaceSMGTower, board.SMGTowerPrefab, "SpawnSMGBtn", "SMG");
  //
  //       var rootVisualElement = uiDocument.rootVisualElement;
  //       cashLabel = rootVisualElement.Q<Label>("Cash");
  //       livesLabel = rootVisualElement.Q<Label>("Lives");
  //       SetCash(50);
  //       currentLives = maxLives;
  //       SetCash(board.cash);
	}

	public void SetBuildMenuEnabled(bool menuEnabled)
	{
		CanvasGroup canvasGroup = BuildMenu.GetComponent<CanvasGroup>();
		canvasGroup.alpha = menuEnabled ? 1.0f : 0.0f;
		canvasGroup.interactable = menuEnabled;
		// IsGUIEnabled = menuEnabled;
	}

	public void SetTowerUIEnabled(bool menuEnabled)
	{
		CanvasGroup canvasGroup = TowerUI.GetComponent<CanvasGroup>();
		canvasGroup.alpha = menuEnabled ? 1.0f : 0.0f;
		canvasGroup.interactable = true;
		// IsGUIEnabled = menuEnabled;
	}

	public void SetCash(int cash)
	{
		cashLabel.text = "" + cash;
	}

	public void SetLives(int lives)
	{
		// currentLives = currentLives - lives;
		// livesLabel.text = currentLives + "/" + maxLives;
	}

	void BindButton(Tower towerPrefab, string btnName, string btnText)
	{
		var rootVisualElement = uiDocument.rootVisualElement;
		var spawnBasicTower = rootVisualElement.Q(btnName);
		
		// Spawn tower?
		// spawnBasicTower.RegisterCallback<ClickEvent>(ev => SetTowerToBePlaced(towerPrefab));
		
		spawnBasicTower.Q<Label>("Cost").text = "" + towerPrefab.Cost;
		spawnBasicTower.Q<Button>("Button").text = btnText;
	}
}
