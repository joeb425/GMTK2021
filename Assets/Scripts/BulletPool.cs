using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
	public static BulletPool Get;

	// public List<GameObject> inactiveBullets;
	public Stack<GameObject> inactiveBullets;

	[SerializeField]
	public GameObject objectToPool;

	[SerializeField]
	public int amountToPool = 150;

	[SerializeField]
	public int increments = 50;

	public void Initialize()
	{
		Get = this;

		inactiveBullets = new Stack<GameObject>();

		AddBullets(amountToPool);
	}

	private void AddBullets(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			GameObject obj = Instantiate(objectToPool);
			ReclaimToPool(obj);
		}
	}

	public void ReclaimToPool(GameObject obj)
	{
		obj.SetActive(false);
		inactiveBullets.Push(obj);
	}

	public GameObject GetInstance()
	{
		if (inactiveBullets.Count <= 0)
		{
			AddBullets(increments);
		}

		var bullet = inactiveBullets.Pop();
		bullet.SetActive(true);
		return bullet;
	}

	private void OnDestroy()
	{
		Get = null;
	}
}