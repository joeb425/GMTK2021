using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Misc
{
	public class LoadGoogleDocs : MonoBehaviour
	{
		public System.Action<string> OnLoaded;

		public static LoadGoogleDocs Load(string docLink, System.Action<string> OnLoad)
		{
			GameObject gameObject = new GameObject();
			LoadGoogleDocs loadGoogleDocs = gameObject.AddComponent<LoadGoogleDocs>();
			loadGoogleDocs.OnLoaded = OnLoad;
			loadGoogleDocs.Load(docLink);
			return loadGoogleDocs;
		}

		private void Load(string docLink)
		{
			StartCoroutine(LoadGoogleDoc(docLink));
		}

		private IEnumerator LoadGoogleDoc(string docLink)
		{
			UnityWebRequest www = UnityWebRequest.Get(docLink);

			yield return www.SendWebRequest();

			if (www.result != UnityWebRequest.Result.Success)
			{
				Debug.Log(www.error);
			}
			else
			{
				// Show results as text
				Debug.Log(www.downloadHandler.text);
				OnLoaded?.Invoke(www.downloadHandler.text);
			}

			DestroyImmediate(gameObject);
		}
	}
}