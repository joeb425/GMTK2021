using System;
using DefaultNamespace.Data;
using Mantis.Engine;
using UnityEngine;
using UnityEngine.Serialization;

namespace DefaultNamespace
{
	[Serializable]
	public class PlayerSaveData
	{
		[SerializeField]
		public int unlockedLevelIndex = 0;
	}

	[CreateAssetMenu(menuName = "Data/SaveManager")]
	public class PlayerSaveManager : PersistantDataManager<PlayerSaveData>
	{
		public static PlayerSaveManager Get()
		{
			return AssetBindings.Get().playerSaveManager;
		}
		
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
			data.unlockedLevelIndex = Math.Max(data.unlockedLevelIndex, levelIndex + 1);
			SaveData();
		}
	}
}