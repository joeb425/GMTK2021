using System;
using HexLibrary;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.MainMenu.HUD
{
	public class GameSidePanel : VisualElement
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

		public GameSidePanel()
		{
			RegisterCallback<AttachToPanelEvent>(OnAttach);
			GlobalData.OnGameInit += OnGameInit;
		}

		private void OnAttach(AttachToPanelEvent evt)
		{
		}

		public void OnScreenStateChanged(VisualElement screen, bool enabled)
		{
			Game.Get.uiHandler.SetElementBlockingMouse(screen, enabled);
		}

		public void OnGameInit()
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
			
			GameState.Get.Board.OnTowerPlaced += OnTowerPlaced;
			GameState.Get.Board.OnSelectedTileChanged += (_, newHex) => OnSelectedTileChanged(newHex);
		}

		void OnSelectedTileChanged(Hex selectedTile)
		{
			// check tower layer
			if (GameState.Get.Board.towerLayer.GetComponentAtHex(selectedTile, out Tower tower))
			{
				_screenSwitcher.EnableScreen(TowerInfoMenuName).Q<TowerInfoMenu>()?.BindToTower(tower);
				return;
			}

			// check ground layer
			if (GameState.Get.Board.groundLayer.GetComponentAtHex(selectedTile, out GroundTileComponent groundTile))
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