using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
public class TowerInfoMenu : VisualElement
{
	private VisualTreeAsset _iconButton;

	public new class UxmlFactory : UxmlFactory<TowerInfoMenu, UxmlTraits>
	{
	}

	public new class UxmlTraits : VisualElement.UxmlTraits
	{
	}

	public TowerInfoMenu()
	{
		RegisterCallback<AttachToPanelEvent>(OnAttach);
	}

	private void OnAttach(AttachToPanelEvent evt)
	{
		Debug.Log("tower info initiate");
		this.Q("UpgradeBtn")?.RegisterCallback<ClickEvent>(ev => UpgradeTower());
		this.Q("LinkBtn")?.RegisterCallback<ClickEvent>(ev => LinkTower());
		this.Q("SellBtn")?.RegisterCallback<ClickEvent>(ev => SellTower());
	}
	void UpgradeTower()
	{
		// Grab tower on tile from gameboard and upgrade from tower clasS?
		Debug.Log("upgrade a tower");
	}

	void LinkTower()
	{
		// The linking stuff
		Debug.Log("link a tower");
	}

	void SellTower()
	{
		// Sell the current tower -> Gameboard tells us content and sells tower?
		Debug.Log("Sell Tower");

	}
}
