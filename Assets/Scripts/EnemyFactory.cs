using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnemyFactory : GameObjectFactory
{
	[SerializeField] List<Enemy> prefab = default;
	public Enemy Get(int enemyElementNo)
	{
		Enemy instance = CreateGameObjectInstance(prefab[enemyElementNo]);
		instance.OriginFactory = this;
		return instance;
	}

	public void Reclaim(Enemy enemy)
	{
		Debug.Assert(enemy.OriginFactory == this, "Wrong factory reclaimed!");
		Destroy(enemy.gameObject);
	}
}