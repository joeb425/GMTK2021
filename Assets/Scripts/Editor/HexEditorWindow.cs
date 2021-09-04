using System;
using System.Collections.Generic;
using System.IO;
using DefaultNamespace.HexGridEditor;
using UnityEditor;
using UnityEngine;

namespace Editor
{
	public class HexEditorWindow : EditorWindow
	{
		private HexGrid _gridPrefab;
		private HexGrid _grid;

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
			_gridPrefab = AssetDatabase.LoadAssetAtPath<HexGrid>("Assets/Prefabs/Hex/HexGrid.prefab");
			if (_gridPrefab == null)
			{
				Debug.Log("failed to load prefab");
			}
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

			_gridPrefab = (HexGrid)EditorGUILayout.ObjectField(_gridPrefab, typeof(HexGrid), false);
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
			_isEnabled = true;
			Debug.Log("Enable edit mode");
			_grid = Instantiate(_gridPrefab);
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

			if (e.isKey && e.type == EventType.KeyDown)
			{
				// generate ray from mouse position
				Vector2 mousePosition = Event.current.mousePosition;
				mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
				Camera cam = sceneView.camera;
				var ray = cam.ScreenPointToRay(mousePosition);

				HexTileSpawnInfo tileSpawnInfo = null;

				foreach (var spawnInfo in _grid.hexTileSpawnData.spawnData)
				{
					if (spawnInfo.spawnKeyCode == e.keyCode)
					{
						tileSpawnInfo = spawnInfo;
						break;
					}
				}

				bool bIsDeletingTile = e.keyCode == KeyCode.Q;

				if (tileSpawnInfo == null && !bIsDeletingTile)
				{
					return;
				}

				if (_grid.gridPlane.Raycast(ray, out float dist))
				{
					Vector3 hitPoint = ray.GetPoint(dist);
					Vector2 point = new Vector2(hitPoint.x, hitPoint.z);
					Hex hex = _grid.flat.PixelToHex(point).HexRound();

					if (bIsDeletingTile)
					{
						_grid.DeleteTile(hex);
					}
					else
					{
						_grid.AddTile(hex, tileSpawnInfo.tileType);
					}
				}
			}
		}

		private JsonHexGrid GenerateJsonGrid()
		{
			JsonHexGrid jsonGrid = new JsonHexGrid();

			foreach (KeyValuePair<Hex, HexGridTile> kvp in _grid.hexGrid)
			{
				HexGridTile gridTile = kvp.Value;

				JsonHex jsonHex = new JsonHex
				{
					hex = kvp.Key,
					type = gridTile.tileType
				};

				jsonGrid.hexData.Add(jsonHex);
			}

			return jsonGrid;
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

			_grid.DeleteAllTiles();

			StreamReader reader = new StreamReader(GetFilePath());
			string json = reader.ReadToEnd();
			reader.Close();
			_grid.LoadLevelFromJson(json);
		}

		private void ResetLevel()
		{
			_grid.DeleteAllTiles();
		}

		private void OnPlayModeStateChanged(PlayModeStateChange state)
		{
			if (state != PlayModeStateChange.ExitingEditMode)
				return;

			DisableEditMode();
		}
	}
}