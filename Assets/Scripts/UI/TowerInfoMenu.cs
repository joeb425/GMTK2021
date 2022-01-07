using Mantis.Utils.UI;
using UI.MainMenu;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class TowerInfoMenu : MantisVisualElement
{
	private Tower _currentTower;
	private TowerUpgradePanel _towerUpgradePanel;
	private TowerDescription _towerDescription;

	public new class UxmlFactory : UxmlFactory<TowerInfoMenu, UxmlTraits>
	{
	}

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
	}

	protected override void OnAttach(AttachToPanelEvent evt)
	{
		_towerDescription = this.Q<TowerDescription>();
		_towerUpgradePanel = this.Q<TowerUpgradePanel>();
		_towerUpgradePanel.InitTowerDescription(_towerDescription);
		this.Q("UpgradeBtn")?.RegisterCallback<ClickEvent>(ev => UpgradeTower());
		this.Q("LinkBtn")?.RegisterCallback<ClickEvent>(ev => LinkTower());
		this.Q("SellBtn")?.RegisterCallback<ClickEvent>(ev => SellTower());
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
		GameState.Get().Board.StartSelectZoneSpread(_currentTower.groundTile.hex);
	}

	void SellTower()
	{
		// Sell the current tower -> Gameboard tells us content and sells tower?
		Debug.Log("Sell Tower");
		GameState.Get().SellSelectedTower();
	}

	public void BindToTower(Tower tower)
	{
		if (tower is null)
		{
			return;
		}

		_currentTower = tower;
		_towerDescription.BindToTower(tower);
		_towerUpgradePanel.BindToTower(tower);
	}
}