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
		public Dictionary<String, VisualElement> Screens = new Dictionary<string, VisualElement>();

		public void AddScreen(String screenName)
		{
			Screens.Add(screenName, this?.Q(screenName));
		}

		public void EnableScreen(String screenName)
		{
			foreach (String otherScreenName in Screens.Keys)
			{
				bool isScreenToShow = otherScreenName == screenName;
				Screens[otherScreenName].style.display = isScreenToShow ? DisplayStyle.Flex : DisplayStyle.None;
				Debug.Log(isScreenToShow);
			}
		}

		[CanBeNull]
		public VisualElement GetScreen(String screenName)
		{
			return Screens.ContainsKey(screenName) ? Screens[screenName] : null;
		}
	}
}