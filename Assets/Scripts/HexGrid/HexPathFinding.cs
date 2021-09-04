using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace.HexGridEditor
{
    
    public class HexPathFinding : MonoBehaviour
    {
        public Dictionary<Hex, HexGridTile> hexGrid;
        public Dictionary<Hex, HexGridTile> Path = new Dictionary<Hex, HexGridTile>();
        public Layout flat;
        Vector3 tileLoc, tile_loc2;
        (Hex, HexGridTile) startTile, prevTile, curTile;
        List<Hex> surTile = new List<Hex>();
        bool searchPath = true;
        public Hex findPath(Dictionary<Hex, HexGridTile> hexGrid , Layout flat)
        {
            startTile = findStartPoint(hexGrid);
            Path.Add(startTile.Item1,startTile.Item2);
            this.flat = flat;
            this.hexGrid = hexGrid;
            // Gizmo stuff
            tileLoc = startTile.Item2.spawnedTile.transform.position;
            //keeping track
            curTile = startTile;
            prevTile = curTile;

            while (searchPath)
            {
                surTile.Clear();
                for (int i = 0; i < 6; i++)
                {
                    if (hexGrid[curTile.Item1.Neighbor(i)] != null)
                    {
                       // Debug.Log(prev_tile.Item1 + " | " + cur_tile.Item1.Neighbor(i) + " - " + (prev_tile.Item1 == cur_tile.Item1.Neighbor(i)));

                        if (prevTile.Item1.Equals(curTile.Item1.Neighbor(i)))
                        {
                            continue;
                        }

                        surTile.Add(curTile.Item1.Neighbor(i));
                    }
                }
                //Debug.Log(surTile.Count);
                foreach (Hex tile_coord in surTile)
                {
                    if (hexGrid[tile_coord].tileType == HexTileType.End)
                    {
                        Path.Add(tile_coord, hexGrid[tile_coord]);
                        searchPath = false;
                        tile_loc2 = hexGrid[tile_coord].spawnedTile.transform.position;
                        break;
                    }
                    if (hexGrid[tile_coord].tileType == HexTileType.Path)
                    {
                        Path.Add(tile_coord, hexGrid[tile_coord]);
                        prevTile = curTile;
                        curTile = (tile_coord, hexGrid[tile_coord]);
                       // Debug.Log("Added a tile! : " + tile_coord + " - " + hexGrid[tile_coord].tileType);
                       // Debug.Log("Prev tile : " + prevTile + "Cur_tile : " + curTile);
                        break;
                    }
                }
            }

            //Debug.Log(Path.Count);
            return null;
        }

        public (Hex, HexGridTile) findStartPoint(Dictionary<Hex, HexGridTile> hexGrid)
        {
            var temp = hexGrid.FirstOrDefault(x => x.Value.tileType == HexTileType.Start);
            return (temp.Key, temp.Value);
            
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 1, 0, 0.5f);
            foreach (KeyValuePair<Hex, HexGridTile> temp in Path)
            {
                var worldPixels = flat.HexToPixel(temp.Key);
                Vector3 pathCircle = new Vector3 (worldPixels.x, worldPixels.y, worldPixels.y);

                // Gizmos.DrawSphere(temp.Value.spawnedTile.transform.position, 1f);    Joes amazing solution
                Gizmos.DrawSphere(pathCircle, 1f);
            }
        }

    }
}
