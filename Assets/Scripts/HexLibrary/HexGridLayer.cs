using System;
using System.Collections.Generic;
using DefaultNamespace;
using GameplayTags;
using UnityEngine;


namespace HexLibrary
{
	[Serializable]
	public struct HexObjectPair
	{
		[SerializeField]
		public Hex hex;

		[SerializeField]
		public HexTileComponent gameObject;

		public HexObjectPair(Hex h, HexTileComponent go)
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

		public Dictionary<Hex, HexTileComponent> hexGrid = new Dictionary<Hex, HexTileComponent>();

		[SerializeField]
		public List<HexTileComponent> tilePrefabs = new List<HexTileComponent>();

		[SerializeField]
		private List<HexObjectPair> serializedGrid = new List<HexObjectPair>();

		public System.Action<Hex, HexTileComponent> OnObjectAdded;
		public System.Action<Hex, HexTileComponent> OnObjectRemoved;
		public System.Action<HexTileComponent, HexTileComponent> OnSelectedObjectChanged;
		public HexTileComponent selectedObject;

		public void InitLayer(HexGrid grid, string layerName)
		{
			// Debug.Log($"Init layer {layerName}");
			this.grid = grid;
			this.layerName = layerName;

			tilePrefabs = new List<HexTileComponent>();
			foreach (HexTileSpawnData spawnData in grid.tilePalette.GetLayerPalette(layerName).spawnDatas)
			{
				tilePrefabs.Add(spawnData.tilePrefab);
			}

			if (GameState.Get != null)
			{
				GameState.Get.Board.OnSelectedTileChanged += (_, newHex) =>
				{
					GetTileAtHex(newHex, out HexTileComponent newSelection);
					if (selectedObject != newSelection)
					{
						OnSelectedObjectChanged?.Invoke(selectedObject, newSelection);
						selectedObject = newSelection;
					}
				};
			}
		}

		public bool GetTileAtHex<T>(Hex hexCoord, out T outTile) where T : HexTileComponent
		{
			if (hexGrid.TryGetValue(hexCoord, out HexTileComponent tile))
			{
				outTile = tile as T;
				return true;
			}

			outTile = null;
			return false;
		}

		// public bool GetComponentAtHex<T>(Hex hexCoord, out T behavior) where T : MonoBehaviour
		// {
		// 	if (GetObjectAtHex(hexCoord, out HexTileComponent go))
		// 	{
		// 		go.gameObject.TryGetComponent(out behavior);
		// 		return true;
		// 	}
		//
		// 	behavior = null;
		// 	return false;
		// }

		public HexTileComponent AddTile(Hex hexCoord, GameplayTag gameplayTag)
		{
			foreach (HexTileComponent prefab in tilePrefabs)
			{
				if (prefab.GetGameplayTag() == gameplayTag)
				{
					return AddTile(hexCoord, Instantiate(prefab));
				}
			}

			return null;
		}

		public HexTileComponent AddTile(Hex hexCoord, HexTileComponent objectOnTile)
		{
			DeleteTile(hexCoord);

			Vector3 worldPos = grid.flat.HexToWorld(hexCoord);

			objectOnTile.transform.parent = gameObject.transform;
			objectOnTile.transform.position = worldPos;

			hexGrid.Add(hexCoord, objectOnTile);
			serializedGrid.Add(new HexObjectPair(hexCoord, objectOnTile));

			if (objectOnTile.TryGetComponent(out HexTileComponent hexTileComponent))
			{
				hexTileComponent.hex = hexCoord;
				hexTileComponent.PlaceOnHex(hexCoord);
			}

			OnObjectAdded?.Invoke(hexCoord, objectOnTile);

			return objectOnTile;
		}

		public bool RemoveTile(Hex hexCoord, out HexTileComponent objectOnTile)
		{
			if (GetTileAtHex(hexCoord, out objectOnTile))
			{
				HexTileComponent hexComponent = objectOnTile.GetComponent<HexTileComponent>();
				if (hexComponent != null)
				{
					hexComponent.RemoveFromHex(hexCoord);
				}

				OnObjectRemoved?.Invoke(hexCoord, objectOnTile);

				hexGrid.Remove(hexCoord);
				return true;
			}

			return false;
		}

		public void DeleteTile(Hex hexCoord)
		{
			if (RemoveTile(hexCoord, out HexTileComponent objectOnTile))
			{
				if (Application.isPlaying)
				{
					Destroy(objectOnTile.gameObject);
				}
				else
				{
					DestroyImmediate(objectOnTile.gameObject);
				}
			}
		}

		public void ResetLayer()
		{
			GlobalHelpers.DeleteAllChildren(gameObject);
			hexGrid = new Dictionary<Hex, HexTileComponent>();
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
			hexGrid = new Dictionary<Hex, HexTileComponent>();

			foreach (var hexObject in serializedGrid)
			{
				if (hexObject.gameObject != null)
				{
					hexGrid[hexObject.hex] = hexObject.gameObject;
				}
			}
		}
	}
}