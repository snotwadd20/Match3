using UnityEngine;

namespace Useless.Match3
{
    public class Tile
    {
        public class TileReference : MonoBehaviour { public Tile owner; }

        public Match3 match3 = null;
        private int _type = -1;
        public GameObject art = null;

        public Vector2 position         { get { return art.transform.position; } set { art.transform.position = value; } }
        public Transform transform      { get { return art.transform; } }
        public GameObject gameObject    { get { return art.gameObject; } }

        public UPoint gridPos;

        public Tile(Match3 match3, int type, UPoint gridPos)
        {
            this.match3 = match3;
            this.type = type;
            this.gridPos = gridPos;
        }//Tile

        public int type { get { return _type; } set { _type = value; ChangeArt(value); } }

        public GameObject ChangeArt(int newType)
        {
            Vector2 oldPos = Vector2.one * -1; ;
            if (art != null)
                oldPos = art.transform.position;

            GameObject.Destroy(art);

            if (newType != -1)
            {
                art = GameObject.Instantiate<GameObject>(match3.tilePrefabs[type]);
                TileReference tr = art.AddComponent<TileReference>();
                tr.owner = this;
                art.transform.parent = match3.transform;
            }//if
            else
            {
                art = null;
            }//else

            if (art != null)
                art.transform.position = oldPos;
            return art;
        }//ChangeArt

    }//Tile
}//namespace