using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class GenericObjectPool
{
	[SerializeField]
	public GameObject objectToPool;

	[SerializeField]
	public int amountToPool = 1000;

	private List<GameObject> pooledObjects;

	public void Initialize()
	{
		pooledObjects = new List<GameObject>();

		for (int i = 0; i < amountToPool; i++)
		{
			GameObject obj = Object.Instantiate(objectToPool);
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