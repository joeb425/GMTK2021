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
	public class GameSettingsMenu : GameVisualElement
	{
		public new class UxmlFactory : UxmlFactory<GameSettingsMenu, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		public override void OnGameInit()
		{
		}
	}
}