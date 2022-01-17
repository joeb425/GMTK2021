using System;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

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
			var op = Addressables.LoadAssetAsync<VisualTreeAsset>("Assets/UI/Screens/LevelSelectItem.uxml");
			_levelSelectionItem = op.WaitForCompletion();
				
				// AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/Screens/LevelSelectItem.uxml");
			if (_levelSelectionItem == null)
			{
				Debug.LogError("LevelSelectionItem not found");
				return;
			}

			RegisterCallback<AttachToPanelEvent>(OnAttach);
		}

		private void OnAttach(AttachToPanelEvent evt)
		{
			Clear();
			GameData gameData = GameData.Get();
			int unlockedLevel = PlayerSaveManager.Get().GetData().unlockedLevelIndex;

			for (var i = 0; i < gameData.levels.Count; i++)
			{
				LevelData level = gameData.levels[i];
				TemplateContainer instance = _levelSelectionItem.CloneTree();
				instance.name = "Level" + (i + 1); 

				Button button = instance.Q<Button>();
				var levelIndex = i;
				button.RegisterCallback<ClickEvent>(ev => LoadLevel(level.level, levelIndex));
				button.text = "" + (i + 1);

				if (i <= unlockedLevel)
				{
					instance.Q("Locked").style.display = DisplayStyle.None;
				}

				Add(instance);
			}
		}

		private void LoadLevel(TextAsset level, int levelIndex)
		{
			// TODO json should just store a single level?
			// GameBoard.Levels loadedLevel = JsonUtility.FromJson<GameBoard.Levels>(level.text);
			GlobalData.CurrentLevel = levelIndex;
			// Addressables.LoadSceneAsync("Assets/Scenes/Game.unity");
			Object.FindObjectOfType<LoadScene>()?.StartLoading();
		}
	}
}