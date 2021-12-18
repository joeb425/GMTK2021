using System;
using System.Collections.Generic;
using DefaultNamespace.Data;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/GameData")]
public class GameData : ScriptableObject
{
	public static GameData Get()
	{
		return GlobalData.GetAssetBindings().gameData;
	}

	[SerializeField]
	public List<LevelData> levels;

	public LevelData GetCurrentLevel()
	{
		int levelIndex = Math.Min(GlobalData.CurrentLevel, levels.Count - 1); 
		return levels[levelIndex];
	}

	public bool IsLastLevel(int levelIndex)
	{
		return levelIndex >= levels.Count - 1;
	}
}