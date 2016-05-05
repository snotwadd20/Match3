﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Useless.Match3
{
    public class MatchGame : MonoBehaviour
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
            public Move(int x1, int y1, int x2, int y2) { point1 = new UPoint(x1, y1);  point2 = new UPoint(x2,y2); }
        }//Move

        //Temporary list for storing detected matches
        private List<Match> allMatches = null;  // { column, row, length, horizontal }

        public int gridWidth = 6;
        public int gridHeight = 6;

        public GameObject[] tilePrefabs;
        public Tile[,] tileGrid;

        public int seed = -1;

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
            CreateGrid();

            FindAllMoves();
            Debug.Log(allMoves.ToString());
            //GenerateMatchlessGrid();
        }//Awake

        // Update is called once per frame
        void Update()
        {

        }//Update

        private void CreateGrid()
        {
            tileGrid = new Tile[gridWidth, gridHeight];

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {

                    int randomtileType = Random.Range(0, tilePrefabs.Length);

                    GameObject go = Instantiate(tilePrefabs[randomtileType], new Vector2(x, y), Quaternion.identity) as GameObject;
                    
                    //TileControl tileControl = go.GetComponent<TileControl>();
                    tileGrid[x, y] = new Tile(randomtileType, go);
                    //tileControl.matchManager = this;
                    //tileControl.gridPosition = new UPoint(x, y);
                    go.name = x + "/" + y;

                    go.transform.parent = transform;
                }//for
            }//for
        }//CreateGrid

        // Create a random grid of tiles
        public void GenerateMatchlessGrid()
        {
            bool done = false;

            // Keep generating grids until one is
            while (!done)
            {
                // Fill out the grid with random tiles to start
                for (int x = 0; x < gridWidth; x++) {
                    for (int y = 0; y < gridHeight; y++) {
                        tileGrid[x, y].tileType = Random.Range(0, tilePrefabs.Length);
                    }//for
                }//for

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

                // Check if there are matches left
                FindAllMatches();
            }//while
        }//ResolveMatches

        // Swap two tiles in the level
        public void Swap(UPoint from, UPoint to)
        {
            Swap(from.x, from.y, to.x, to.y);
        }//Swap
        public void Swap(int x1, int y1, int x2, int y2)
        {
            int typeswap = tileGrid[x1,y1].tileType;
            tileGrid[x1,y1].tileType = tileGrid[x2,y2].tileType;
            tileGrid[x2,y2].tileType = typeswap;
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
                        allMoves.Add( new Move(x, y, x + 1, y) );
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
                    }
                }//for
            }//for

            // Reset matches
            allMatches.Clear();
        }//FindAllMoves

        public void RemoveMatches()
        {
            // Calculate how much a tile should be shifted downwards
            for (var x = 0; x < gridWidth; x++)
            {
                var shift = 0;
                for (var y = gridHeight - 1; y >= 0; y--)
                {
                    // Loop from bottom to top
                    if (tileGrid[x,y].tileType == -1)
                    {
                        // Tile is removed, increase shift
                        shift++;
                        tileGrid[x,y].shift = 0;
                    }//if
                    else
                    {
                        // Set the shift
                        tileGrid[x,y].shift = shift;
                    }//else
                }//for
            }//for
        }//RemoveMatches

        public void ShiftTiles()
        {

        }


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
                        if (tileGrid[x,y].tileType == tileGrid[x + 1,y].tileType &&
                            tileGrid[x,y].tileType != -1)
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
                        if (tileGrid[x,y].tileType == tileGrid[x,y + 1].tileType &&
                            tileGrid[x,y].tileType != -1)
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
                            //allMatches.Add({ column: i, row:j+1-matchlength,length: matchlength, horizontal: false });
                            allMatches.Add(new Match(x, y + 1 - matchlength, matchlength, false));
                        }//if

                        matchlength = 1;
                    }//if
                }//for
            }//for
        }//FindAllMatches

        //private delegate void AffectCluster(int index, int x, int y, Match match);
        private void DeletAllMatchedTiles()
        {
            for (int i = 0; i < allMatches.Count; i++)
            {
                Match match = allMatches[i];
                int coffset = 0;
                int roffset = 0;
                int nx = match.x + coffset; 
                int ny = match.y + roffset;
                for (int j = 0; j < match.length; j++)
                {
                    nx = match.x + coffset;
                    ny = match.y + roffset;

                    tileGrid[nx,ny].tileType = -1;
                    tileGrid[nx, ny].prefab.SetActive(false);
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
        }//DeletAllMatchedTiles

    }//MatchGame
}//namespace
