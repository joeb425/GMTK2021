using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
	public class MainMenuManager : ScreenSwitcher
		//ScreenSwitcher
	{
		// private ScreenSwitcher ScreenSwitcher;
		
		public new class UxmlFactory : UxmlFactory<MainMenuManager, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		private static String MENU_SCREEN_NAME = "MenuScreen";
		private static String SETTINGS_SCREEN_NAME = "SettingsScreen";

		public MainMenuManager()
		{
			Debug.Log("Test");
			// yes need this
			RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
		}

		void OnGeometryChange(GeometryChangedEvent evt)
		{
			Debug.Log("Geometry change");
			AddScreen(MENU_SCREEN_NAME);
			AddScreen(SETTINGS_SCREEN_NAME);
			
			VisualElement mainMenuScreen = GetScreen(MENU_SCREEN_NAME);
			mainMenuScreen?.Q("StartBtn")?.RegisterCallback<ClickEvent>(ev => StartGame());
			mainMenuScreen?.Q("SettingsBtn")?.RegisterCallback<ClickEvent>(ev => EnableScreen(SETTINGS_SCREEN_NAME));
			mainMenuScreen?.Q("ExitBtn")?.RegisterCallback<ClickEvent>(ev => ExitGame());
			
			VisualElement settingsScreen = GetScreen(SETTINGS_SCREEN_NAME);
			settingsScreen?.Q("BackBtn")?.RegisterCallback<ClickEvent>(ev => EnableScreen(MENU_SCREEN_NAME));
		}

		void StartGame()
		{
			Debug.Log("Start game");
		}

		void ExitGame()
		{
			
		}
	}
}