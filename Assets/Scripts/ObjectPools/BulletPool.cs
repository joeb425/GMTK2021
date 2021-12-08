using Mantis.Engine.Misc;
using UnityEngine;

namespace ObjectPools
{
	[CreateAssetMenu(menuName = "Pool/BulletPool")]
	public class BulletPool : GenericObjectPool<Bullet>
	{
	}
}