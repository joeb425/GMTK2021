using DefaultNamespace.Data;
using JetBrains.Annotations;
using Misc.GameplayTags;
using UnityEditor;
using UnityEngine;

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

	private static GameplayTagManager _tagManager;
	public static GameplayTagManager GetTagManager()
	{
		return _tagManager ??= new GameplayTagManager();
	}

	public static int CurrentLevel { get; set; } = 0;


	public static System.Action OnGameInit;

	public static void Clear()
	{
		OnGameInit = null;
	}
}
