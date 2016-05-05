using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Useless.Match3
{
    public class Match3 : MonoBehaviour
    {
        public int gridWidth = 4;
        public int gridHeight = 4;

        public List<GameObject> tilePrefabs;

        private Tile[,] grid;

        public int seed = -1;

        // Use this for initialization
        void Awake()
        {
            //Seed the random number generator as appropriate
            if (seed == -1)
                seed = (int)System.DateTime.Now.Ticks % 1234567;

            Random.seed = seed;
            Debug.Log("Seed: " + seed);

            //Add player input script
            InitPlayerInput();

            //Fill the grid up with random stuff
            InitializeGrid();
        }//Awake

        private void InitPlayerInput()
        {
            PlayerInput pi = gameObject.AddComponent<PlayerInput>();
            pi.interactiveLayer = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
            pi.gameManager = this;
        }//InitPlayerInput

        private void InitializeGrid()
        {
            grid = new Tile[gridWidth, gridHeight];
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    grid[x, y] = new Tile(this, Random.Range(0, tilePrefabs.Count));
                    grid[x, y].position = new Vector2(x, y);
                    grid[x, y].transform.parent = transform;
                }//for
            }//for
        }//InitializeGrid

        public override string ToString()
        {
            string str = "   | ";
            string line = "\n";
            for (int x = 0; x < gridHeight; x++)
            {
                str += x.ToString("D2") + ",";
                line += " - - -";
            }//for
            str += line;

            for (int y = gridHeight - 1; y >= 0; y--)
            {
                str += "\n" + y + " | ";
                for (int x = 0; x < gridHeight; x++)
                {
                    str += ((grid[x, y].type < 0) ? " " + grid[x, y].type.ToString() : grid[x, y].type.ToString("D2")) + ",";
                }//for
            }//for
            return str;
        }//ToString

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.KeypadEnter))
                print(ToString());
        }//Update
    }//Match3
}//namespace