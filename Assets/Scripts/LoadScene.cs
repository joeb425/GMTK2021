using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Android;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace DefaultNamespace
{
	public class LoadScene : MonoBehaviour
	{
		[SerializeField]
		private AssetReference scene;

		[SerializeField]
		private UIDocument loadingScreen;

		private string _percentText = String.Empty;

		private AsyncOperationHandle<SceneInstance> loadOperation;

		private bool _isLevelLoaded;
		private ProgressBar _progressLabel;
		private Label _statusLabel;

		private void Awake()
		{
			var ve = loadingScreen.rootVisualElement;
			_progressLabel = ve.Q<ProgressBar>("ProgressBar");
			_statusLabel = ve.Q<Label>("StatusLabel");
		}

		IEnumerator Start()
		{
			loadOperation = Addressables.LoadSceneAsync(scene, LoadSceneMode.Single, false);
			yield return loadOperation;

			_statusLabel.text = "Press any key...";
		}
		

		private void Update()
		{
			if (loadOperation.IsValid())
			{
				float percent = loadOperation.PercentComplete * 100.0f;
				string percentText = percent + "%";
				_percentText = "" + loadOperation.PercentComplete;
				_progressLabel.value = percent;
				_progressLabel.title = percentText;
			}
			else
			{
				_percentText = "";
			}

			if (loadOperation.IsDone && (Input.anyKeyDown || Input.touchCount > 0))
			{
				loadOperation.Result.ActivateAsync();
			}
		}

		void OnGUI()
		{
			GUI.Label(new Rect(100, 100, 200, 200), _percentText);
		}
	}
}