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
		GameState.Get.OnCashChanged += (oldValue, newValue) => UpdateCashLabel(newValue);
		GameState.Get.OnLivesChanged += (oldValue, newValue) => UpdateLivesLabel(newValue);

		GameState.Get.Board.OnSelectedTileChanged += (oldTile, newTile) => OnSelectedTileChanged(newTile);

		UpdateCashLabel(GameState.Get.CurrentCash);
		UpdateLivesLabel(GameState.Get.CurrentLives);

		BindButton(GameState.Get.Board.BasicTowerPrefab, "SpawnBasicBtn", "Basic");
		BindButton(GameState.Get.Board.DoubleTowerPrefab, "SpawnDoubleBtn", "Double");
		BindButton(GameState.Get.Board.RocketTowerPrefab, "SpawnRocketBtn", "Rocket");
		BindButton(GameState.Get.Board.SniperTowerPrefab, "SpawnSniperBtn", "Sniper");
		BindButton(GameState.Get.Board.SMGTowerPrefab, "SpawnSMGBtn", "SMG");
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
		_livesLabel.text = lives + "/" + GameState.Get.MaxLives;
	}

	void BindButton(Tower towerPrefab, string btnName, string btnText)
	{
		var rootVisualElement = uiDocument.rootVisualElement;
		var spawnBasicTower = rootVisualElement.Q(btnName);
		
		// Spawn tower?
		spawnBasicTower.RegisterCallback<ClickEvent>(ev => GameState.Get.Board.SetTowerToBePlaced(towerPrefab));
		
		spawnBasicTower.Q<Label>("Cost").text = "" + towerPrefab.Cost;
		spawnBasicTower.Q<Button>("Button").text = btnText;
	}

	void OnSelectedTileChanged(GameTile selectedTile)
	{
		bool showTowerUI = selectedTile.Content.Type == GameTileContentType.Tower;
		SetTowerUIEnabled(showTowerUI);
	}
}
