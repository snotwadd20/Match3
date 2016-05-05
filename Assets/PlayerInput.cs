using UnityEngine;
using System.Collections;

namespace Useless.Match3
{
    public class PlayerInput : MonoBehaviour
    {
        public LayerMask interactiveLayer = 0; //What can be clicked on to interact with

        private GameObject activeTile; //The first tile the player picks (to swap)
        private Vector3 activeTileOriginalScale;

        public Match3 gameManager;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.GetRayIntersection(ray, 50f, interactiveLayer);

                if (activeTile == null)
                    SelectTile(hit);
                else
                    AttemptMove(hit);
            }//if
            else if (Input.GetKeyDown(KeyCode.Mouse1))
                activeTile = null;
        }//Update

        void SelectTile(RaycastHit2D hit)
        {
            if (hit)
            {
                activeTile = hit.collider.gameObject;

                //highlight the tile here
                activeTileOriginalScale = activeTile.transform.localScale;
                activeTile.transform.localScale = activeTileOriginalScale * 1.2f;
            }//if
        }//SelectTile

        void AttemptMove(RaycastHit2D hit)
        {
            if (hit)
            {
                if (Vector2.Distance(activeTile.transform.position, hit.collider.gameObject.transform.position) <= 1.25f)
                {

                    //MOVE TILE
                    print("TODO: MOVE TILE");
                }//if
            }//if 
            activeTile.transform.localScale = activeTileOriginalScale;
            activeTile = null;
        }//AttemptMove  

    }//PlayerInput
}//namespace
