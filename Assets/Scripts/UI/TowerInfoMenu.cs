using UI.MainMenu;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerInfoMenu : VisualElement
{
	private Tower _currentTower;

	public AttributeContainerDisplay AttributeContainerDisplay { get; private set; }

	public new class UxmlFactory : UxmlFactory<TowerInfoMenu, UxmlTraits>
	{
	}

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
	}

	public TowerInfoMenu()
	{
		RegisterCallback<AttachToPanelEvent>(OnAttach); // phil?=<ghenius 
	}

	private void OnAttach(AttachToPanelEvent evt)
	{
		this.Q("UpgradeBtn")?.RegisterCallback<ClickEvent>(ev => UpgradeTower());
		this.Q("LinkBtn")?.RegisterCallback<ClickEvent>(ev => LinkTower());
		this.Q("SellBtn")?.RegisterCallback<ClickEvent>(ev => SellTower());
		
		AttributeContainerDisplay = this.Q<AttributeContainerDisplay>();
		if (AttributeContainerDisplay == null)
		{
			Debug.Log("Failed to find attribute container display");
		}
	}

	void UpgradeTower()
	{
		// Grab tower on tile from gameboard and upgrade from tower clasS?
		Debug.Log("upgrade a tower");
		_currentTower.UpgradeTower();
	}

	void LinkTower()
	{
		// The linking stuff
		Debug.Log("link a tower");
		// Need to select a tile that has a spread on it without wrecking the interfaces and messing everything up
		// need to store the current selected tile and pass the next selected tile as well
		// Grab the position of the tower and query the board for the ground tile can change the colour from there
		// can cheat and use selectedtile and oldselectedtile
		_currentTower.LinkTower();
	}

	void SellTower()
	{
		// Sell the current tower -> Gameboard tells us content and sells tower?
		Debug.Log("Sell Tower");
		// GameState.Get.Board.selectedTile.Content.Recycle();
		// GameState.Get.Board.ToggleBuildSpot(GameState.Get.Board.selectedTile);
		// GameState.Get.SetCash(GameState.Get.CurrentCash + 15);
	}

	public void BindToTower(Tower tower)
	{
		if (tower is null)
		{
			return;
		}

		_currentTower = tower;
		AttributeContainerDisplay.BindToAttributeContainer(tower.Attributes);
	}
}