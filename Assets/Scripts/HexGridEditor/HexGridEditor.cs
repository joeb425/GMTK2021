using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
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

			if (e.isKey && e.type == EventType.KeyDown)
			{
				// generate ray from mouse position
				Vector2 mousePosition = Event.current.mousePosition;
				mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y;
				Camera cam = sceneview.camera;
				var ray = cam.ScreenPointToRay(mousePosition);
				
				HexTileSpawnInfo tileSpawnInfo = null;

				foreach (HexTileSpawnInfo spawnInfo in grid.hexTileSpawnData.spawnData)
				{
					if (spawnInfo.spawnKeyCode == e.keyCode)
					{
						tileSpawnInfo = spawnInfo;
					}
				}

				bool bIsDeletingTile = e.keyCode == KeyCode.Q; 
				
				if (tileSpawnInfo == null && !bIsDeletingTile)
				{
					return;
				}

				if (grid.gridPlane.Raycast(ray, out float dist))
				{
					Vector3 hitPoint = ray.GetPoint(dist);
					Vector2 point = new Vector2(hitPoint.x, hitPoint.z);
					Hex hex = grid.flat.PixelToHex(point).HexRound();
					
					if (bIsDeletingTile)
					{
						grid.DeleteTile(hex, true);
					}
					else
					{
						grid.AddTile(hex, tileSpawnInfo.tileType, true);
					}
				}
			}
		}

		public JsonHexGrid GenerateJsonGrid()
		{
			JsonHexGrid jsonGrid = new JsonHexGrid();

			foreach (KeyValuePair<Hex, HexGridTile> kvp in this.grid.hexGrid)
			{
				HexGridTile gridTile = kvp.Value;

				JsonHex jsonHex = new JsonHex();
				jsonHex.hex = kvp.Key;
				jsonHex.type = (int)gridTile.tileType;
				jsonGrid.hexData.Add(jsonHex);
			}

			return jsonGrid;
		}

		public string GetFilePath()
		{
			return Application.dataPath + "/Levels/" + _levelName + ".txt";
		}

		private void SaveLevel()
		{
			Debug.Log("Save level " + GetFilePath());

			string data = JsonUtility.ToJson(GenerateJsonGrid());

			Debug.Log(data);
			System.IO.File.WriteAllText(GetFilePath(), data);
		}

		private void LoadLevel()
		{
			Debug.Log("Load level " + GetFilePath());

			this.grid.DeleteAllTiles(true);

			StreamReader reader = new StreamReader(GetFilePath());
			string json = reader.ReadToEnd();
			reader.Close();
			Debug.Log(json);
			
			grid.LoadLevelFromJson(json);

			// JsonHexGrid jsonGrid = JsonUtility.FromJson<JsonHexGrid>(json);
			//
			// foreach (JsonHex jsonHex in jsonGrid.hexData)
			// {
			// 	Hex hex = jsonHex.hex;
			// 	HexTileType hexType = (HexTileType)jsonHex.type;
			// 	grid.AddTile(hex, hexType);
			// }
		}

		private void ResetLevel()
		{
			grid.InitSpawnData();
			grid.DeleteAllTiles(true);
		}
	}
}