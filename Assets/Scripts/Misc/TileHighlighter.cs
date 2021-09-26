using System.Collections.Generic;
using HexLibrary;
using UnityEngine;

namespace Misc
{
	[CreateAssetMenu(menuName = "Misc/TileHighlighter")]
	public class TileHighlighter : ScriptableObject
	{
		[SerializeField]
		private GameObject TileHighlightPrefab;

		private Dictionary<Hex, GameObject> highlightedTiles = new Dictionary<Hex, GameObject>();

		public void SetHexHighlighted(Hex hex, bool isHighlighted, Color color = new Color())
		{
			if (isHighlighted)
			{
				if (highlightedTiles.TryGetValue(hex, out GameObject existingTile))
				{
					MeshRenderer[] meshes = existingTile.GetComponentsInChildren<MeshRenderer>();
					foreach (MeshRenderer meshRenderer in meshes)
					{
						meshRenderer.material.color = color;
					}
				}
				else
				{
					Vector3 worldPos = GameState.Get.Board.grid.flat.HexToWorld(hex);
					GameObject gameObject = Instantiate(TileHighlightPrefab);
					MeshRenderer[] meshes = gameObject.GetComponentsInChildren<MeshRenderer>();
					foreach (MeshRenderer meshRenderer in meshes)
					{
						meshRenderer.material.color = color;
					}

					gameObject.transform.position = worldPos;
					highlightedTiles.Add(hex, gameObject);
				}
			}
			else
			{
				if (highlightedTiles.TryGetValue(hex, out var go))
				{
					Destroy(go);
					highlightedTiles.Remove(hex);
				}
			}
		}
	}
}