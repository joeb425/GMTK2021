using System;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;

namespace HexLibrary
{
	[Serializable]
	public struct HexObjectPair
	{
		[SerializeField]
		public Hex hex;

		[SerializeField]
		public GameObject gameObject;

		public HexObjectPair(Hex h, GameObject go)
		{
			hex = h;
			gameObject = go;
		}
	}

	[Serializable]
	public class HexGridLayer : MonoBehaviour, ISerializationCallbackReceiver
	{
		[SerializeField]
		public string layerName;

		[SerializeField]
		public HexGrid grid;

		public Dictionary<Hex, GameObject> hexGrid = new Dictionary<Hex, GameObject>();

		[SerializeField]
		public List<GameObject> tilePrefabs = new List<GameObject>();

		[SerializeField]
		private List<HexObjectPair> serializedGrid = new List<HexObjectPair>();

		public void InitLayer(HexGrid grid, string layerName)
		{
			Debug.Log($"Init layer {layerName}");
			this.grid = grid;
			this.layerName = layerName;

			tilePrefabs = new List<GameObject>();
			foreach (HexTileSpawnData spawnData in grid.tilePalette.GetLayerPalette(layerName).spawnDatas)
			{
				tilePrefabs.Add(spawnData.tilePrefab);
			}
		}

		public bool GetTile(Hex hexCoord, out GameObject tile)
		{
			return hexGrid.TryGetValue(hexCoord, out tile);
		}

		public GameObject AddTile(Hex hexCoord, Guid guid)
		{
			foreach (GameObject prefab in tilePrefabs)
			{
				if (prefab.GetComponent<GuidComponent>().GetGuid() == guid)
				{
					return AddTile(hexCoord, prefab);
				}
			}

			return null;
		}

		public GameObject AddTile(Hex hexCoord, GameObject tilePrefab)
		{
			DeleteTile(hexCoord);

			Vector3 worldPos = grid.flat.HexToWorld(hexCoord);

			var tileContent = Instantiate(tilePrefab, gameObject.transform, true);

			tileContent.transform.position = worldPos;

			hexGrid.Add(hexCoord, tileContent);
			serializedGrid.Add(new HexObjectPair(hexCoord, tileContent));

			return tileContent;
		}

		public bool RemoveTile(Hex hexCoord, out GameObject objectOnTile)
		{
			if (GetTile(hexCoord, out objectOnTile))
			{
				hexGrid.Remove(hexCoord);
				return true;
			}

			return false;
		}

		public void DeleteTile(Hex hexCoord)
		{
			if (RemoveTile(hexCoord, out GameObject objectOnTile))
			{
				if (Application.isPlaying)
				{
					Destroy(objectOnTile);
				}
				else
				{
					DestroyImmediate(objectOnTile);
				}
			}
		}

		public void ResetLayer()
		{
			GlobalHelpers.DeleteAllChildren(gameObject);
			hexGrid = new Dictionary<Hex, GameObject>();
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

		public void OnBeforeSerialize()
		{
			serializedGrid.RemoveAll(hexObject => hexObject.gameObject == null);
		}

		public void OnAfterDeserialize()
		{
			hexGrid = new Dictionary<Hex, GameObject>();

			foreach (var hexObject in serializedGrid)
			{
				if (hexObject.gameObject != null)
				{
					hexGrid[hexObject.hex] = hexObject.gameObject;
				}
			}
		}

		public bool GetHexComponent(Hex hex, out HexComponent hexComponent)
		{
			if (GetTile(hex, out var tile))
			{
				hexComponent = tile.GetComponent<HexComponent>();
				if (hexComponent != null)
				{
					return true;
				}
			}

			hexComponent = null;
			return false;
		}
	}
}