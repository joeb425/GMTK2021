using UnityEngine;
using UnityEngine.UIElements;

namespace DefaultNamespace.Data
{
	[CreateAssetMenu(menuName = "Data/AssetBindings")]
	public class AssetBindings : ScriptableObject
	{
		[SerializeField]
		public GamePrefabs gamePrefabs;

		[SerializeField]
		public LevelData levelData;
	}
}