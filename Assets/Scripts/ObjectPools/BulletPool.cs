using Mantis.Engine.Misc;
using UnityEngine;

namespace ObjectPools
{
	[CreateAssetMenu(menuName = "Pool/BulletPool")]
	public class BulletPool : GenericObjectPool<Bullet>
	{
		public static BulletPool Get()
		{
			return GlobalData.GetAssetBindings().gamePrefabs.bulletPool;
		}
	}
}