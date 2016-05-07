using UnityEngine;

namespace Useless.Match3
{
    public class Tile
    {
        public class TileReference : MonoBehaviour { public Tile owner; void Update() { name = owner.gridPos.ToString(); } }

        public Match3 match3 = null;
        private int _type = -1;
        public GameObject art = null;

        public Vector2 position         { get { return art.transform.position; } set { art.transform.position = value; } }
        public Transform transform      { get { return art.transform; } }
        public GameObject gameObject    { get { return art.gameObject; } }

        public UPoint gridPos;

        public Vector2 _cachedArtPos;

        public int type { get { return _type; } set { _type = value; ChangeArt(value); } }

        //------------------------------------------------------------
        //------------------------------------------------------------
        public Tile(Match3 match3, int type, UPoint gridPos)
        {
            _cachedArtPos = gridPos;
            this.match3 = match3;
            this.type = type;
            this.gridPos = gridPos;
        }//Tile
     
        //------------------------------------------------------------
        //------------------------------------------------------------
        public GameObject ChangeArt(int newType)
        {
            if (art)
            {
                _cachedArtPos = art.transform.position;
                GameObject.Destroy(art);
            }//if
            
            if (newType != -1)
            {
                art = GameObject.Instantiate<GameObject>(match3.tilePrefabs[type]);
                TileReference tr = art.AddComponent<TileReference>();
                tr.owner = this;
                art.transform.parent = match3.transform;
            }//if
            else
            {
                art = new GameObject("Art Placeholder");
            }//else

            art.transform.position = _cachedArtPos;

            return art;
        }//ChangeArt

        //------------------------------------------------------------
        //Change the type without changing the art
        public void TweakType(int newType)
        {
            _type = newType;
        }//TweakType
    }//Tile
}//namespace