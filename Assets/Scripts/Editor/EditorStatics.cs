using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Data;
using Mantis.AttributeSystem;
using Mantis.GameplayTags;
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

		static T CreateOrFind<T>(string filePath, Dictionary<string, T> towerDatas) where T : Object
		{
			if (towerDatas.ContainsKey(filePath))
			{
				Debug.Log($"Found: {filePath}");
				return towerDatas[filePath];
			}
			else
			{
				// string filePath = $"Assets/Data/TowerData/{fileName}.asset";
				T towerData = ScriptableObject.CreateInstance(typeof(T)) as T;
				AssetDatabase.CreateAsset(towerData, filePath);
				Debug.Log($"Created at: {filePath}");
				return towerData;
			}
		}

		static void OnLoadedTowerData(string fileData)
		{
			// Dictionary<string, TowerData> towerDatas = new Dictionary<string, TowerData>();
			// foreach (TowerData towerData in CustomAssetUtils.FindAssetsByType<TowerData>())
			// {
			// 	Debug.Log(AssetDatabase.GetAssetPath(towerData));
			// 	towerDatas.Add(towerData.name, towerData);
			// }

			Dictionary<string, TowerData> towerDatas = CustomAssetUtils.FindAssetsByTypeWithPath<TowerData>();
			Dictionary<string, GameplayAttributeDefaults> attributeDefaults = CustomAssetUtils.FindAssetsByTypeWithPath<GameplayAttributeDefaults>();

			// Dictionary<string, TowerData> towerDatas = new Dictionary<string, TowerData>();
			// foreach (TowerData towerData in CustomAssetUtils.FindAssetsByType<TowerData>())
			// {
			// 	towerDatas.Add(towerData.name, towerData);
			// }

			List<string> unusedTowerDatas = towerDatas.Keys.ToList();

			string[] rows = fileData.Split('\n');

			for (int i = 1; i < rows.Length; i++)
			{
				string row = rows[i];
				var values = row.Split(',');
				var name = values[0].Split("Tower.").Last();
				int.TryParse(values[1], out var price);
				float.TryParse(values[2], out var range);
				float.TryParse(values[3], out var damage);
				float.TryParse(values[4], out var atkspd);
				int.TryParse(values[5], out var split);
				float.TryParse(values[6], out var splash);
				float.TryParse(values[7], out var splashRadius);
				int.TryParse(values[8], out var chain);
				float.TryParse(values[9], out var chainRadius);
				float.TryParse(values[10], out var turnSpeed);
				int.TryParse(values[11], out var sell);
				var description = values[12];

				string filePath = $"Assets/Data/TowerData/TowerData_{name}.asset";
				TowerData towerData = CreateOrFind(filePath, towerDatas);

				string defaultsPath = $"Assets/Data/TowerData/AttributeDefaults_{name}.asset";
				GameplayAttributeDefaults defaults = CreateOrFind(defaultsPath, attributeDefaults);
				bool defaultsChanged =
					defaults.FindOrAdd(MyAttributes.Get().Range, range) |
					defaults.FindOrAdd(MyAttributes.Get().Damage, damage) |
					defaults.FindOrAdd(MyAttributes.Get().AttackSpeed, atkspd) |
					defaults.FindOrAdd(MyAttributes.Get().Split, split) |
					defaults.FindOrAdd(MyAttributes.Get().SplashPercent, splash) |
					defaults.FindOrAdd(MyAttributes.Get().SplashRadius, splashRadius) |
					defaults.FindOrAdd(MyAttributes.Get().Chain, chain) |
					defaults.FindOrAdd(MyAttributes.Get().ChainRadius, chainRadius) |
					defaults.FindOrAdd(MyAttributes.Get().TurnSpeed, turnSpeed);

				if (defaultsChanged)
				{
					EditorUtility.SetDirty(defaults);
					AssetDatabase.SaveAssetIfDirty(defaults);
				}

				bool hasChanged =
					UpdateValue(ref towerData.towerName, name) |
					UpdateValue(ref towerData.towerCost, price) |
					UpdateValue(ref towerData.towerSell, sell) |
					UpdateValue(ref towerData.towerDescription, description);

				if (hasChanged)
				{
					EditorUtility.SetDirty(towerData);
					AssetDatabase.SaveAssetIfDirty(towerData);
				}
			}

			// string[] allPaths = new string[unusedTowerDatas.Count];
			// for (var i = 0; i < unusedTowerDatas.Count; i++)
			// {
			// 	var fileName = unusedTowerDatas[i];
			// 	string filePath = $"Assets/Data/TowerData/{fileName}.asset";
			// 	allPaths[i] = filePath;
			// 	Debug.Log($"Delete unused {filePath}");
			// }

			// List<string> failed = new List<string>();
			// AssetDatabase.DeleteAssets(allPaths, failed);
			UpdateTowers();
		}

		public static void UpdateTowers()
		{
			// 
			var towers = CustomAssetUtils.FindMonoBehaviourByType<Tower>(new [] { "Assets/Prefabs/Towers" });
			var datas = CustomAssetUtils.FindAssetsByType<TowerData>();
			var attributeDefaults = CustomAssetUtils.FindAssetsByType<GameplayAttributeDefaults>();

			Debug.Log("Updating towers!");
			foreach (Tower tower in towers)
			{
				GameplayTag tag = tower.GetGameplayTag();
				if (!tag)
				{
					continue;
				}

				string name = tower.GetGameplayTag().name.Split("Tower.").Last();

				TowerData data = datas.First(d => d.name == $"TowerData_{name}");
				if (data == null)
				{
					Debug.LogError($"Failed to find tower data for tower {name}");
				}

				GameplayAttributeDefaults attr = attributeDefaults.First(d => d.name == $"AttributeDefaults_{name}");
				if (attr == null)
				{
					Debug.LogError($"Failed to find attribute defaults for tower {name}");
				}

				bool hasChanged =
					UpdateValue(ref tower.towerData, data) |
					UpdateValue(ref tower.GetAttributes().attributeDefaults, attr);

				if (hasChanged)
				{
					UpdateValue(ref tower.towerData, data);
					EditorUtility.SetDirty(tower);
					AssetDatabase.SaveAssetIfDirty(tower);
				}
			}
		}
		
		private static bool UpdateValue<T>(ref T value, T newValue)
		{
			if (newValue == null)
			{
				return false;
			}

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