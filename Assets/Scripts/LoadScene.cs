using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Android;
using UnityEngine.InputSystem.Utilities;
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

		[SerializeField]
		private bool loadOnAwake = false;

		[SerializeField]
		private bool pressAnyKeyToContinue;

		private string _percentText = String.Empty;

		private AsyncOperationHandle<SceneInstance> loadOperation;

		private bool _isLevelLoaded;
		private ProgressBar _progressLabel;
		private Label _statusLabel;
		private VisualElement _rootVE;

		private void Awake()
		{
			_rootVE = loadingScreen.rootVisualElement;
			_rootVE.style.display = DisplayStyle.None;
			_progressLabel = _rootVE.Q<ProgressBar>("ProgressBar");
			_statusLabel = _rootVE.Q<Label>("StatusLabel");
		}

		public void Start()
		{
			if (loadOnAwake)
			{
				StartLoading();
			}
		}

		public void StartLoading()
		{
			StartCoroutine(StartLoadingScene());
		}

		public IEnumerator StartLoadingScene()
		{
			_rootVE.style.display = DisplayStyle.Flex;
			loadOperation = Addressables.LoadSceneAsync(scene, LoadSceneMode.Single, false);
			yield return loadOperation;

			if (pressAnyKeyToContinue)
			{
				_statusLabel.text = "Press any key...";
			}
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

				if (loadOperation.IsDone && (!pressAnyKeyToContinue || Input.anyKeyDown || Input.touchCount > 0))
				{
					loadOperation.Result.ActivateAsync();
				}
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