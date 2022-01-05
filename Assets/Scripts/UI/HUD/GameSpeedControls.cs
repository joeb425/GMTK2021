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
	public class GameSpeedControls : GameVisualElement
	{
		public new class UxmlFactory : UxmlFactory<GameSpeedControls, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		private Button _speedUpButton;
		private Button _slowDownButton;
		private Label _gameSpeedLabel;

		public override void OnGameInit()
		{
			_speedUpButton = this.Q<Button>("SpeedUpButton");
			_speedUpButton.RegisterCallback<ClickEvent>(ev => GameState.Get().SpeedUpGame());

			_slowDownButton = this.Q<Button>("SlowDownButton");
			_slowDownButton.RegisterCallback<ClickEvent>(ev => GameState.Get().SlowDownGame());

			_gameSpeedLabel = this.Q<Label>("GameSpeedLabel");
			GameState.Get().OnGameSpeedChanged += f => _gameSpeedLabel.text = f + "x"; 

			UIHandler uiHandler = Game.Get.GetUIHandler();
			uiHandler.SetElementBlockingMouse(_speedUpButton, true);
			uiHandler.SetElementBlockingMouse(_slowDownButton, true);
		}
	}
}