using UnityEngine;
using UnityEngine.UIElements;

namespace UI.MainMenu.HUD
{
	// TODO finish this
	public class TowerButton : Button
	{
		private ScreenSwitcher _screenSwitcher;
		private Tower _towerPrefab;

		public new class UxmlFactory : UxmlFactory<TowerButton, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		public void Init(Tower towerPrefab)
		{
			_towerPrefab = towerPrefab;
			GameState.Get().OnCashChanged += (_, cash) => CheckPrice(cash);
		}

		public void CheckPrice(int cash)
		{
			bool canAfford = cash >= _towerPrefab.towerData.towerCost;
			SetEnabled(canAfford);
		}
	}
}