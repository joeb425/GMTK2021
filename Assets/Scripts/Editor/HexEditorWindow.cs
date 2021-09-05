using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HexLibrary;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

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

		bool GetPrefabFromKey(KeyCode key, out GameObject prefab)
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
	}
}