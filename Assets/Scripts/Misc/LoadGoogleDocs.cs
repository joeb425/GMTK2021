using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Misc
{
	
	
	public class LoadGoogleDocs : MonoBehaviour
	{
		public void Load()
		{
			StartCoroutine(LoadGoogleDoc(gameObject));
		}

		private IEnumerator LoadGoogleDoc(GameObject killMe)
		{
			UnityWebRequest www =
				UnityWebRequest.Get(
					"https://docs.google.com/spreadsheets/d/e/2PACX-1vTLjwrRjkQUgdmoWzjUyMSnbwqe1pX1ZXw_tQLKoRSOnTsu9Mh61Vp9kKJgtR2sKmKbN7cCy0f9VKLs/pub?output=csv");

			yield return www.SendWebRequest();

			if (www.result != UnityWebRequest.Result.Success)
			{
				Debug.Log(www.error);
			}
			else
			{
				// Show results as text
				Debug.Log(www.downloadHandler.text);
				//
				// // Or retrieve results as binary data
				// byte[] results = www.downloadHandler.data;
			}

			Dictionary<string, TowerData> towerDatas = new Dictionary<string, TowerData>();
			foreach (TowerData towerData in FindAssetsByType<TowerData>())
			{
				// Debug.Log($"found tower data {towerData.towerName}");
				towerDatas.Add(towerData.towerName, towerData);
			}
			
			
			string fileData = www.downloadHandler.text;
			string[] rows = fileData.Split('\n');

			for (int i = 1; i < rows.Length; i++)
			{
				string row = rows[i];
				var values = row.Split(',');
				var name = values[0];
				int.TryParse(values[1], out var price);
				float.TryParse(values[2], out var range);
				float.TryParse(values[3], out var damage);
				float.TryParse(values[4], out var atkspd);
				int.TryParse(values[5], out var split);
				float.TryParse(values[6], out var splash);
				float.TryParse(values[7], out var dot);
				float.TryParse(values[8], out var slow);
				float.TryParse(values[9], out var stun);

				TowerData towerData = null;

				if (towerDatas.ContainsKey(name))
				{
					towerData = towerDatas[name];
				}
				else
				{
					towerData = ScriptableObject.CreateInstance(typeof(TowerData)) as TowerData;
					AssetDatabase.CreateAsset(towerData, $"Assets/Data/TowerData/TowerData_{name}.asset");
				}

				towerData.towerName = name;
				towerData.towerCost = price;
				towerData.attackRange = range;
				towerData.damage = damage;
				towerData.attackSpeed = atkspd;
				towerData.split = split;
				towerData.splash = splash;
				EditorUtility.SetDirty(towerData);

				
				// foreach (string value in values)
				// {
				// 	Debug.Log(value);
				// }
			}

			DestroyImmediate(killMe);
		}
		
		public static List<T> FindAssetsByType<T>() where T : UnityEngine.Object
		{
			List<T> assets = new List<T>();
			string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(T)));
			for (int i = 0; i < guids.Length; i++)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
				T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
				if (asset != null)
				{
					assets.Add(asset);
				}
			}

			return assets;
		}
	}
}