using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexLibrary
{
    public class HexPathFinding : MonoBehaviour
    {
        private global::HexLibrary.HexGrid grid;

        private HexGridLayer groundLayer;
        
        // public Dictionary<Hex, HexGridTile> hexGrid;
        // public Dictionary<Hex, HexGridTile> Path = new Dictionary<Hex, HexGridTile>();
        public List<Hex> Path;
        Hex startTile, prevTile, curTile;
        List<Hex> surTile = new List<Hex>();
        bool searchPath = true;
        public void findPath(global::HexLibrary.HexGrid grid)
        {
            this.grid = grid;
            groundLayer = grid.GetLayer("Ground");

            var hexGrid = groundLayer.hexGrid;
            
            startTile = FindStartPoint();
            Path.Add(startTile);

            //keeping track
            curTile = startTile;
            prevTile = curTile;

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
                        Path.Add(tileCoord);
                        searchPath = false;
                        // tile_loc2 = hexGrid[tileCoord].spawnedTile.transform.position;
                        break;
                    }
                    if (hexComponent.TileType == HexTileType.Path)
                    {
                        Path.Add(tileCoord);
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
        }

        public Hex FindStartPoint()
        {
            var kvp = groundLayer.hexGrid.FirstOrDefault(x => x.Value.GetComponent<HexComponent>().TileType == HexTileType.Start);
            return kvp.Key;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 1, 0, 0.5f);
            foreach (Hex temp in Path)
            {
                var worldPos = grid.flat.HexToWorld(temp);
                Gizmos.DrawSphere(worldPos, 1f);
            }
        }
    }
}
