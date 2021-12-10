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
		return Get().levels[GlobalData.CurrentLevel];
	}

	public bool IsLastLevel(int levelIndex)
	{
		return levelIndex >= levels.Count;
	}
}