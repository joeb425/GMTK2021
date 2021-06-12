using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
	// [SerializeField]
	// Bullet prefab = default;

	public static BulletPool SharedInstance;

	public List<GameObject> pooledObjects;

	[SerializeField]
	public GameObject objectToPool;

	[SerializeField]
	public int amountToPool = 1000;

	public void Initialize()
	{
		SharedInstance = this;

		pooledObjects = new List<GameObject>();
		for (int i = 0; i < amountToPool; i++)
		{
			GameObject obj = (GameObject) Instantiate(objectToPool);
			obj.SetActive(false);
			pooledObjects.Add(obj);
		}
	}

	public GameObject GetPooledObject()
	{
		for (int i = 0; i < pooledObjects.Count; i++)
		{
			if (!pooledObjects[i].activeInHierarchy)
			{
				return pooledObjects[i];
			}
		}

		return null;
	}
}