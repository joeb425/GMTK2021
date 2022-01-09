using DefaultNamespace.Data;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class GlobalData
{
	private static AssetBindings _assetBindings;

	public static AssetBindings GetAssetBindings()
	{
		if (_assetBindings is null)
		{
			AsyncOperationHandle<AssetBindings> op = Addressables.LoadAssetAsync<AssetBindings>("Assets/Data/AssetBindings.asset");
			_assetBindings = op.WaitForCompletion();
		}

		return _assetBindings;
	}

	public static int CurrentLevel { get; set; } = 0;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void HandleBeforeSceneLoad()
	{
		GetAssetBindings().playerSaveManager.Init();
	}
}