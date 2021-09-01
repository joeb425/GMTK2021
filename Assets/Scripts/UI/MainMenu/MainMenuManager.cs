using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
	public class MainMenuManager : ScreenSwitcher
		//ScreenSwitcher
	{
		// public string stringAttr { get; set; }
		
		public new class UxmlFactory : UxmlFactory<MainMenuManager, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
			// UxmlStringAttributeDescription m_String = new UxmlStringAttributeDescription
			// 	{ name = "string-attr", defaultValue = "default_value" };
			
			public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
			{
				base.Init(ve, bag, cc);
				
				// var ate = ve as MainMenuManager;
				//
				// ate.Clear();
				//
				// ate.stringAttr = m_String.GetValueFromBag(bag, cc);
				// ate.Add(new TextField("String") { value = ate.stringAttr });
			}
		}

		private static string MENU_SCREEN_NAME = "MenuScreen";
		private static string SETTINGS_SCREEN_NAME = "SettingsScreen";
		private static string LEVELSELECT_SCREEN_NAME = "LevelSelectScreen";

		public MainMenuManager()
		{
			RegisterCallback<AttachToPanelEvent>(OnAttach);
		}

		private void OnAttach(AttachToPanelEvent evt)
		{
			ClearScreens();
			AddScreen(MENU_SCREEN_NAME);
			AddScreen(SETTINGS_SCREEN_NAME);
			AddScreen(LEVELSELECT_SCREEN_NAME);
			
			EnableScreen(MENU_SCREEN_NAME);
			
			VisualElement mainMenuScreen = GetScreen(MENU_SCREEN_NAME);
			mainMenuScreen?.Q("StartBtn")?.RegisterCallback<ClickEvent>(ev => EnableScreen(LEVELSELECT_SCREEN_NAME));
			mainMenuScreen?.Q("SettingsBtn")?.RegisterCallback<ClickEvent>(ev => EnableScreen(SETTINGS_SCREEN_NAME));
			mainMenuScreen?.Q("ExitBtn")?.RegisterCallback<ClickEvent>(ev => ExitGame());
			
			VisualElement settingsScreen = GetScreen(SETTINGS_SCREEN_NAME);
			settingsScreen?.Q("BackBtn")?.RegisterCallback<ClickEvent>(ev => EnableScreen(MENU_SCREEN_NAME));

			VisualElement levelSelectScreen = GetScreen(LEVELSELECT_SCREEN_NAME);
			levelSelectScreen?.Q("BackBtn")?.RegisterCallback<ClickEvent>(ev => EnableScreen(MENU_SCREEN_NAME));
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