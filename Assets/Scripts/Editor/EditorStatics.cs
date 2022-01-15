using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		static T CreateOrFind<T>(string filePath, Dictionary<string, T> towerDatas) where T : ScriptableObject
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

		static T CreateOrFindPrefab<T>(string filePath, GameObject prefab) where T : MonoBehaviour
		{
			GameObject existingObj = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
			if (existingObj != null)
			{
				Debug.Log($"Found: {filePath}");
				return existingObj.GetComponent<T>();
			}
			// if (assets.ContainsKey(filePath))
			// {
			// 	Debug.Log($"Found: {filePath}");
			// 	return assets[filePath];
			// }
			else
			{
				GameObject objSource = (GameObject) PrefabUtility.InstantiatePrefab(prefab);
				GameObject obj = PrefabUtility.SaveAsPrefabAsset(objSource, filePath);
				Object.DestroyImmediate(objSource);
				Debug.Log($"Created prefab at: {filePath}");
				EditorUtility.SetDirty(obj);
				AssetDatabase.SaveAssetIfDirty(obj);
				return obj.GetComponent<T>();
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

			GameObject baseTowerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Towers/BaseTower.prefab");

			AssetBindings.Get().gamePrefabs.baseTowers = new List<Tower>();

			// Dictionary<string, TowerData> towerDatas = new Dictionary<string, TowerData>();
			// foreach (TowerData towerData in CustomAssetUtils.FindAssetsByType<TowerData>())
			// {
			// 	towerDatas.Add(towerData.name, towerData);
			// }

			List<string> unusedTowerDatas = towerDatas.Keys.ToList();

			string[] rows = fileData.Split('\n');

			const string rootPath = "Assets/Game/Towers";

			List<Tower> allTowers = new List<Tower>();
			for (int i = 1; i < rows.Length; i++)
			{
				string row = rows[i];
				var values = row.Split(',');
				string towerTagString = values[0];
				string towerID = towerTagString.Split("Tower.").Last();
				string towerName = values[1];
				int.TryParse(values[2], out var price);
				int.TryParse(values[3], out var sell);
				float.TryParse(values[4], out var damage);
				float.TryParse(values[5], out var atkspd);
				float.TryParse(values[6], out var range);
				int.TryParse(values[7], out var split);
				float.TryParse(values[8], out var splash);
				float.TryParse(values[9], out var splashRadius);
				int.TryParse(values[10], out var chain);
				float.TryParse(values[11], out var chainRadius);
				float.TryParse(values[12], out var turnSpeed);
				string description = values[13];

				// string filePath = $"Assets/Data/TowerData/TowerData_{towerID}.asset";
				string filePath = $"{rootPath}/TowerData_{towerID}.asset";
				TowerData towerData = CreateOrFind(filePath, towerDatas);

				// string defaultsPath = $"Assets/Data/TowerData/AttributeDefaults_{towerID}.asset";
				string defaultsPath = $"{rootPath}/AttributeDefaults_{towerID}.asset";
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
					UpdateValue(ref towerData.towerName, towerName) |
					UpdateValue(ref towerData.towerCost, price) |
					UpdateValue(ref towerData.towerSell, sell) |
					UpdateValue(ref towerData.towerDescription, description);

				if (hasChanged)
				{
					EditorUtility.SetDirty(towerData);
					AssetDatabase.SaveAssetIfDirty(towerData);
				}

				StringBuilder rootID = new StringBuilder(towerID);
				rootID[^1] = '1';

				GameObject towerTypeRoot = AssetDatabase.LoadAssetAtPath<GameObject>($"{rootPath}/Tower_{rootID}.prefab");
				GameObject parentPrefab = towerTypeRoot != null ? towerTypeRoot : baseTowerPrefab;
				if (parentPrefab != null)
				{
					Tower tower = CreateOrFindPrefab<Tower>($"{rootPath}/Tower_{towerID}.prefab", parentPrefab);
					GameplayTagManager.RequestTag(towerTagString, out GameplayTag towerTag);
					UpdateValue(ref tower.gameplayTag, towerTag);
					UpdateValue(ref tower.GetAttributes().attributeDefaults, defaults);
					UpdateValue(ref tower.towerData, towerData);
					EditorUtility.SetDirty(tower);
					AssetDatabase.SaveAssetIfDirty(tower);

					bool isBaseTower = towerID.Substring(2) == "1.1";
					if (isBaseTower)
					{
						AssetBindings.Get().gamePrefabs.baseTowers.Add(tower);
					}

					allTowers.Add(tower);
				}
				else
				{
					Debug.LogError("parent tower prefab null");
				}
			}

			EditorUtility.SetDirty(AssetBindings.Get().gamePrefabs);
			AssetDatabase.SaveAssetIfDirty(AssetBindings.Get().gamePrefabs);

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

			UpdateTowerUpgradePaths(rootPath, allTowers);
		}

		public static void UpdateTowerUpgradePaths(string rootPath, List<Tower> allTowers)
		{
			Dictionary<string, Tower> towersByName = new Dictionary<string, Tower>();
			foreach (Tower t in allTowers)
			{
				towersByName.Add(t.name, t);
			}
			
			foreach (Tower t in allTowers)
			{
				t.towerData.upgradePaths = new List<UpgradePath>();
				string[] towerID =  t.GetGameplayTag().name.Split(".");
				int baseType = int.Parse(towerID[^3]);
				int variant = int.Parse(towerID[^2]);
				int upgrade = int.Parse(towerID[^1]);

				List<string> towerNames = new List<string>();
				if (variant == 1)
				{
					towerNames.Add($"Tower_{baseType}.{variant + 1}.1");
					towerNames.Add($"Tower_{baseType}.{variant + 2}.1");
				}
				else
				{
					towerNames.Add($"Tower_{baseType}.{variant}.{upgrade + 1}");
				}

				foreach (string towerName in towerNames)
				{
					if (towersByName.TryGetValue(towerName, out var upgradeTower))
					{
						UpgradePath path = new UpgradePath();
						path.upgradeCost = upgradeTower.towerData.towerCost;
						path.tower = upgradeTower;
						t.towerData.upgradePaths.Add(path);
					}
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