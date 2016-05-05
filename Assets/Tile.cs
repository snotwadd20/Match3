using UnityEngine;

namespace Useless.Match3
{
    public class Tile
    {
        public Match3 match3 = null;
        private int _type = -1;
        public GameObject art = null;

        public Vector2 position         { get { return art.transform.position; } set { art.transform.position = value; } }
        public Transform transform      { get { return art.transform; } }
        public GameObject gameObject    { get { return art.gameObject; } }

        public Tile(Match3 match3, int type)
        {
            this.match3 = match3;
            this.type = type;
            
        }//Tile

        public int type { get { return _type; } set { _type = value; ChangeArt(value); } }

        public GameObject ChangeArt(int newType)
        {
            GameObject.Destroy(art);

            if (newType != -1)
            {
                art = GameObject.Instantiate<GameObject>(match3.tilePrefabs[type]);
            }//if
            else
            {
                art = null;
            }//else

            return art;
        }//ChangeArt

    }//Tile
}//namespace