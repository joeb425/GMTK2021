using UnityEngine;

using UnityEngine.UIElements;

public class UIHandler : MonoBehaviour
{
	[SerializeField]
	public Canvas buildMenu = default;

	[SerializeField]
	public Canvas towerUI = default;

	[SerializeField]
	public UIDocument uiDocument;

	private Label _cashLabel;
	private Label _livesLabel;

	public void Init()
	{
		ReadUIDocument();
		
		SetBuildMenuEnabled(false);
		GameState.GlobalGameState.OnCashChanged += (oldValue, newValue) => UpdateCashLabel(newValue);
		GameState.GlobalGameState.OnLivesChanged += (oldValue, newValue) => UpdateLivesLabel(newValue);

		UpdateCashLabel(GameState.GlobalGameState.CurrentCash);
		UpdateLivesLabel(GameState.GlobalGameState.CurrentLives);

		// BindButton(board.PlaceBasicTower, board.BasicTowerPrefab, "SpawnBasicBtn", "Basic");
		//       BindButton(board.PlaceDoubleTower, board.DoubleTowerPrefab, "SpawnDoubleBtn", "Double");
		//       BindButton(board.PlaceRocketTower, board.RocketTowerPrefab, "SpawnRocketBtn", "Rocket");
		//       BindButton(board.PlaceSniperTower, board.SniperTowerPrefab, "SpawnSniperBtn", "Sniper");
		//       BindButton(board.PlaceSMGTower, board.SMGTowerPrefab, "SpawnSMGBtn", "SMG");
		//
	}

	public void ReadUIDocument()
	{
		var rootVisualElement = uiDocument.rootVisualElement;
		_cashLabel = rootVisualElement.Q<Label>("Cash");
		_livesLabel = rootVisualElement.Q<Label>("Lives");
	}

	public void SetBuildMenuEnabled(bool menuEnabled)
	{
		CanvasGroup canvasGroup = buildMenu.GetComponent<CanvasGroup>();
		canvasGroup.alpha = menuEnabled ? 1.0f : 0.0f;
		canvasGroup.interactable = menuEnabled;
	}

	public void SetTowerUIEnabled(bool menuEnabled)
	{
		CanvasGroup canvasGroup = towerUI.GetComponent<CanvasGroup>();
		canvasGroup.alpha = menuEnabled ? 1.0f : 0.0f;
		canvasGroup.interactable = true;
	}

	public void UpdateCashLabel(int lives)
	{
		_cashLabel.text = "" + lives;
	}

	public void UpdateLivesLabel(int lives)
	{
		_livesLabel.text = lives + "/" + GameState.GlobalGameState.MaxLives;
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
