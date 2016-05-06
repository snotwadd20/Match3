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

        // Array of currently valid moves
        private List<Move> allLegalMoves;

        //Temporary list for storing detected matches
        private List<Match> allMatches = null;  // { column, row, length, horizontal }

        //------------------------------------------------------------------
        // Use this for initialization
        void Awake()
        {
            allMatches = new List<Match>();
            allLegalMoves = new List<Move>();

            //Seed the random number generator as appropriate
            if (seed == -1)
                seed = (int)System.DateTime.Now.Ticks % 1234567;

            Random.seed = seed;
            Debug.Log("Seed: " + seed);

            //Add player input script
            InitPlayerInput();

            //Fill the grid up with random stuff
            InitializeGrid();
            FindAllMatches();
            PrintAllMatches();
        }//Awake

        //------------------------------------------------------------------
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
                        if (grid[x, y].type == grid[x + 1, y].type &&
                            grid[x, y].type != -1)
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
                        if (grid[x, y].type == grid[x, y + 1].type &&
                            grid[x, y].type != -1)
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

        //------------------------------------------------------------------
        void FindAllLegalMoves()
        {
            // Reset moves
            allLegalMoves.Clear();
            // Check horizontal swaps
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth - 1; x++)
                {
                    //Tweak
                    TweakSwapTypes(x, y, x + 1, y);
                    //Check for matches
                    FindAllMatches();
                    //Tweak again
                    TweakSwapTypes(x, y, x + 1, y);

                    if (allMatches.Count > 0)
                        allLegalMoves.Add(new Move(x, y, x + 1, y));
                }//for
            }//for

            // Check vertical swaps
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight - 1; y++)
                {
                    //Tweak
                    TweakSwapTypes(x, y, x, y + 1);
                    //Check for matches
                    FindAllMatches();
                    //Tweak again
                    TweakSwapTypes(x, y, x, y + 1);

                    if (allMatches.Count > 0)
                    {
                        Move move = new Move(x, y, x, y + 1);
                        if (!allLegalMoves.Contains(move))
                            allLegalMoves.Add(move);
                    }//if  
                }//for
            }//for

            // Reset matches
            allMatches.Clear();
        }//FindAllLegalMoves

        //------------------------------------------------------------------
        private void InitPlayerInput()
        {
            PlayerInput pi = gameObject.AddComponent<PlayerInput>();
            pi.interactiveLayer = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
            pi.gameManager = this;
        }//InitPlayerInput

        //------------------------------------------------------------------
        private void InitializeGrid()
        {
            grid = new Tile[gridWidth, gridHeight];
            for (int y = 0; y < gridHeight; y++)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    grid[x, y] = new Tile(this, Random.Range(0, tilePrefabs.Count), new UPoint(x,y));
                    grid[x, y].position = new Vector2(x, y);
                    grid[x, y].transform.parent = transform;
                }//for
            }//for
        }//InitializeGrid

        //------------------------------------------------------------------
        private void PrintAllMatches()
        {
            for (int i = 0; i < allMatches.Count; i++)
                print(allMatches[i].ToString());
        }//PrintAllMatches

        //------------------------------------------------------------------
        private void PrintLegalMoves()
        {
            for (int i = 0; i < allLegalMoves.Count; i++)
                print(allLegalMoves[i].ToString());
        }//PrintAllMoves

        //------------------------------------------------------------------
        //Remove all matches from the board and fill them with tiles 
        private void ResolveAllMatches()
        {
            // Check for matches (fills out the matches list)
            FindAllMatches();

            // While there are matches left
            while (allMatches.Count > 0)
            {
                // Remove matches
                RemoveMatches();

                // Shift tiles
                //ShiftTiles();

                //Replace shifted tiles
                SpawnMissingTiles();

                // Check if there are matches left
                FindAllMatches();
            }//while
        }//ResolveAllMatches

        //------------------------------------------------------------------
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

                    grid[nx, ny].type = -1;
                    
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

        //------------------------------------------------------------------
        public void SpawnMissingTiles()
        {
            //Now fill in all the empty ones
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    if (grid[x, y].type == -1)
                    {
                        grid[x, y].type = Random.Range(0, tilePrefabs.Count);
                    }//if
                }//for
            }//for
        }//SpawnMissingTiles

        //------------------------------------------------------------------
        private void TweakSwapTypes(int x1, int y1, int x2, int y2)
        {
            int temp = grid[x1, y1].type;
            grid[x1, y1].TweakType(grid[x2, y2].type);
            grid[x2, y2].TweakType(temp);
        }//TweakSwapTypes

        //------------------------------------------------------------------
        public bool IsLegalMove(Move move)
        {
            allLegalMoves.Clear();
            FindAllLegalMoves();

            foreach(Move legalMove in allLegalMoves)
            {
                bool checkMatch = legalMove.point1.x == move.point1.x && legalMove.point1.y == move.point1.y && legalMove.point2.x == move.point2.x && legalMove.point2.y == move.point2.y;
                bool checkSwapped = legalMove.point1.x == move.point2.x && legalMove.point1.y == move.point2.y && legalMove.point2.x == move.point1.x && legalMove.point2.y == move.point1.y;

                if (checkMatch || checkSwapped)
                    return true;
            }//foreach
            
            return false;
        }//TileHasLegalMove

        //------------------------------------------------------------------
        public int MatchesInDirection(int type, UPoint start, UPoint direction)
        {
            int matchCounter = 0;

            UPoint temp = start;
            temp += direction;//Move one over (we don't care about the start point)

            while (temp.x < gridWidth  && temp.x >= 0 && temp.y < gridHeight && temp.y >= 0)
            {
                if (grid[temp.x, temp.y].type == type)
                    matchCounter++;

                temp += direction;
            }//while

            return matchCounter;

        }//MatchTypesInDirection

        //------------------------------------------------------------------
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

        //------------------------------------------------------------------
        public void GridSwap(UPoint pt1, UPoint pt2) { GridSwap(pt1.x, pt1.y, pt2.x, pt2.y); }//GridSwap
        public void GridSwap(int x1, int y1, int x2, int y2)
        {
            //Instantly swap two tiles on the grid by changing their type
            int temp = grid[x2, y2].type;

            //Swap grid2 type with grid1, generating a new prefab
            grid[x2, y2].type = grid[x1, y1].type;
            //grid[x2, y2].transform.parent = transform;

            //Swap grid1 type with grid2
            grid[x1, y1].type = temp;
            //grid[x1, y1].transform.parent = transform;
        }//GridSwap

        //------------------------------------------------------------------
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                print(ToString());

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                FindAllLegalMoves();
                PrintLegalMoves();
            }//if

            if(Input.GetKeyDown(KeyCode.Alpha3))
            {
                FindAllMatches();
                RemoveMatches();
            }//if

            if(Input.GetKeyDown(KeyCode.Alpha4))
            {
                SpawnMissingTiles();
            }//if

        }//Update

        //---------------------------------------
        // SUB-CLASSES
        //---------------------------------------        
        public class Match
        {
            //Members
            public int x; public int y; public int length; public bool isHorizontal;
            //Constructor
            public Match(int x, int y, int length, bool isHorizontal) { this.x = x; this.y = y; this.length = length; this.isHorizontal = isHorizontal; }

            public override string ToString()
            {
                return "Match at:" + x + "," + y + " -> Length: " + length + (isHorizontal ? " Horizontal" : " Vertical");
            }//ToString
        }//Match
        
        //A move (swapping tiles)
        public class Move : System.IEquatable<Move>
        {
            public UPoint point1; public UPoint point2;
            public Move(UPoint p1, UPoint p2) { point1 = p1; point2 = p2; }
            public Move(int x1, int y1, int x2, int y2) { point1 = new UPoint(x1, y1); point2 = new UPoint(x2, y2); }
            public override string ToString()
            {
                return point1.x + "," + point1.y + " <=> " + point2.x + "," + point2.y;
            }//ToString

            public override int GetHashCode()
            {
                return (point1.x ^ point1.y %123461237 * point2.x / point2.y);
            }

            static public bool operator == (Move point1, Move point2)
            {
                return Equals(point1, point2);
            }//==

            static public bool operator !=(Move point1, Move point2)
            {
                return !Equals(point1, point2);
            }//==
            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                Move objAsPart = obj as Move;
                if (objAsPart == null) return false;
                else return Equals(objAsPart);
            }
            public bool Equals(Move other)
            {
                return (other.point1 == this.point1 && other.point2 == this.point2) ||
                        (other.point1 == this.point2 && other.point2 == this.point1);
            }//Equals
        }//Move

    }//Match3
}//namespace