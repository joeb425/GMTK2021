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
	public class GameTopPanel : GameVisualElement
	{
		public new class UxmlFactory : UxmlFactory<GameTopPanel, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		private Label _cashLabel;
		private Label _livesLabel;

		public override void OnGameInit()
		{
			_cashLabel = this.Q<Label>("Cash");
			_livesLabel = this.Q<Label>("Lives");
			GameState.Get().OnCashChanged += (oldValue, newValue) => UpdateCashLabel(newValue);
			GameState.Get().OnLivesChanged += (oldValue, newValue) => UpdateLivesLabel(newValue);
			UpdateCashLabel(GameState.Get().CurrentCash);
			UpdateLivesLabel(GameState.Get().CurrentLives);
		}

		private void UpdateCashLabel(int lives)
		{
			_cashLabel.text = "" + lives;
		}

		private void UpdateLivesLabel(int lives)
		{
			_livesLabel.text = "" + lives;
		}

	}
}