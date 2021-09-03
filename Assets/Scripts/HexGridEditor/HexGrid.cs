using System;
using System.Collections.Generic;
using System.IO.Compression;
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

	[ExecuteInEditMode]
	public class HexGrid : MonoBehaviour
	{
		public float width = 32.0f;
		public float height = 32.0f;

		[SerializeField]
		public GameObject buildPrefab;

		[SerializeField]
		public GameObject pathPrefab;

		[SerializeField]
		public GameObject exitPrefab;

		[SerializeField]
		public GameObject startPrefab;

		[SerializeField]
		public HexTileSpawnData hexTileSpawnData;

		public Dictionary<HexTileType, HexTileSpawnInfo> spawnDataByType = new Dictionary<HexTileType, HexTileSpawnInfo>();

		public Layout flat;

		public Dictionary<Hex, HexGridTile> hexGrid = new Dictionary<Hex, HexGridTile>();

		[SerializeField]
		public Plane gridPlane = new Plane(Vector3.up, Vector3.zero);

		public HexGrid()
		{
			flat = new Layout(Layout.flat, new Point(1.0, 1.0), new Point(0.0, 0.0));
		}

		public void InitSpawnData()
		{
			spawnDataByType = new Dictionary<HexTileType, HexTileSpawnInfo>();
			foreach (HexTileSpawnInfo spawnInfo in hexTileSpawnData.spawnData)
			{
				spawnDataByType.Add(spawnInfo.tileType, spawnInfo);
			}
		}

		private void Start()
		{
			InitSpawnData();
		}

		[CanBeNull]
		public HexGridTile GetTile(Hex hexCoord)
		{
			return hexGrid.ContainsKey(hexCoord) ? hexGrid[hexCoord] : null;
		}

		public void AddTile(Hex hexCoord, HexTileType type, bool isInEditor = false)
		{
			DeleteTile(hexCoord, isInEditor);
			
			HexTileSpawnInfo tileSpawnInfo = spawnDataByType[type];

			GameObject spawnedHex = Instantiate(tileSpawnInfo.tilePrefab, transform, false);

			Point hexToPixel = flat.HexToPixel(hexCoord);
			Vector3 worldPos = new Vector3((float)hexToPixel.x, 0.0f, (float)hexToPixel.y);
			spawnedHex.transform.position = worldPos;

			HexGridTile hexGridTile = new HexGridTile();
			hexGridTile.spawnedTile = spawnedHex;
			hexGridTile.tileType = tileSpawnInfo.tileType;
			hexGrid.Add(hexCoord, hexGridTile);
		}

		public void DeleteTile(Hex hexCoord, bool isInEditor = false)
		{
			if (!hexGrid.ContainsKey(hexCoord)) 
				return;
			
			if (isInEditor)
			{
				DestroyImmediate(hexGrid[hexCoord].spawnedTile);
			}
			else
			{
				Destroy(hexGrid[hexCoord].spawnedTile);
			}

			hexGrid.Remove(hexCoord);
		}

		public void DeleteAllTiles(bool isInEditor = false)
		{
			foreach (KeyValuePair<Hex,HexGridTile> kvp in hexGrid)
			{
				if (isInEditor)
				{
					DestroyImmediate(kvp.Value.spawnedTile);
				}
				else
				{
					Destroy(kvp.Value.spawnedTile);
				}
			}

			hexGrid = new Dictionary<Hex, HexGridTile>();
		}

		public void LoadLevel()
		{
			
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