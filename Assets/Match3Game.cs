using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Useless.Match3
{
    public class Match3Game : MonoBehaviour
    {
        public class Match
        {
            //Members
            public int x; public int y; public int length; public bool isHorizontal;
            //Constructor
            public Match(int x, int y, int length, bool isHorizontal) { this.x = x; this.y = y; this.length = length; this.isHorizontal = isHorizontal; }
        }//Match

        //A move (swapping tiles)
        public class Move
        {
            public UPoint point1; public UPoint point2;
            public Move(UPoint p1, UPoint p2) { point1 = p1; point2 = p2; }
            public Move(int x1, int y1, int x2, int y2) { point1 = new UPoint(x1, y1); point2 = new UPoint(x2, y2); }
        }//Move

        //Temporary list for storing detected matches
        private List<Match> allMatches = null;  // { column, row, length, horizontal }

        public int gridWidth = 6;
        public int gridHeight = 6;
        
        public GameObject[] tilePrefabs;
        public Tile[,] tileGrid;

        public int seed = -1;

        //Number of tiles currently moving around
        private int movingTiles = 0;

        private bool animateMoves = false;

        // Array of moves
        private List<Move> allMoves;

        void Awake()
        {
            allMatches = new List<Match>();
            allMoves = new List<Move>();

            if (seed != -1)
                Random.seed = seed;
            else
                Random.seed = (int)System.DateTime.Now.Ticks % 1234567890;

            Debug.Log("Random seed: " + Random.seed);

            //GenerateMatchlessGrid();
            FillGridRandomly();
            FindAllMatches();
            //PrintAllMatches();
            RemoveMatches();
            //animateMoves = true;

        }//Awake

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShiftTiles();
            }

            if(Input.GetKeyDown(KeyCode.KeypadEnter))
                print(ToString());


            if (Input.GetKeyDown(KeyCode.Return))
            {
                SpawnMissingTiles();
                print(ToString());
            }
        }//Update

        public void FillGridRandomly()
        {
            tileGrid = new Tile[gridWidth, gridHeight];

            // Fill out the grid with random tiles to start
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    int randomtileType = Random.Range(0, tilePrefabs.Length);

                    GameObject go = Instantiate(tilePrefabs[randomtileType], new Vector2(x, y), Quaternion.identity) as GameObject;
                    TileControl tc = go.GetComponent<TileControl>();
                    tc.gameManager = this;
                    tc.myXY = new UPoint(x, y);
                    
                    tileGrid[x, y] = new Tile(randomtileType, go, tc);
                    go.name = x + "/" + y;
                    go.transform.parent = transform;
                }//for
            }//for
        }//FillGridRandomly

        // Create a random grid of tiles
        public void GenerateMatchlessGrid()
        {
            bool done = false;

            // Keep generating grids until one is
            while (!done)
            {
                FillGridRandomly();

                // Remove all matches so we start with a matchless board
                RemoveAllMatches();

                // Check if there are valid moves
                FindAllMoves();

                // Done when there is a valid move
                if (allMoves.Count > 0)
                {
                    done = true;
                }//if
            }//while
        }//GenerateMatchlessGrid

        //Remove all matches from the board and fill them with tiles 
        private void RemoveAllMatches()
        {
            // Check for matches (fills out the matches list)
            FindAllMatches();

            // While there are matches left
            while (allMatches.Count > 0)
            {
                // Remove matches
                RemoveMatches();

                // Shift tiles
                ShiftTiles();

                //Replace shifted tiles
                SpawnMissingTiles();

                // Check if there are matches left
                FindAllMatches();
            }//while
        }//ResolveMatches

        // Swap two tiles in the level
        public void Swap(UPoint pt1, UPoint pt2, bool swapSprites = false)
        {
            Swap(pt1.x, pt1.y, pt2.x, pt2.y);
        }//Swap
        public void Swap(int x1, int y1, int x2, int y2)
        {
            //TODO: Prefabs need to be moved in space (maybe done somewhere else)
            Tile tile1 = tileGrid[x1, y1];
            Tile tile2 = tileGrid[x2, y2];

            //Prefabs need to be tracked by new cell, not old cell (swap prefabs)
            GameObject pf = tile2.prefab;
            tile2.prefab = tile1.prefab;
            tile1.prefab = pf;

            //The TileControl objects need to be updated with new grid cell numbers
            UPoint upt = tile2.tileControl.myXY;
            tile2.tileControl.myXY = tile1.tileControl.myXY;
            tile1.tileControl.myXY = upt;

            //Tile types need to be swapped
            int temp = tile2.tileType;
            tile2.tileType = tile1.tileType;
            tile1.tileType = temp;

            
        }//Swap

        // Find available moves
        void FindAllMoves()
        {
            // Reset moves
            allMoves.Clear();

            // Check horizontal swaps
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth - 1; x++)
                {
                    // Swap, find matches and swap back
                    Swap(x, y, x + 1, y);
                    FindAllMatches();
                    Swap(x, y, x + 1, y);

                    // Check if the swap made a Match
                    if (allMatches.Count > 0)
                    {
                        // Found a move
                        //moves.push({ column1: x, row1: y, column2: x + 1, row2: y});
                        allMoves.Add(new Move(x, y, x + 1, y));
                        //print("Valid move found: " + (new UPoint(x,y)) + " " + (new UPoint(x+1,y)));
                    }//if
                }//for
            }//for

            // Check vertical swaps
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight - 1; y++)
                {
                    // Swap, find matches and swap back
                    Swap(x, y, x, y + 1);
                    FindAllMatches();
                    Swap(x, y, x, y + 1);

                    // Check if the swap made a Match
                    if (allMatches.Count > 0)
                    {
                        // Found a move
                        //allMoves.Add({column1: x, row1: y, column2: x, row2: y+1});
                        allMoves.Add(new Move(x, y, x, y + 1));
                        //print("Valid move found: " + (new UPoint(x, y)) + " " + (new UPoint(x, y + 1)));
                    }//if
                }//for
            }//for

            // Reset matches
            allMatches.Clear();
        }//FindAllMoves

        public void RemoveMatches()
        {
            //DeleteAllMatchedTiles();
            for (int i = 0; i < allMatches.Count; i++)
            {
                Match match = allMatches[i];
                int coffset = 0;
                int roffset = 0;
                int nx = 0;
                int ny = 0;
                for (int j = 0; j < match.length; j++)
                {
                    nx = match.x + coffset;
                    ny = match.y + roffset;

                    tileGrid[nx, ny].tileType = -1;
                    tileGrid[nx, ny].prefab.SetActive(false);

                    if (!tileGrid[nx, ny].prefab.activeSelf && tileGrid[nx, ny].tileType != -1)
                    {
                        print("SHIT");
                    }

                    if (match.isHorizontal)
                    {
                        coffset++;
                    }//if
                    else
                    {
                        roffset++;
                    }//else
                }//for
            }//for
        }//RemoveMatches
        
        public IEnumerator DoTileGravity()
        {
            for (int y = 1; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    //Loop from bottom to top
                    if (tileGrid[x, y].tileType != -1)
                    {
                        bool done = false;
                        int _failsafe = 0;
                        int tempY = y;

                        Tile fallingObj = tileGrid[x, y];

                        while (!done)
                        {
                            if (tempY >=0 && tileGrid[x, tempY-1].tileType == -1)
                            {
                                //Sink until there aint no more sinkin' to do
                                Swap(x, tempY, x, tempY - 1);
                                //Do falling movement
                            }//if
                            else
                                done = true;

                            tempY--;


                            _failsafe++;
                            if (_failsafe >= 100)
                                done = true;
                        }//while

                        Vector2 endPos = new Vector2(x, tempY + 1);
                        if (animateMoves)
                            yield return StartCoroutine(fallingObj.tileControl.Moving(endPos));
                        else
                            fallingObj.prefab.transform.position = endPos;

                    }//if
                }//for
            }//for
            yield return new WaitForEndOfFrame();
        }//DoTileGravity

        public void ShiftTiles()
        {
            StartCoroutine(DoTileGravity());
        }//ShiftTiles

        public void SpawnMissingTiles()
        {
            //Now fill in all the empty ones
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = gridHeight - 1; y >= 0 ; y--)
                {
                    if (tileGrid[x, y].tileType == -1)
                    {
                        int type = Random.Range(0, tilePrefabs.Length);
                        
                        // Insert new random tile
                        GameObject oldPrefab = tileGrid[x, y].prefab;
                        tileGrid[x, y].prefab = Instantiate<GameObject>(tilePrefabs[type]);

                        //tileGrid[x, y].shift = 0;
                        tileGrid[x, y].tileType = type;
                        TileControl tc = tileGrid[x, y].tileControl;
                        tc.myXY = new UPoint(x, y);

                        //tileGrid[x, y].prefab.name = x + "/" + y + "(n)";
                        tileGrid[x, y].prefab.transform.rotation = Quaternion.identity;
                        tileGrid[x, y].prefab.transform.position = new Vector2(x,y);
                        tileGrid[x, y].prefab.transform.parent = transform;

                        Destroy(oldPrefab);
                    }//if
                }//for
            }//for
        }//SpawnMissingTiles
        

        /*
        if (tileGrid[x,y].tileType == -1)
                    {
                        int type = Random.Range(0, tilePrefabs.Length);
                        // Insert new random tile
                        Destroy(tileGrid[x, y].prefab);
                        tileGrid[x, y].shift = 0;
                        tileGrid[x, y].tileType = type;
                        tileGrid[x,y].prefab = Instantiate(tilePrefabs[type], new Vector2(x, y), Quaternion.identity) as GameObject;
                        tileGrid[x, y].prefab.name = x+"/"+y;
                        tileGrid[x, y].prefab.transform.parent = transform;
                    }//if
                    else
                    {*/


        // Find matches in the level
        private void FindAllMatches()
        {
            // Reset the matches structure
            if (allMatches == null)
                allMatches = new List<Match>();
            else
                allMatches.Clear();

            // Find horizontal matches
            for (int y = 0; y < gridHeight; y++)
            {
                // Start with a single tile, Match of 1
                int matchLength = 1;
                for (int x = 0; x < gridWidth; x++)
                {
                    bool checkMatch = false;

                    if (x == gridWidth - 1)
                    {
                        // Last tile
                        checkMatch = true;
                    }//if
                    else
                    {
                        // Check the type of the next tile
                        if (tileGrid[x, y].tileType == tileGrid[x + 1, y].tileType &&
                            tileGrid[x, y].tileType != -1)
                        {
                            // Same type as the previous tile, increase matchlength
                            matchLength += 1;
                        }//if
                        else
                        {
                            // Different type
                            checkMatch = true;
                        }//else
                    }//else

                    // Check if there was a Match
                    if (checkMatch)
                    {
                        if (matchLength >= 3)
                        {
                            // Found a horizontal Match
                            //column: i + 1 - matchlength, 
                            //row: j, 
                            //length: matchlength, 
                            //horizontal: true });

                            Match theMatch = new Match(x + 1 - matchLength, y, matchLength, true);
                            allMatches.Add(theMatch);
                            //print("Match at:" + theMatch.x + "," + theMatch.y + " -> L: " + theMatch.length + " isHorz: " + theMatch.isHorizontal);
                        }//if

                        matchLength = 1;
                    }//if
                }//for
            }//for

            // Find vertical matches
            for (int x = 0; x < gridWidth; x++)
            {
                // Start with a single tile, Match of 1
                int matchlength = 1;
                for (int y = 0; y < gridHeight; y++)
                {
                    bool checkMatch = false;

                    if (y == gridHeight - 1)
                    {
                        // Last tile
                        checkMatch = true;
                    }//if
                    else
                    {
                        // Check the type of the next tile
                        if (tileGrid[x, y].tileType == tileGrid[x, y + 1].tileType &&
                            tileGrid[x, y].tileType != -1)
                        {
                            // Same type as the previous tile, increase matchlength
                            matchlength += 1;
                        }//if
                        else
                        {
                            // Different type
                            checkMatch = true;
                        }//else
                    }//else

                    // Check if there was a Match
                    if (checkMatch)
                    {
                        if (matchlength >= 3)
                        {
                            // Found a vertical Match
                            Match theMatch = new Match(x, y + 1 - matchlength, matchlength, false);
                            allMatches.Add(theMatch);

                            //print("Match at:" + theMatch.x + "," + theMatch.y + " -> L: " + theMatch.length + " isHorz: " + theMatch.isHorizontal);
                        }//if

                        matchlength = 1;
                    }//if
                }//for
            }//for
        }//FindAllMatches

        private void PrintAllMatches()
        {
            for (int i = 0; i < allMatches.Count; i++)
            {
                Match theMatch = allMatches[i];
                print("Match at:" + theMatch.x + "," + theMatch.y + " -> L: " + theMatch.length + " isHorz: " + theMatch.isHorizontal);
            }//for
        }

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

            for (int y= gridHeight-1; y >= 0; y--)
            {
                str += "\n" + y + " | ";
                for(int x=0; x < gridHeight; x++)
                {
                    str += ((tileGrid[x, y].tileType < 0) ? " " + tileGrid[x, y].tileType.ToString() : tileGrid[x, y].tileType.ToString("D2")) + ",";
                }//for
            }//for
            return str;
        }//ToString

        public void ReportTileMovement()
        {
            movingTiles++;
        }//ReportTileMovement

        public void ReportTileStopped()
        {
            movingTiles--;

            if (movingTiles == 0)
            {
            }//movingTiles
        }//ReportTileStopped       

    }//Match3Game


}//namespace
