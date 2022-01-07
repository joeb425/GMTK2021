using System;
using System.Linq;
using HexLibrary;
using Mantis.Engine;
using Mantis.Hex;
using Mantis.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.HUD
{
	public class GameSidePanel : GameVisualElement
	{
		private ScreenSwitcher _screenSwitcher;

		public new class UxmlFactory : UxmlFactory<GameSidePanel, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		public static string TowerBuildMenuName = "TowerBuildMenu";
		public static string TowerInfoMenuName = "TowerInfoMenu";

		public void OnScreenStateChanged(VisualElement screen, bool enabled)
		{
			// VisualElement screenContent = screen.Children().First();
			// if (screenContent != null)
			// {
			// 	Game.Get.GetUIHandler().SetElementBlockingMouse(screenContent, enabled);
			// }

			Game.Get.GetUIHandler().SetElementBlockingMouse(screen, enabled);
		}

		public override void OnGameInit()
		{
			_screenSwitcher = this.Q<ScreenSwitcher>();
			if (_screenSwitcher is null)
			{
				Debug.LogError("No screen switcher found in game side panel");
				return;
			}

			_screenSwitcher.ReadScreens();
			_screenSwitcher.HideAll();
			_screenSwitcher.OnScreenStateChanged += OnScreenStateChanged;

			GameBoard board = GameState.Get().Board;
			board.OnTowerPlaced -= OnTowerPlaced;
			board.OnTowerPlaced += OnTowerPlaced;

			board.OnSelectedTileChanged -= OnSelectedTileChanged;
			board.OnSelectedTileChanged += OnSelectedTileChanged;
		}

		void OnSelectedTileChanged(Hex oldHex, Hex selectedTile)
		{
			// check tower layer
			if (GameState.Get().Board.towerLayer.GetTileAtHex(selectedTile, out Tower tower))
			{
				_screenSwitcher.EnableScreen(TowerInfoMenuName).Q<TowerInfoMenu>()?.BindToTower(tower);
				return;
			}

			// check ground layer
			if (GameState.Get().Board.groundLayer.GetTileAtHex(selectedTile, out GroundTileComponent groundTile))
			{
				if (groundTile.TileType == HexTileType.Build)
				{
					_screenSwitcher.EnableScreen(TowerBuildMenuName);
				}
				else
				{
					_screenSwitcher.HideAll();
				}

				return;
			}

			_screenSwitcher.HideAll();
		}

		private void OnTowerPlaced(Hex hex, Tower tower)
		{
			_screenSwitcher.EnableScreen(TowerInfoMenuName).Q<TowerInfoMenu>()?.BindToTower(tower);
		}
	}
}