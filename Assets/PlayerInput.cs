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
        
        void AttemptMove(RaycastHit2D hit)
        {
            if (hit && hit.collider.gameObject != activeTile)
            {
                if (Vector2.Distance(activeTile.transform.position, hit.collider.gameObject.transform.position) <= 1.25f)
                {
                    Tile tile1 = activeTile.GetComponent<Tile.TileReference>().owner;
                    Tile tile2 = hit.collider.gameObject.GetComponent<Tile.TileReference>().owner;

                    //print(gameManager.MatchesInDirection(tile1.type, tile1.gridPos, tile2.gridPos - tile1.gridPos));
                    Match3.Move move = new Match3.Move(tile1.gridPos, tile2.gridPos);
                    //Check if it's a legal move
                    if (gameManager.IsLegalMove(move))
                    {
                        //Scale it back down before we move it
                        activeTile.transform.localScale = activeTileOriginalScale;

                        StartCoroutine(Move(tile1, tile2));
                    }//if
                    else
                    {
                        print("Not a legal move: " + move.ToString());
                    }//else
                        
                }//if
            }//if 
            activeTile.transform.localScale = activeTileOriginalScale;
            activeTile = null;
        }//AttemptMove

        IEnumerator Move(Tile tile1, Tile tile2)
        {
            Vector2 tile1Pos = tile1.position;
            Vector2 tile2Pos = tile2.position;

            tile1.art.MoveTo(tile2Pos, 0.5f, 0);

            tile2.art.MoveTo(tile1Pos, 0.5f, 0);

            yield return new WaitForSeconds(0.5f);
            tile1.art.SetActive(false);
            tile2.art.SetActive(false);

            tile1.position = tile1Pos;
            tile2.position = tile2Pos;

            //Then swap the tiles, which will create new arts for each of the appropriate type
            gameManager.GridSwap(tile1.gridPos, tile2.gridPos);
            gameManager.ResolveAllMatches();
            yield return new WaitForEndOfFrame();
        }//move

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

    }//PlayerInput
}//namespace
