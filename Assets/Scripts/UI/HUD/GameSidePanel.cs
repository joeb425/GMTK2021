﻿using System;
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

			_screenSwitcher.HideAll();
			_screenSwitcher.OnScreenStateChanged += OnScreenStateChanged;
			
			Game.Get.gameState.Board.OnTowerPlaced += OnTowerPlaced;
			Game.Get.gameState.Board.OnSelectedTileChanged += (_, newHex) => OnSelectedTileChanged(newHex);
		}

		void OnSelectedTileChanged(Hex selectedTile)
		{
			// check tower layer
			if (GameState.Get.Board.towerLayer.GetTile(selectedTile, out GameObject towerContent))
			{
				Tower tower = towerContent.GetComponent<Tower>();
				if (tower != null)
				{
					_screenSwitcher.EnableScreen(TowerInfoMenuName).Q<TowerInfoMenu>()?.BindToTower(tower);
					return;
				}
			}

			// check ground layer
			if (!GameState.Get.Board.groundLayer.GetTile(selectedTile, out GameObject tileContent))
			{
				_screenSwitcher.HideAll();
				return;
			}

			var hexComponent = tileContent.GetComponent<HexComponent>();
			if (hexComponent == null)
			{
				_screenSwitcher.HideAll();
				return;
			}

			switch (hexComponent.TileType)
			{
				case HexTileType.Build:
					_screenSwitcher.EnableScreen(TowerBuildMenuName);
					break;
				default:
					_screenSwitcher.HideAll();
					break;
			}
		}

		private void OnTowerPlaced(Hex hex, Tower tower)
		{
			_screenSwitcher.EnableScreen(TowerInfoMenuName).Q<TowerInfoMenu>()?.BindToTower(tower);
		}
	}
}