using UnityEngine;
using System.Collections;

namespace Useless.Match3
{
    public class Tile
    {
        public int tileType;
        public GameObject prefab;
        public TileControl tileControl;
        public int shift;

        public Tile()
        {
            tileType = -1;
            shift = 0;
        }//Constructor1

        public Tile(int tileType, GameObject prefab, TileControl tileControl)
        {
            this.tileType = tileType;
            this.prefab = prefab;
            this.tileControl = tileControl;
            shift = 0;
        }//Constructor2
    }//Tile
}//namespace
