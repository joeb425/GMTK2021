using System;
using HexLibrary;
using Mantis.Engine;
using Mantis.Hex;
using Mantis.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.HUD
{
	public abstract class GameVisualElement : MantisVisualElement
	{
		public GameVisualElement()
		{
			if (!Application.isPlaying) 
				return;

			RegisterCallback((AttachToPanelEvent evt) =>
			{
				EngineStatics.OnGamePreInit -= OnGamePreInit;
				EngineStatics.OnGamePreInit += OnGamePreInit;

				EngineStatics.OnGameInit -= OnGameInit;
				EngineStatics.OnGameInit += OnGameInit;
			});

			RegisterCallback((DetachFromPanelEvent evt) =>
			{
				EngineStatics.OnGamePreInit -= OnGamePreInit;
				EngineStatics.OnGameInit -= OnGameInit;
			});
		}

		public virtual void OnGamePreInit()
		{
		}

		public virtual void OnGameInit()
		{
		}
	}
}