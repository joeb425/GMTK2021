﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HexLibrary;
using Misc;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
using Object = System.Object;

namespace Editor
{
	public class HexEditorWindow : EditorWindow
	{
		private HexLibrary.HexGrid _gridPrefab;
		private HexLibrary.HexGrid _grid;

		private HexTilePalette _tilePalette;
		private string[] _layerNames;
		private int _selectedLayerIndex = 0;

		private bool _hasGridBeenSpawned;
		private bool _isEnabled;

		private string _levelName = "level";

		[MenuItem("Window/HexEditorWindow")]
		private static void ShowWindow()
		{
			var window = GetWindow<HexEditorWindow>();
			window.titleContent = new GUIContent("Hex Editor");
			window.Show();
		}

		private void OnEnable()
		{
			EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
			_gridPrefab = AssetDatabase.LoadAssetAtPath<HexLibrary.HexGrid>("Assets/Prefabs/Hex/HexGrid.prefab");
			LoadTilePalette();

			if (_isEnabled)
			{
				EnableEditMode();
			}
		}

		private void LoadTilePalette()
		{
			_tilePalette = _gridPrefab.tilePalette;
			_layerNames = _tilePalette.GetPaletteAsDictionary().Keys.ToArray();
			_selectedLayerIndex = 0;
		}

		private void OnDestroy()
		{
			DisableEditMode();
		}

		private void OnGUI()
		{
			_isEnabled = GUILayout.Toggle(_isEnabled, "Enable edit mode");

			if (GUILayout.Button("Save Level") && _hasGridBeenSpawned)
			{
				SaveLevel();
			}

			if (GUILayout.Button("Load level") && _hasGridBeenSpawned)
			{
				LoadLevel();
			}

			if (GUILayout.Button("Reset grid") && _hasGridBeenSpawned)
			{
				ResetLevel();
			}

			EditorGUILayout.Space(20);

			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Level Name: ");
			_levelName = EditorGUILayout.TextField(_levelName);
			EditorGUILayout.EndHorizontal();

			int newIndex = EditorGUILayout.Popup(_selectedLayerIndex, _layerNames);
			if (newIndex != _selectedLayerIndex)
			{
				_selectedLayerIndex = newIndex;
			}

			_gridPrefab = (HexLibrary.HexGrid)EditorGUILayout.ObjectField(_gridPrefab, typeof(HexLibrary.HexGrid), false);

			if (GUILayout.Button("Load game data"))
			{
				LoadGoogleDocs.Load(
					"https://docs.google.com/spreadsheets/d/e/2PACX-1vTLjwrRjkQUgdmoWzjUyMSnbwqe1pX1ZXw_tQLKoRSOnTsu9Mh61Vp9kKJgtR2sKmKbN7cCy0f9VKLs/pub?gid=2074852759&single=true&output=csv",
					OnLoadedTowerData);
			}

			if (GUILayout.Button("Load gameplay tags"))
			{
				GlobalData.GetTagManager().ReadTagsGoogleDocs();
			}
		}

		private void Update()
		{
			switch (_isEnabled)
			{
				case true when !_hasGridBeenSpawned:
					_hasGridBeenSpawned = true;
					EnableEditMode();
					break;
				case false when _hasGridBeenSpawned:
					_hasGridBeenSpawned = false;
					DisableEditMode();
					break;
			}
		}

		private void EnableEditMode()
		{
			// if (_grid == null)
			{
				_grid = Instantiate(_gridPrefab);
				_grid.Init();
			}

			_isEnabled = true;
			SceneView.duringSceneGui += DuringSceneGui;
		}

		private void DisableEditMode()
		{
			_isEnabled = false;
			Debug.Log("Disable edit mode");

			if (_grid)
			{
				DestroyImmediate(_grid.gameObject);
			}

			SceneView.duringSceneGui -= DuringSceneGui;
		}

		private void DuringSceneGui(SceneView sceneView)
		{
			if (!_hasGridBeenSpawned)
			{
				return;
			}

			Event e = Event.current;

			// generate ray from mouse position
			Vector2 mousePosition = Event.current.mousePosition;
			mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
			Camera cam = sceneView.camera;
			var ray = cam.ScreenPointToRay(mousePosition);
			if (!_grid.GetHexUnderRay(ray, out Hex hex))
			{
				return;
			}

			if (e.isKey && e.type == EventType.KeyDown)
			{
				if (GetPrefabFromKey(e.keyCode, out var prefab))
				{
					GetCurrentGridLayer().AddTile(hex, Instantiate(prefab));
				}
				else if (e.keyCode == KeyCode.Q) // delete tile when pressing Q
				{
					GetCurrentGridLayer().DeleteTile(hex);
				}
			}
		}

		bool GetPrefabFromKey(KeyCode key, out HexTileComponent prefab)
		{
			List<HexTileSpawnData> spawnDatas = GetLayerPalette().spawnDatas;
			if (spawnDatas.Count == 0)
			{
				Debug.Log("spawn datas invaalid");
			}

			HexTileSpawnData spawnData = spawnDatas.Find(data => data.spawnKeyCode == key);
			if (spawnData != null)
			{
				prefab = spawnData.tilePrefab;
				return true;
			}

			prefab = null;
			return false;
		}

		private JsonHexGrid GenerateJsonGrid()
		{
			return _grid.GenerateJsonGrid();
		}

		private string GetFilePath()
		{
			return Application.dataPath + "/Levels/" + _levelName + ".txt";
		}

		private void SaveLevel()
		{
			Debug.Log("Save level " + GetFilePath());

			string data = JsonUtility.ToJson(GenerateJsonGrid());
			File.WriteAllText(GetFilePath(), data);
		}

		private void LoadLevel()
		{
			Debug.Log("Load level " + GetFilePath());

			_grid.ResetGrid();

			StreamReader reader = new StreamReader(GetFilePath());
			string json = reader.ReadToEnd();
			reader.Close();
			_grid.LoadLevelFromJson(json);
		}

		private void ResetLevel()
		{
			_grid.ResetGrid();
			LoadTilePalette();
		}

		private void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			if (state != PlayModeStateChange.ExitingEditMode)
				return;

			DisableEditMode();
		}

		public HexTileLayerPalette GetLayerPalette()
		{
			return _grid.tilePalette.GetLayerPalette(GetLayerName());
		}

		private string GetLayerName()
		{
			return _layerNames[_selectedLayerIndex];
		}

		private HexGridLayer GetCurrentGridLayer()
		{
			return _grid.GetLayer(_layerNames[_selectedLayerIndex]);
		}

		public void OnLoadedTowerData(string fileData)
		{
			Dictionary<string, TowerData> towerDatas = new Dictionary<string, TowerData>();
			foreach (TowerData towerData in CustomAssetUtils.FindAssetsByType<TowerData>())
			{
				// Debug.Log($"found tower data {towerData.towerName}");
				towerDatas.Add(towerData.towerName, towerData);
			}

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
			}
		}
	}
}