using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

namespace UI.MainMenu
{
	public class LevelSelectionPanel : VisualElement
	{
		private VisualTreeAsset _levelSelectionItem;

		public new class UxmlFactory : UxmlFactory<LevelSelectionPanel, UxmlTraits>
		{
		}

		public new class UxmlTraits : VisualElement.UxmlTraits
		{
		}

		public LevelSelectionPanel()
		{
			_levelSelectionItem =
				AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Screens/LevelSelectItem.uxml");
			if (_levelSelectionItem == null)
			{
				Debug.LogError("LevelSelectionItem not found");
				return;
			}

			RegisterCallback<AttachToPanelEvent>(OnAttach);
		}

		private void OnAttach(AttachToPanelEvent evt)
		{
			LevelData levelData = GlobalData.GetLevelData();
			for (var i = 0; i < levelData.levels.Count; i++)
			{
				TextAsset level = levelData.levels[i];
				TemplateContainer instance = _levelSelectionItem.CloneTree();

				Button button = instance.Q<Button>();
				var levelIndex = i;
				button.RegisterCallback<ClickEvent>(ev => LoadLevel(level, levelIndex));
				button.text = "Level " + i;

				Add(button);
			}
		}

		private void LoadLevel(TextAsset level, int levelIndex)
		{
			// TODO json should just store a single level?
			// GameBoard.Levels loadedLevel = JsonUtility.FromJson<GameBoard.Levels>(level.text);
			GlobalData.CurrentLevel = levelIndex;
			SceneManager.LoadScene("Assets/Scenes/Game.unity");
		}
	}
}