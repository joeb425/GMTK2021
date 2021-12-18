using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UI.MainMenu
{
	public class LevelFinishedScreen : VisualElement
	{
		private VisualTreeAsset _levelSelectionItem;

		public new class UxmlFactory : UxmlFactory<LevelFinishedScreen, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		public LevelFinishedScreen()
		{
			RegisterCallback<AttachToPanelEvent>(OnAttach);
		}

		private void OnAttach(AttachToPanelEvent evt)
		{
			Button restartBtn = this.Q<Button>("NextLevelBtn");
			if (GameData.Get().IsLastLevel(GlobalData.CurrentLevel))
			{
				restartBtn.style.display = DisplayStyle.None;
			}
			else
			{
				restartBtn?.RegisterCallback<ClickEvent>(ev => GoToNextLevel());
			}

			Button mainMenuBtn = this.Q<Button>("MainMenuBtn");
			mainMenuBtn?.RegisterCallback<ClickEvent>(ev => GoToMainMenu());
		}

		private void GoToNextLevel()
		{
			Game.Get.NextLevel();
		}

		private void GoToMainMenu()
		{
			SceneManager.LoadScene("Assets/Scenes/MainMenu.unity", LoadSceneMode.Single);
		}
	}
}