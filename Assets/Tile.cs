using UnityEngine;
using System.Collections;

namespace Useless.Match3
{
    public class Tile
    {
        public int tileType;
        public GameObject prefab;
        //public TileControl tileControl;
        public int shift;

        public Tile()
        {
            tileType = -1;
            shift = 0;
        }//Constructor1

        public Tile(int tileType, GameObject prefab)
        {
            this.tileType = tileType;
            this.prefab = prefab;
        }//Constructor2
    }//Tile
}//namespace
