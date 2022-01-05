using System.Collections.Generic;
using System.Linq;
using Mantis.Utils;
using Mantis.Utils.Editor;
using UnityEditor;
using UnityEngine;

namespace Mantis.LinkTD.Editor.Editor
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
				towerDatas.Add(towerData.name, towerData);
			}

			List<string> unusedTowerDatas = towerDatas.Keys.ToList();

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

				unusedTowerDatas.Remove(fileName);

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
					Debug.Log($"{towerData.name} changed");
					EditorUtility.SetDirty(towerData);
					AssetDatabase.SaveAssetIfDirty(towerData);
				}
			}

			string[] allPaths = new string[unusedTowerDatas.Count];
			for (var i = 0; i < unusedTowerDatas.Count; i++)
			{
				var fileName = unusedTowerDatas[i];
				string filePath = $"Assets/Data/TowerData/{fileName}.asset";
				allPaths[i] = filePath;
				Debug.Log($"Delete unused {filePath}");
			}

			List<string> failed = new List<string>();
			AssetDatabase.DeleteAssets(allPaths, failed);
		}

		private static bool UpdateValue<T>(ref T value, T newValue)
		{
			if (Equals(value, newValue))
				return false;

			value = newValue;
			return true;
		}

		[MenuItem("LinkTD/LoadEnemyData")]
		static void LoadEnemyData()
		{
			LoadGoogleDocs.Load(
				"https://docs.google.com/spreadsheets/d/e/2PACX-1vTLjwrRjkQUgdmoWzjUyMSnbwqe1pX1ZXw_tQLKoRSOnTsu9Mh61Vp9kKJgtR2sKmKbN7cCy0f9VKLs/pub?gid=2004669391&single=true&output=csv",
				OnLoadedEnemyData);
		}

		static void OnLoadedEnemyData(string fileData)
		{
			Dictionary<string, EnemyData> towerDatas = new Dictionary<string, EnemyData>();
			foreach (EnemyData enemyData in CustomAssetUtils.FindAssetsByType<EnemyData>())
			{
				towerDatas.Add(enemyData.name, enemyData);
			}

			List<string> unusedTowerDatas = towerDatas.Keys.ToList();
			string[] rows = fileData.Split('\n');

			for (int i = 1; i < rows.Length; i++)
			{
				string row = rows[i];
				var values = row.Split(',');
				var name = values[0].Split('.').Last();
				int.TryParse(values[1], out var health);
				float.TryParse(values[2], out var speed);
				float.TryParse(values[3], out var block);

				EnemyData enemyData = null;

				string fileName = $"EnemyData_{name}";

				unusedTowerDatas.Remove(fileName);

				if (towerDatas.ContainsKey(fileName))
				{
					enemyData = towerDatas[fileName];
				}
				else
				{
					string filePath = $"Assets/Data/EnemyData/{fileName}.asset";
					enemyData = ScriptableObject.CreateInstance(typeof(EnemyData)) as EnemyData;
					AssetDatabase.CreateAsset(enemyData, filePath);
					Debug.Log($"Create TowerData at: {filePath}");
				}

				bool hasChanged =
					UpdateValue(ref enemyData.health, health) ||
					UpdateValue(ref enemyData.speed, speed) ||
					UpdateValue(ref enemyData.block, block);

				if (hasChanged)
				{
					Debug.Log($"{enemyData.name} changed");
					EditorUtility.SetDirty(enemyData);
					AssetDatabase.SaveAssetIfDirty(enemyData);
				}
			}

			string[] allPaths = new string[unusedTowerDatas.Count];
			for (var i = 0; i < unusedTowerDatas.Count; i++)
			{
				var fileName = unusedTowerDatas[i];
				string filePath = $"Assets/Data/EnemyData/{fileName}.asset";
				allPaths[i] = filePath;
				Debug.Log($"Delete unused {filePath}");
			}

			List<string> failed = new List<string>();
			AssetDatabase.DeleteAssets(allPaths, failed);
		}
	}
}