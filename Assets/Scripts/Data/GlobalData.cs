using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

public class GlobalData
{
	private static GamePrefabs _globalPrefabs;
	public static GamePrefabs GetGamePrefabs()
	{
		if (_globalPrefabs == null)
		{
			_globalPrefabs = AssetDatabase.LoadAssetAtPath<GamePrefabs>("Assets/Data/GamePrefabs.asset");
		}
		
		return _globalPrefabs;
	}

	private static LevelData _levelData;
	public static LevelData GetLevelData()
	{
		if (_levelData == null)
		{
			_levelData = AssetDatabase.LoadAssetAtPath<LevelData>("Assets/Data/LevelData.asset");
		}
		return _levelData;
	}

	public static int CurrentLevel { get; set; } = -1;
}
