using Mantis.Utils.UI;
using UnityEngine.UIElements;

namespace UI.HUD
{
	// TODO finish this
	public class TowerUpgradePanel : MantisVisualElement
	{
		public new class UxmlFactory : UxmlFactory<TowerUpgradePanel, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		protected override void OnAttach(AttachToPanelEvent evt)
		{
		}

		protected void Init(Tower tower)
		{
			foreach (UpgradePath upgradePath in tower.towerData.upgradePaths)
			{
			}
		}
		
	}
}