using System.Collections.Generic;
using System.Linq;
using Mantis.GameplayTags;
using Mantis.Utils;
using UnityEditor;
using UnityEngine;
using CustomAssetUtils = Mantis.Utils.CustomAssetUtils;

namespace Misc
{
	static class EditorStatics
	{
		[MenuItem("LinkTD/LoadTowerData")]
		static void DebugTestTagManager()
		{
			LoadGoogleDocs.Load(
				"https://docs.google.com/spreadsheets/d/e/2PACX-1vTLjwrRjkQUgdmoWzjUyMSnbwqe1pX1ZXw_tQLKoRSOnTsu9Mh61Vp9kKJgtR2sKmKbN7cCy0f9VKLs/pub?gid=0&single=true&output=csv",
				OnLoadedTowerData);
		}

		static void OnLoadedTowerData(string fileData)
		{
			Dictionary<string, TowerData> towerDatas = new Dictionary<string, TowerData>();
			foreach (TowerData towerData in CustomAssetUtils.FindAssetsByType<TowerData>())
			{
				Debug.Log(towerData.name);
				towerDatas.Add(towerData.name, towerData);
			}

			string[] rows = fileData.Split('\n');

			for (int i = 1; i < rows.Length; i++)
			{
				string row = rows[i];
				var values = row.Split(',');
				var name = values[0].Split('.').Last();
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

				string fileName = $"TowerData_{name}";
				if (towerDatas.ContainsKey(fileName))
				{
					towerData = towerDatas[fileName];
				}
				else
				{
					string filePath = $"Assets/Data/TowerData/{fileName}.asset";
					towerData = ScriptableObject.CreateInstance(typeof(TowerData)) as TowerData;
					AssetDatabase.CreateAsset(towerData, filePath);
					Debug.Log($"Create TowerData at: {filePath}");
				}

				// towerData.towerName = name;
				// towerData.towerCost = price;
				// towerData.attackRange = range;
				// towerData.damage = damage;
				// towerData.attackSpeed = atkspd;
				// towerData.split = split;
				// towerData.splash = splash;
				// EditorUtility.SetDirty(towerData);

				bool hasChanged =
					UpdateValue(ref towerData.towerName, name) ||
					UpdateValue(ref towerData.towerCost, price) ||
					UpdateValue(ref towerData.attackRange, range) ||
					UpdateValue(ref towerData.damage, damage) ||
					UpdateValue(ref towerData.attackSpeed, atkspd) ||
					UpdateValue(ref towerData.split, split) ||
					UpdateValue(ref towerData.splash, splash);

				if (hasChanged)
				{
					EditorUtility.SetDirty(towerData);
				}
			}
		}

		private static bool UpdateValue<T>(ref T value, T newValue)
		{
			if (Equals(value, newValue)) 
				return false;

			value = newValue;
			return true;

		}
	}
}