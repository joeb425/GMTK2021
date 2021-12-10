using Mantis.Engine.Misc;
using UnityEngine;

namespace ObjectPools
{
	[CreateAssetMenu(menuName = "Pool/EnemyPool")]
	public class EnemyPool : GenericObjectPool<Enemy>
	{
		public static EnemyPool Get()
		{
			return GlobalData.GetAssetBindings().gamePrefabs.enemyPool;
		}
	}
}