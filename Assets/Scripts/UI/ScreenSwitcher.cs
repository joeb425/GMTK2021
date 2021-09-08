using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.MainMenu
{
	public class ScreenSwitcher : VisualElement
	{
		public new class UxmlFactory : UxmlFactory<ScreenSwitcher, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		private Dictionary<string, VisualElement> _screens = new Dictionary<string, VisualElement>();

		public System.Action<VisualElement, bool> OnScreenStateChanged;

		public ScreenSwitcher()
		{
			RegisterCallback<AttachToPanelEvent>(OnAttach);
		}

		private void OnAttach(AttachToPanelEvent evt)
		{
			ClearScreens();
			foreach (VisualElement child in Children())
			{
				AddScreen(child.name);
			}
		}

		public void ClearScreens()
		{
			_screens = new Dictionary<string, VisualElement>();
		}

		public void AddScreen(string screenName)
		{
			if (_screens.ContainsKey(screenName))
			{
				return;
			}

			_screens.Add(screenName, this?.Q(screenName));
		}

		public VisualElement EnableScreen(string screenToEnableName)
		{
			VisualElement screenToEnable = null;

			foreach (var kvp in _screens)
			{
				if (kvp.Key == screenToEnableName)
				{
					screenToEnable = kvp.Value;
					kvp.Value.style.display = DisplayStyle.Flex;
					OnScreenStateChanged?.Invoke(kvp.Value, true);
				}
				else
				{
					kvp.Value.style.display = DisplayStyle.None;
					OnScreenStateChanged?.Invoke(kvp.Value, false);
				}
			}

			return screenToEnable;
		}

		public T EnableScreen<T>(string screenToEnableName) where T : VisualElement
		{
			return EnableScreen(screenToEnableName) as T;
		}

		public void HideAll()
		{
			foreach (var screen in _screens.Values)
			{
				screen.style.display = DisplayStyle.None;
				OnScreenStateChanged?.Invoke(screen, false);
			}
		}

		[CanBeNull]
		public T GetScreen<T>(string screenName) where T : VisualElement
		{
			return GetScreen(screenName) as T;
		}

		public VisualElement GetScreen(string screenName)
		{
			return _screens.ContainsKey(screenName) ? _screens[screenName] : null;
		}
	}
}