using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace DefaultNamespace.HexGridEditor
{
	[Serializable]
	public enum HexTileType
	{
		Build = 0,
		Path = 1,
		Start = 2,
		End = 3,
	}

	// [ExecuteInEditMode]
	public class HexGrid : MonoBehaviour
	{
		[SerializeField]
		public HexTileSpawnData hexTileSpawnData;

		[SerializeField]
		TextAsset levelToLoad;

		public HexPathFinding PathFinder;

		public Layout flat;

		public Dictionary<Hex, HexGridTile> hexGrid = new Dictionary<Hex, HexGridTile>();

		public Plane gridPlane = new Plane(Vector3.up, Vector3.zero);

		public HexGrid()
		{
			flat = new Layout(Layout.flat, new Vector2(1.0f, 1.0f), Vector2.zero);
		}

		public void Awake()
		{
			if (Application.isPlaying)
			{
				LoadLevel();
				PathFinder.findPath(hexGrid, flat);
			}
		}

		[CanBeNull]
		public HexGridTile GetTile(Hex hexCoord)
		{
			return hexGrid.ContainsKey(hexCoord) ? hexGrid[hexCoord] : null;
		}

		public void AddTile(Hex hexCoord, HexTileType type)
		{
			DeleteTile(hexCoord);
			
			HexTileSpawnInfo tileSpawnInfo = GetSpawnInfoFromType(type);

			GameObject spawnedHex = Instantiate(tileSpawnInfo.tilePrefab, transform, false);

			Vector2 hexToPixel = flat.HexToPixel(hexCoord);
			Vector3 worldPos = new Vector3(hexToPixel.x, 0.0f, hexToPixel.y);
			spawnedHex.transform.position = worldPos;

			HexGridTile hexGridTile = new HexGridTile();
			hexGridTile.spawnedTile = spawnedHex;
			hexGridTile.tileType = tileSpawnInfo.tileType;
			hexGrid.Add(hexCoord, hexGridTile);
		}

		public HexTileSpawnInfo GetSpawnInfoFromType(HexTileType tileType)
		{
			return hexTileSpawnData.spawnData.FirstOrDefault(spawnInfo => spawnInfo.tileType == tileType);
		}

		public void DeleteTile(Hex hexCoord)
		{
			if (!hexGrid.ContainsKey(hexCoord)) 
				return;
			
			if (Application.isPlaying)
			{
				Destroy(hexGrid[hexCoord].spawnedTile);
			}
			else
			{
				DestroyImmediate(hexGrid[hexCoord].spawnedTile);
			}

			hexGrid.Remove(hexCoord);
		}

		public void DeleteAllTiles()
		{
			int numChildren = transform.childCount;
			GameObject[] childrenToDestroy = new GameObject[numChildren];
			for (int i = 0; i < numChildren; ++i)
			{
				childrenToDestroy[i] = transform.GetChild(i).gameObject;
			}
			
			foreach (GameObject child in childrenToDestroy)
			{
				if (Application.isPlaying)
				{
					Destroy(child);
				}
				else
				{
					DestroyImmediate(child);
				}
			}

			if (hexGrid != null)
			{
				foreach (KeyValuePair<Hex, HexGridTile> kvp in hexGrid)
				{
					if (Application.isPlaying)
					{
						Destroy(kvp.Value.spawnedTile);
					}
					else
					{
						DestroyImmediate(kvp.Value.spawnedTile);
					}
				}
			}

			hexGrid = new Dictionary<Hex, HexGridTile>();
		}

		public void LoadLevel(bool isInEditor = false)
		{
			Debug.Log("Load level!");
			LoadLevelFromJson(levelToLoad.text);
		}

		public void LoadLevelFromJson(string json)
		{
			DeleteAllTiles();
			JsonHexGrid jsonGrid = JsonUtility.FromJson<JsonHexGrid>(json);
			foreach (JsonHex jsonHex in jsonGrid.hexData)
			{
				Hex hex = jsonHex.hex;
				HexTileType hexType = (HexTileType)jsonHex.type;
				AddTile(hex, hexType);
			}
		}

		private void Update()
		{
			// Debug.Log("updating");
			//
			// var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			//
			// if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
			// {
			// 	Debug.Log(hit.point);
			// 	Point point = new Point(hit.point.x, hit.point.z);
			// 	Hex hex = flat.PixelToHex(point).HexRound();
			// 	Debug.Log(hex);
			// 	Point roundedWorld = flat.HexToPixel(hex);
			// 	_spawneHex.transform.position = new Vector3((float)roundedWorld.x, 0, (float)roundedWorld.y);
			// }
		}

		void OnDrawGizmos()
		{
			Gizmos.DrawSphere(Vector3.zero, 0.5f);
			// TODO draw hex grid

			// Vector3 pos = Camera.current.transform.position;
			// for (float y = pos.y - 800.0f; y < pos.y + 800.0f; y += height)
			// {
			// 	Gizmos.DrawLine(
			// 		new Vector3(-1000000.0f, 0.0f, Mathf.Floor(y / height) * height),
			// 		new Vector3(1000000.0f, 0.0f, Mathf.Floor(y / height) * height));
			// }
			//
			// for (float x = pos.x - 1200.0f; x < pos.x + 1200.0f; x += width)
			// {
			// 	Gizmos.DrawLine(
			// 		new Vector3(Mathf.Floor(x / width) * width, 0.0f, -1000000.0f),
			// 		new Vector3(Mathf.Floor(x / width) * width, 0.0f, 1000000.0f));
			// }
		}
	}
}