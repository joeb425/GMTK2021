using UnityEngine;

namespace DefaultNamespace
{
	public class GlobalHelpers
	{
		public static void DeleteAllChildren(GameObject gameObject)
		{
			int numChildren = gameObject.transform.childCount;

			GameObject[] childrenToDestroy = new GameObject[numChildren];

			for (int i = 0; i < numChildren; ++i)
			{
				childrenToDestroy[i] = gameObject.transform.GetChild(i).gameObject;
			}

			foreach (GameObject child in childrenToDestroy)
			{
				DestroySafely(child);
			}
		}

		public static void DestroySafely(GameObject gameObject)
		{
			if (Application.isPlaying)
			{
				Object.Destroy(gameObject);
			}
			else
			{
				Object.DestroyImmediate(gameObject);
			}
		}
	}
}