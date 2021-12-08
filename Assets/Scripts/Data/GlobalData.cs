using DefaultNamespace.Data;
using Mantis.GameplayTags;
using UnityEditor;

public class GlobalData
{
	private static AssetBindings _assetBindings;
	public static AssetBindings GetAssetBindings()
	{
		if (_assetBindings is null)
		{
			_assetBindings = AssetDatabase.LoadAssetAtPath<AssetBindings>("Assets/Data/AssetBindings.asset");
		}

		return _assetBindings;
	}

	public static int CurrentLevel { get; set; } = 0;
}
