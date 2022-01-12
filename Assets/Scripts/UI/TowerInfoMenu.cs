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
		this.Q("SellBtn")?.RegisterCallback<ClickEvent>(ev => SellTower());
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