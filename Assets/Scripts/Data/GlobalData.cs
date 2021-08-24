using UnityEditor;

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
}
