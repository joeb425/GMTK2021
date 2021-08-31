using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
	public class LevelSelectionScreen : VisualElement
	{
		public new class UxmlFactory : UxmlFactory<LevelSelectionScreen, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		public LevelSelectionScreen()
		{
			RegisterCallback<AttachToPanelEvent>(OnAttach);
		}

		private void OnAttach(AttachToPanelEvent evt)
		{
		}
	}
}