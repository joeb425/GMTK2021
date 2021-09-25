using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexLibrary
{
	public class HexPathFinding
	{
		private HexGrid grid;

		private HexGridLayer groundLayer;

		List<Hex> surTile = new List<Hex>();

		public List<Hex> FindPath2(HexGrid grid)
		{
			this.grid = grid;
			groundLayer = grid.GetLayer("Ground");
			List<Hex> path = new List<Hex>();
			FindPath_Rec(FindStartPoint(), path);
			return path;
		}
		
		public bool FindPath_Rec(Hex current, List<Hex> path)
		{
			if (groundLayer.GetHexComponent(current, out var hexComponent))
			{
				bool isPath = hexComponent.TileType == HexTileType.Start ||
				              hexComponent.TileType == HexTileType.Path ||
				              hexComponent.TileType == HexTileType.End;

				if (!isPath)
					return false;

				path.Add(current);

				if (hexComponent.TileType == HexTileType.End)
				{
					return true;
				}

				for (int i = 0; i < 6; i++)
				{
					Hex neighbor = current.Neighbor(i);
					if (path.Contains(neighbor))
					{
						continue;
					}

					if (FindPath_Rec(neighbor, path))
					{
						return true;
					}
				}
			}

			return false;
		}

		public List<Hex> FindPath(HexGrid grid)
		{
			List<Hex> path = new List<Hex>();

			this.grid = grid;
			groundLayer = grid.GetLayer("Ground");

			var hexGrid = groundLayer.hexGrid;

			Hex startTile = FindStartPoint();
			path.Add(startTile);

			//keeping track
			Hex curTile = startTile;
			Hex prevTile = curTile;

			bool searchPath = true;
			while (searchPath)
			{
				surTile.Clear();
				for (int i = 0; i < 6; i++)
				{
					if (hexGrid[curTile.Neighbor(i)] != null)
					{
						// Debug.Log(prev_tile.Item1 + " | " + cur_tile.Item1.Neighbor(i) + " - " + (prev_tile.Item1 == cur_tile.Item1.Neighbor(i)));

						if (prevTile.Equals(curTile.Neighbor(i)))
						{
							continue;
						}

						surTile.Add(curTile.Neighbor(i));
					}
				}

				//Debug.Log(surTile.Count);
				foreach (Hex tileCoord in surTile)
				{
					if (!groundLayer.GetHexComponent(tileCoord, out var hexComponent))
					{
						continue;
					}

					if (hexComponent.TileType == HexTileType.End)
					{
						path.Add(tileCoord);
						// tile_loc2 = hexGrid[tileCoord].spawnedTile.transform.position;
						break;
					}

					if (hexComponent.TileType == HexTileType.Path)
					{
						path.Add(tileCoord);
						prevTile = curTile;
						curTile = (tileCoord);
						// Debug.Log("Added a tile! : " + tile_coord + " - " + hexGrid[tile_coord].tileType);
						// Debug.Log("Prev tile : " + prevTile + "Cur_tile : " + curTile);
						break;
					}
				}
			}

			//Debug.Log(Path.Count);
			// return null;

			return path;
		}

		public Hex FindStartPoint()
		{
			var kvp = groundLayer.hexGrid.FirstOrDefault(x => x.Value.GetComponent<GroundTileComponent>().TileType == HexTileType.Start);
			return kvp.Key;
		}
	}
}