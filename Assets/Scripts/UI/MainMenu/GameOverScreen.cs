using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
	public class GameOverScreen : VisualElement
	{
		private VisualTreeAsset _levelSelectionItem;

		public new class UxmlFactory : UxmlFactory<GameOverScreen, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		public GameOverScreen()
		{
			RegisterCallback<AttachToPanelEvent>(OnAttach);
		}

		private void OnAttach(AttachToPanelEvent evt)
		{
			Button restartBtn = this.Q<Button>("RestartBtn");
			restartBtn?.RegisterCallback<ClickEvent>(ev => RestartGame());

			Button mainMenuBtn = this.Q<Button>("MainMenuBtn");
			mainMenuBtn?.RegisterCallback<ClickEvent>(ev => GoToMainMenu());
		}

		private void RestartGame()
		{
			Game.Get.RestartGame();
		}

		private void GoToMainMenu()
		{
			SceneManager.LoadScene("Assets/Scenes/MainMenu.unity", LoadSceneMode.Single);
		}
	}
}