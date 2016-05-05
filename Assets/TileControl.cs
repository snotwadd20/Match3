using UnityEngine;
using System.Collections;


namespace Useless.Match3
{
    public class TileControl : MonoBehaviour
    {
        public Match3Game gameManager;

        public UPoint myXY;

        public void Move(UPoint xy)
        {
            StartCoroutine(Moving(xy));
        }//Move

        //Physically move the piece and then report 
        //to gridManager when finished
        IEnumerator Moving(UPoint xy)
        {
            gameManager.ReportTileMovement();

            Vector2 destination = xy;

            bool moving = true;

            while (moving)
            {
                transform.position = Vector2.MoveTowards(transform.position, destination, 5f * Time.deltaTime);

                if (Vector2.Distance(transform.position, destination) <= 0.1f)
                {
                    transform.position = destination;
                    moving = false;
                }//if
                yield return null;
            }//while

            myXY = xy;
            //gameObject.name = xy.x + "/" + xy.x + "(o)";
            gameManager.ReportTileStopped();
        }//Moving
    }//TileControl
}//namespace