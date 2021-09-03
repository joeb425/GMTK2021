using System;
using System.Collections.Generic;
using System.IO;
using Unity.Profiling;
using UnityEditor;
using UnityEngine;

namespace DefaultNamespace.HexGridEditor
{
	[CustomEditor(typeof(HexGrid))]
	public class HexGridEditor : Editor
	{
		HexGrid grid;
		private string _levelName = "level";
		private List<GameObject> _spawnedHexes = new List<GameObject>();

		public void OnEnable()
		{
			grid = (HexGrid)target;
			SceneView.duringSceneGui += GridUpdate;
		}

		public void OnDisable()
		{
			SceneView.duringSceneGui -= GridUpdate;
		}

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Save Level"))
			{
				SaveLevel();
			}

			if (GUILayout.Button("Load Level"))
			{
				LoadLevel();
			}

			if (GUILayout.Button("Reset Level"))
			{
				ResetLevel();
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label("Level file");
			_levelName = EditorGUILayout.TextField(_levelName);
			GUILayout.EndHorizontal();

			DrawDefaultInspector();
		}

		public void GridUpdate(SceneView sceneview)
		{
			Event e = Event.current;

			Vector2 mousePosition = Event.current.mousePosition;
			mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
			Camera cam = sceneview.camera;
			var ray = cam.ScreenPointToRay(mousePosition);

			if (e.isKey && e.type == EventType.KeyDown)
			{
				GameObject objectToPlace = null;
				int index = 0;

				if (e.keyCode == KeyCode.F1)
				{
					objectToPlace = grid.buildPrefab;
					index = 0;
				}
				else if (e.keyCode == KeyCode.F2)
				{
					objectToPlace = grid.pathPrefab;
					index = 1;
				}
				else if (e.keyCode == KeyCode.F3)
				{
					objectToPlace = grid.exitPrefab;
					index = 2;
				}
				else if (e.keyCode == KeyCode.F4)
				{
					objectToPlace = grid.startPrefab;
					index = 3;
				}

				if (!objectToPlace)
				{
					return;
				}

				if (grid.gridPlane.Raycast(ray, out float dist))
				{
					Vector3 hitPoint = ray.GetPoint(dist);
					GameObject spawnedHex = Instantiate(objectToPlace, grid.transform, false);

					Point point = new Point(hitPoint.x, hitPoint.z);
					Hex hex = grid.flat.PixelToHex(point).HexRound();

					Point roundedWorld = grid.flat.HexToPixel(hex);

					spawnedHex.transform.position = new Vector3((float)roundedWorld.x, 0, (float)roundedWorld.y);

					Debug.Log(spawnedHex.transform.position);

					grid.hexGrid.Add(hex, index);

					_spawnedHexes.Add(spawnedHex);
				}
			}
		}

		public HexGridData GenerateGridData()
		{
			HexGridData gridData = new HexGridData();
			
			foreach (KeyValuePair<Hex,int> kvp in grid.hexGrid)
			{
				HexData hexData = new HexData();
				hexData.hex = kvp.Key;
				hexData.type = kvp.Value;
				gridData.hexData.Add(hexData);
			}

			return gridData;
		}

		public string GetFilePath()
		{
			return Application.dataPath + "/Levels/" + _levelName;
		}

		private void SaveLevel()
		{
			Debug.Log("Save level " + GetFilePath());

			string data = JsonUtility.ToJson(GenerateGridData());
			
			Debug.Log(data);
			// grid._hexGridData = new HexGridData();
			System.IO.File.WriteAllText(GetFilePath(), data);
		}

		private void LoadLevel()
		{
			Debug.Log("Load level " + GetFilePath());

			foreach (GameObject spawnedHex in _spawnedHexes)
			{
				DestroyImmediate(spawnedHex);
			}

			StreamReader reader = new StreamReader(GetFilePath());
			string json = reader.ReadToEnd();
			Debug.Log(json);

			HexGridData gridData = JsonUtility.FromJson<HexGridData>(json);

			reader.Close();

			foreach (HexData hexData in gridData.hexData)
			{
				Hex hex = hexData.hex;
				int hexType = hexData.type;

				GameObject objectToPlace = null;
				switch (hexType)
				{
					case 0:
					{
						objectToPlace = grid.buildPrefab;
						break;
					}
					case 1:
					{
						objectToPlace = grid.pathPrefab;
						break;
					}
				}

				if (objectToPlace == null)
				{
					continue;
				}

				GameObject spawnedHex = Instantiate(objectToPlace, grid.transform, false);
				Point point = grid.flat.HexToPixel(hex);
				spawnedHex.transform.position = new Vector3((float)point.x, 0.0f, (float)point.y);
				_spawnedHexes.Add(spawnedHex);
			}
		}

		private void ResetLevel()
		{
			grid.hexGrid = new Dictionary<Hex, int>();

			foreach (GameObject spawnedHex in _spawnedHexes)
			{
				DestroyImmediate(spawnedHex);
			}
		}
	}
}