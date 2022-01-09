using System;
using Mantis.Engine;
using UnityEngine;

namespace DefaultNamespace
{
	[Serializable]
	public class PlayerSaveData
	{
		[SerializeField]
		public int currentLevel = 0;
	}

	[CreateAssetMenu(menuName = "Data/SaveManager")]
	public class PlayerSaveManager : PersistantDataManager<PlayerSaveData>
	{
		public void Init()
		{
			LoadData();
			EngineStatics.OnGameInit += BindToGameState;
		}

		public void BindToGameState()
		{
			GameState.Get().OnLevelFinished += () => FinishLevel(GlobalData.CurrentLevel);
			SaveData();
		}

		public void FinishLevel(int levelIndex)
		{
			data.currentLevel = Math.Max(data.currentLevel, levelIndex + 1);
			SaveData();
		}
	}
}