using DefaultNamespace.Data;
using Mantis.GameplayTags;
using UnityEditor;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GlobalData
{
	private static AssetBindings _assetBindings;

	public static AssetBindings GetAssetBindings()
	{
		if (_assetBindings is null)
		{
// #if UNITY_EDITOR
			// _assetBindings = AssetDatabase.LoadAssetAtPath<AssetBindings>("Assets/Data/AssetBindings.asset");
// #else
			AsyncOperationHandle<AssetBindings> op = Addressables.LoadAssetAsync<AssetBindings>("Assets/Data/AssetBindings.asset");
			_assetBindings = op.WaitForCompletion();
// #endif
		}

		return _assetBindings;
	}

	public static int CurrentLevel { get; set; } = 0;
}