using System;
using System.Collections.Generic;
using DefaultNamespace;
using Mantis.GameplayTags;
using UnityEngine;

namespace HexLibrary
{
	[Serializable]
	public class HexGrid : MonoBehaviour, ISerializationCallbackReceiver
	{
		[SerializeField]
		public TextAsset levelToLoad;

		[SerializeField]
		public HexTilePalette tilePalette;

		[SerializeField]
		private List<HexGridLayer> serializedLayers = new List<HexGridLayer>();

		public HexPathFinding PathFinder = new HexPathFinding();

		public Layout flat;

		public Plane gridPlane = new Plane(Vector3.up, Vector3.zero);

		// stores a reference to the layer 
		public Dictionary<string, HexGridLayer> layers = new Dictionary<string, HexGridLayer>();

		public HexGrid()
		{
			flat = new Layout(Layout.flat, new Vector2(1.0f, 1.0f), Vector2.zero);
		}

		public bool GetHexUnderRay(Ray ray, out Hex hex)
		{
			if (gridPlane.Raycast(ray, out float dist))
			{
				Vector3 hitPoint = ray.GetPoint(dist);
				Vector2 point = new Vector2(hitPoint.x, hitPoint.z);
				hex = flat.PixelToHex(point).HexRound();
				return true;
			}

			hex = null;
			return false;
		}

		public void Awake()
		{
			// Init();
		}

		public void Init()
		{
			Debug.Log("Init");
			if (Application.isPlaying)
			{
				LoadLevel();
				// PathFinder.FindPath(this);
			}
			else
			{
				ResetGrid();
			}
		}

		public void LoadLevel()
		{
			// TODO load level selected from menu screen
			LevelData levelData = GlobalData.GetAssetBindings().levelData;
			if (!levelData.IsLastLevel(GlobalData.CurrentLevel))
			{
				levelToLoad = levelData.levels[GlobalData.CurrentLevel];
				Debug.Log($"Load level {GlobalData.CurrentLevel} : {levelToLoad.name}");

			}

			
			LoadLevelFromJson(levelToLoad.text);
		}

		public void InitLayers()
		{
			layers = new Dictionary<string, HexGridLayer>();
			serializedLayers = new List<HexGridLayer>();

			Debug.Log("Init layers?");
			foreach (HexTileLayerPalette layer in tilePalette.tilePalette)
			{
				Debug.Log($"Add layer {layer.layerName}");
				AddLayer(layer.layerName);
			}
		}

		public void AddLayer(string layerName)
		{
			if (layers.ContainsKey(layerName))
			{
				return;
			}

			GameObject layerContainer = new GameObject(layerName);
			layerContainer.transform.parent = transform;
			HexGridLayer layer = layerContainer.AddComponent<HexGridLayer>();
			layer.InitLayer(this, layerName);

			layers.Add(layerName, layer);
			serializedLayers.Add(layer);
		}

		public HexGridLayer GetLayer(string layerName)
		{
			return layers.ContainsKey(layerName) ? layers[layerName] : null;
			//serializedLayers.FirstOrDefault(layer => layer.layerName == layerName);

			// return layers[layerName];
		}

		public void ResetGrid()
		{
			// foreach (HexGridLayer layer in layers.Values)
			// {
			// 	layer.ResetLayer();
			// }

			GlobalHelpers.DeleteAllChildren(gameObject);

			InitLayers();
		}

		public void LoadLevelFromJson(string json)
		{
			ResetGrid();
			JsonHexGrid jsonHexGrid = JsonUtility.FromJson<JsonHexGrid>(json);
			foreach (JsonHexLayer jsonHexLayer in jsonHexGrid.hexLayer)
			{
				HexGridLayer hexGridLayer = GetLayer(jsonHexLayer.layerName);

				foreach (JsonHex jsonHex in jsonHexLayer.hexData)
				{
					Hex hex = jsonHex.hex;
					GlobalData.GetTagManager().RequestTag(jsonHex.tagName, out GameplayTag gameplayTag);
					if (hexGridLayer.AddTile(hex, gameplayTag))
					{
						// Debug.Log($"Spawn tile {hex} : {jsonHex.guid}");
					}
				}
			}
		}

		public JsonHexGrid GenerateJsonGrid()
		{
			JsonHexGrid jsonHexGrid = new JsonHexGrid();

			foreach (var layerKvp in layers)
			{
				string layerName = layerKvp.Key;
				JsonHexLayer hexLayer = new JsonHexLayer(layerName);
				jsonHexGrid.hexLayer.Add(hexLayer);

				HexGridLayer hexGridLayer = layerKvp.Value;
				foreach (var hexKvp in hexGridLayer.hexGrid)
				{
					HexTileComponent hexContent = hexKvp.Value;
					JsonHex jsonHex = new JsonHex(hexContent);
					hexLayer.hexData.Add(jsonHex);
				}
			}

			return jsonHexGrid;
		}

		public void OnBeforeSerialize()
		{
			serializedLayers.RemoveAll(layer => layer == null);
		}

		public void OnAfterDeserialize()
		{
			serializedLayers.RemoveAll(layer => layer == null);

			layers = new Dictionary<string, HexGridLayer>();

			foreach (HexGridLayer layer in serializedLayers)
			{
				// Debug.Log(layer.layerName + " " + (layer == null));
				layers.Add(layer.layerName, layer);
			}
		}
	}
}