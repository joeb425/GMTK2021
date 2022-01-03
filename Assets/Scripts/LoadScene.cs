using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Android;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace DefaultNamespace
{
	public class LoadScene : MonoBehaviour
	{
		[SerializeField]
		private AssetReference scene;

		private string _percentText = String.Empty;

		private AsyncOperationHandle<SceneInstance> loadOperation; 

		private void Awake()
		{
			loadOperation = Addressables.LoadSceneAsync(scene);
		}

		private void Update()
		{
			if (loadOperation.IsValid())
			{
				_percentText = "" + loadOperation.PercentComplete;
				Debug.Log($"Loading scene {loadOperation.PercentComplete}");
			}
			else
			{
				_percentText = "";
			}
		}

		void OnGUI()
		{
			GUI.Label(new Rect(100, 100, 200, 200), _percentText);
		}
	}
}