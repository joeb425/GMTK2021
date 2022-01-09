using UnityEngine;

namespace DefaultNamespace.Data
{
	[CreateAssetMenu(menuName = "Data/AssetBindings")]
	public class AssetBindings : ScriptableObject
	{
		public static AssetBindings Get()
		{
			return GlobalData.GetAssetBindings();
		}

		[SerializeField]
		public GameData gameData;

		[SerializeField]
		public GamePrefabs gamePrefabs;

		[SerializeField]
		public GameAssets gameAssets;

		[SerializeField]
		public PlayerSaveManager playerSaveManager;
	}
}