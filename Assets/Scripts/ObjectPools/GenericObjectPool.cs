using System;
using System.Collections.Generic;
using GameplayTags;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ObjectPools
{
	[Serializable]
	public class GenericObjectPool<T> : ScriptableObject where T : MonoBehaviour, IGameplayTag
	{
		[Serializable]
		public struct InitalPoolObjects
		{
			[SerializeField]
			public T prefab;

			[SerializeField]
			public int amountToPool;
		};

		private Dictionary<GameplayTag, Stack<T>> _inactiveObjects = new Dictionary<GameplayTag, Stack<T>>();

		[SerializeField]
		public List<InitalPoolObjects> initialObjects = new List<InitalPoolObjects>();

		[SerializeField]
		public int increments = 25;

		private void Awake()
		{
			Debug.Log("Object pool Awake");
		}

		private void OnEnable()
		{
			Debug.Log("Object pool enable");
		}

		public void Initialize()
		{
			_inactiveObjects = new Dictionary<GameplayTag, Stack<T>>();

			foreach (InitalPoolObjects initialT in initialObjects)
			{
				AddObject(initialT.prefab, initialT.amountToPool);
			}
		}

		public void AddObject(T prefab, int amount)
		{
			for (int i = 0; i < amount; i++)
			{
				T obj = Object.Instantiate(prefab);
				ReclaimToPool(obj);
			}
		}

		public void ReclaimToPool(T obj)
		{
			GameplayTag gameplayTag = obj.GetGameplayTag();
			if (!_inactiveObjects.ContainsKey(gameplayTag))
			{
				_inactiveObjects.Add(gameplayTag, new Stack<T>());
				AddObject(obj, increments);
			}

			obj.gameObject.SetActive(false);
			_inactiveObjects[gameplayTag].Push(obj);
		}

		public T GetInstance(T prefab)
		{
			if (prefab == null)
			{
				return null;
			}

			GameplayTag gameplayTag = prefab.GetGameplayTag();
			if (gameplayTag is null)
			{
				Debug.Log($"{prefab} has null gameplay tag");
				return null;
			}

			if (!_inactiveObjects.ContainsKey(gameplayTag))
			{
				_inactiveObjects.Add(gameplayTag, new Stack<T>());
				AddObject(prefab, increments);
			}

			var pool = _inactiveObjects[gameplayTag];
			if (pool.Count <= 0)
			{
				AddObject(prefab, increments);
			}

			var T = _inactiveObjects[gameplayTag].Pop();
			T.gameObject.SetActive(true);
			return T;
		}
	}
}