using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct InitialBullets
{
	[SerializeField]
	public Bullet bulletPrefab;

	[SerializeField]
	public int amountToPool;
}

public class BulletPool : MonoBehaviour
{
	public static BulletPool Get;

	public Dictionary<Guid, Stack<Bullet>> inactiveBullets;

	[SerializeField]
	public List<InitialBullets> initialBullets;

	[SerializeField]
	public int increments = 25;

	public void Initialize()
	{
		Get = this;

		inactiveBullets = new Dictionary<Guid, Stack<Bullet>>();

		foreach (InitialBullets initialBullet in initialBullets)
		{
			AddBullets(initialBullet.bulletPrefab, initialBullet.amountToPool);
		}
	}

	private void AddBullets(Bullet bulletPrefab, int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			Bullet obj = Instantiate(bulletPrefab);
			ReclaimToPool(obj);
		}
	}

	public void ReclaimToPool(Bullet bullet)
	{
		Guid bulletGuid = bullet.guid.GetGuid();
		if (!inactiveBullets.ContainsKey(bulletGuid))
		{
			inactiveBullets.Add(bulletGuid, new Stack<Bullet>());
			AddBullets(bullet, increments);
		}

		bullet.gameObject.SetActive(false);
		inactiveBullets[bulletGuid].Push(bullet);
	}

	public Bullet GetInstance(Bullet bulletPrefab)
	{
		if (bulletPrefab == null)
		{
			return null;
		}

		Guid bulletGuid = bulletPrefab.guid.GetGuid();
		if (!inactiveBullets.ContainsKey(bulletGuid))
		{
			inactiveBullets.Add(bulletGuid, new Stack<Bullet>());
			AddBullets(bulletPrefab, increments);
		}

		var pool = inactiveBullets[bulletGuid];
		if (pool.Count <= 0)
		{
			AddBullets(bulletPrefab, increments);
		}

		var bullet = inactiveBullets[bulletGuid].Pop();
		bullet.gameObject.SetActive(true);
		return bullet;
	}

	private void OnDestroy()
	{
		Get = null;
	}
}