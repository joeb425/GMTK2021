using Mantis.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.MainMenu
{
	public class MainMenuManager : VisualElement
	{
		public new class UxmlFactory : UxmlFactory<MainMenuManager, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		private static string MENU_SCREEN_NAME = "MenuScreen";
		private static string SETTINGS_SCREEN_NAME = "SettingsScreen";
		private static string LEVELSELECT_SCREEN_NAME = "LevelSelectScreen";

		private ScreenSwitcher _screenSwitcher;

		public MainMenuManager()
		{
			RegisterCallback<AttachToPanelEvent>(OnAttach);
		}

		private void OnAttach(AttachToPanelEvent evt)
		{
			_screenSwitcher = this.Q<ScreenSwitcher>();
			if (_screenSwitcher == null)
			{
				return;
			}

			_screenSwitcher.ReadScreens();
			_screenSwitcher.EnableScreen(MENU_SCREEN_NAME);

			VisualElement mainMenuScreen = _screenSwitcher.GetScreen(MENU_SCREEN_NAME);
			mainMenuScreen?.Q("StartBtn").RegisterCallback<ClickEvent>(ev => _screenSwitcher.EnableScreen(LEVELSELECT_SCREEN_NAME));
			mainMenuScreen?.Q("SettingsBtn").RegisterCallback<ClickEvent>(ev => _screenSwitcher.EnableScreen(SETTINGS_SCREEN_NAME));
			mainMenuScreen?.Q("ExitBtn").RegisterCallback<ClickEvent>(ev => ExitGame());
			
			VisualElement settingsScreen = _screenSwitcher.GetScreen(SETTINGS_SCREEN_NAME);
			settingsScreen?.Q("BackBtn")?.RegisterCallback<ClickEvent>(ev => _screenSwitcher.EnableScreen(MENU_SCREEN_NAME));

			VisualElement levelSelectScreen = _screenSwitcher.GetScreen(LEVELSELECT_SCREEN_NAME);
			levelSelectScreen?.Q("BackBtn")?.RegisterCallback<ClickEvent>(ev => _screenSwitcher.EnableScreen(MENU_SCREEN_NAME));
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