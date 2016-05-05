using UnityEngine;
using System.Collections.Generic;
using Useless;
using Useless.Match3;

public class GridManager : MonoBehaviour
{

 /*   public class XY
    {
        public int X;
        public int Y;

        public XY (int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    
    public class Tile
    {
        public int tileType;
        public GameObject prefab;
        public TileControl tileControl;

        public Tile ()
        {            
            tileType = -1;            
        }//Constructor1

        public Tile (int tileType, GameObject prefab, TileControl tileControl)
        {
            this.tileType = tileType;
            this.prefab = prefab;
            this.tileControl = tileControl;
        }//Constructor2
    }//Tile
    
    public GameObject[] TilePrefabs;

    public int gridWidth;
    public int gridHeight;
    public Tile[,] Grid;
    
    
    private int movingTiles;

    void Awake()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        Grid = new Tile[gridWidth, gridHeight];        

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                
                int randomtileType = Random.Range(0, TilePrefabs.Length);
                
                GameObject go = Instantiate(TilePrefabs[randomtileType], new Vector2(x, y), Quaternion.identity) as GameObject;
                TileControl tileControl = go.GetComponent<TileControl>();                
                Grid[x, y] = new Tile(randomtileType, go, tileControl);
                tileControl.matchManager = this;
                tileControl.gridPosition = new UPoint(x, y);
                go.name = x + "/" + y;

                go.transform.parent = transform;
            }//for
        }//for
       CheckMatches();
    }//CreateGrid

    
    public void SwitchTiles(UPoint firstXY, UPoint secondXY)
    {
        Tile firstTile = new Tile(Grid[firstXY.ix, firstXY.iy].tileType, Grid[firstXY.ix, firstXY.iy].prefab, Grid[firstXY.ix, firstXY.iy].tileControl);
        Tile secondTile = new Tile(Grid[secondXY.ix, secondXY.iy].tileType, Grid[secondXY.ix, secondXY.iy].prefab, Grid[secondXY.ix, secondXY.iy].tileControl);

        Grid[firstXY.ix, firstXY.iy] = secondTile;
        Grid[secondXY.ix, secondXY.iy] = firstTile;        
    }//SwitchTiles
    

    public void CheckMatches()
    {
        List<UPoint> checkingTiles = new List<UPoint>(); // Tiles that are currently being considered for a match-3.
        List<UPoint> tilesToDestroy = new List<UPoint>(); // Tiles that are confirmed match-3s and will be destroyed.
        
        // Vertical check
        for (int x = 0; x < gridWidth; x++)
        {
            int currenttileType = -1;
            int lasttileType = -1;

            if (checkingTiles.Count >= 3)
                tilesToDestroy.AddRange(checkingTiles);

            checkingTiles.Clear();

            for (int y = 0; y < gridHeight; y++)
            {
                currenttileType = Grid[x, y].tileType;                

                if (currenttileType != lasttileType)
                {
                    if (checkingTiles.Count >= 3)
                        tilesToDestroy.AddRange(checkingTiles);

                    checkingTiles.Clear();
                }//if

                checkingTiles.Add(new UPoint(x,y));
                lasttileType = currenttileType;
            }//for
        }//for

        checkingTiles.Clear();

        // Horizontal check
        for (int y = 0; y < gridHeight; y++)
        {
            int currenttileType = -1;
            int lasttileType = -1;

            if (checkingTiles.Count >= 3)
            {
                for (int i = 0; i < checkingTiles.Count; i++)
                {
                    if (!tilesToDestroy.Contains(checkingTiles[i]))
                        tilesToDestroy.Add(checkingTiles[i]);
                }//for
            }//if

            checkingTiles.Clear();

            for (int x = 0; x < gridWidth; x++)
            {
                currenttileType = Grid[x, y].tileType;

                if (currenttileType != lasttileType)
                {
                    if (checkingTiles.Count >= 3)
                    {
                        for (int i = 0; i < checkingTiles.Count; i++)
                        {
                            if (!tilesToDestroy.Contains(checkingTiles[i]))
                                tilesToDestroy.Add(checkingTiles[i]);

                        }//for
                    }//if

                    checkingTiles.Clear();
                }//if

                checkingTiles.Add(new UPoint(x, y));
                lasttileType = currenttileType;
            }//for
        }//for

        if (tilesToDestroy.Count != 0)
            DestroyMatches(tilesToDestroy);
        else
            ReplaceTiles();
            
    }//CheckMatches

    
    void DestroyMatches (List<UPoint> tilesToDestroy)
    {
        for (int i = 0; i < tilesToDestroy.Count; i++)
        {            
            Destroy(Grid[tilesToDestroy[i].ix, tilesToDestroy[i].iy].prefab);
            Grid[tilesToDestroy[i].ix, tilesToDestroy[i].iy] = new Tile();
        }//for
        GravityCheck();
    }//DestroyMatches
    
    void GravityCheck()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            int missingTileCount = 0;

            for (int y = 0; y < gridHeight; y++)
            {
                if (Grid[x, y].tileType == -1)
                    missingTileCount++;
                else
                {
                    if (missingTileCount >= 1)
                    {
                        Tile tile = new Tile(Grid[x, y].tileType, Grid[x, y].prefab, Grid[x, y].tileControl);
                        Grid[x, y].tileControl.Move(new UPoint(x, y - missingTileCount));
                        Grid[x, y - missingTileCount] = tile;
                        Grid[x, y] = new Tile();                        
                    }//if
                }//else
            }//for            
        }//for        
        ReplaceTiles();
    }//GravityCheck
    
    void ReplaceTiles()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            int missingTileCount = 0;

            for (int y = 0; y < gridHeight; y++)
            {
                if (Grid[x, y].tileType == -1)
                    missingTileCount++;
            }//for

            Debug.Log("missing tile count is " + missingTileCount + " in x #" + x);

            for (int i = 0; i < missingTileCount; i++)
            {
                int tileY = gridHeight - missingTileCount + i;
                int randomtileType = Random.Range(0, TilePrefabs.Length);
                GameObject go = Instantiate(TilePrefabs[randomtileType], new Vector2(x, gridHeight + i), Quaternion.identity) as GameObject;              
                TileControl tileControl = go.GetComponent<TileControl>();
                tileControl.matchManager = this;
                tileControl.Move(new UPoint(x, tileY));
                Grid[x, tileY] = new Tile(randomtileType, go, tileControl);
                go.name = x + "/" + tileY;
            }//for
        }//for
    }//ReplaceTiles

    public bool MoveIsLegal(UPoint from, UPoint to)
    {
        Tile[,] tempGrid = new Tile[gridWidth, gridHeight];
        //Grid.CopyTo(tempGrid, 0);
        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                tempGrid[x, y] = Grid[x, y];
            }//for
        }//for
        
        //Swap them on the temp grid
        Tile tempTile = Grid[to.ix, to.iy];
        Grid[to.ix, to.iy] = Grid[from.ix, from.iy];
        Grid[from.ix, from.iy] = tempTile;

        //Now check the temp grid for matches
        List<UPoint> checkingTiles = new List<UPoint>(); // Tiles that are currently being considered for a match-3.
        //List<UPoint> matchedTiles = new List<UPoint>(); // Tiles that are confirmed match-3s.

        // Vertical check
        for (int x = 0; x < gridWidth; x++)
        {
            int currenttileType = -1;
            int lasttileType = -1;

            if (checkingTiles.Count >= 3)
                return true;

            checkingTiles.Clear();

            for (int y = 0; y < gridHeight; y++)
            {
                currenttileType = tempGrid[x, y].tileType;

                if (currenttileType != lasttileType)
                {
                    if (checkingTiles.Count >= 3)
                        return true;
                        //matchedTiles.AddRange(checkingTiles);

                    checkingTiles.Clear();
                }//if

                checkingTiles.Add(new UPoint(x, y));
                lasttileType = currenttileType;
            }//for
        }//for

        checkingTiles.Clear();

        // Horizontal check
        for (int y = 0; y < gridHeight; y++)
        {
            int currenttileType = -1;
            int lasttileType = -1;

            if (checkingTiles.Count >= 3)
                return true;

            checkingTiles.Clear();

            for (int x = 0; x < gridWidth; x++)
            {
                currenttileType = tempGrid[x, y].tileType;

                if (currenttileType != lasttileType)
                {
                    if (checkingTiles.Count >= 3)
                        return true;

                    checkingTiles.Clear();
                }//if

                checkingTiles.Add(new UPoint(x, y));
                lasttileType = currenttileType;
            }//for
        }//for
       
        return false;
    }//MoveIsLegal
    
    public void ReportTileMovement ()
    {
        movingTiles++;
    }//ReportTileMovement

    public void ReportTileStopped ()
    {
        movingTiles--;

        if (movingTiles == 0)
            CheckMatches();
    }//ReportTileStopped

    public void CheckForLegalMoves()
    {
        // Vertical check
        for (int x = 0; x < gridWidth; x++)
        {
            int secondToLastType = -1;
            int lastType = -2;
            int currentType = -3;

            for (int y = 0; y < gridHeight; y++)
            {
                currentType = Grid[x, y].tileType;
                if (lastType == currentType)
                {
                    if (CheckForTileType(x, y - 3, currentType))
                        return;
                    if (CheckForTileType(x + 1, y - 2, currentType))
                        return;
                    if (CheckForTileType(x - 1, y - 2, currentType))
                        return;
                    if (CheckForTileType(x, y + 2, currentType))
                        return;
                    if (CheckForTileType(x + 1, y + 1, currentType))
                        return;
                    if (CheckForTileType(x - 1, y + 1, currentType))
                        return;
                }//if
                else if (secondToLastType == currentType)
                {
                    if (CheckForTileType(x + 1, y - 1, currentType))
                        return;
                    if (CheckForTileType(x - 1, y - 1, currentType))
                        return;
                }//else if
                secondToLastType = lastType;
                lastType = currentType;
            }//for
        }//for

        // Horizontal check
        for (int y = 0; y < gridHeight; y++)
        {
            int secondToLastType = -1;
            int lastType = -2;
            int currentType = -3;

            for (int x = 0; x < gridWidth; x++)
            {
                currentType = Grid[x, y].tileType;
                if (lastType == currentType)
                {
                    if (CheckForTileType(x - 3, y, currentType))
                        return;
                    if (CheckForTileType(x - 2, y + 1, currentType))
                        return;
                    if (CheckForTileType(x - 2, y - 1, currentType))
                        return;
                    if (CheckForTileType(x + 2, y, currentType))
                        return;
                    if (CheckForTileType(x + 1, y + 1, currentType))
                        return;
                    if (CheckForTileType(x + 1, y - 1, currentType))
                        return;
                }//if
                else if (secondToLastType == currentType)
                {
                    if (CheckForTileType(x - 1, y + 1, currentType))
                        return;
                    if (CheckForTileType(x - 1, y - 1, currentType))
                        return;
                }//else if
                secondToLastType = lastType;
                lastType = currentType;
            }//for
        }//for

        // No matches? Shuffle!
        ShuffleGrid();
    }//CheckForLegalMoves

    private bool CheckForTileType(int x, int y, int tileType)
    {
        if (x >= 0 && x < gridWidth && y >= 0 && y < gridHeight)
            return Grid[x, y].tileType == tileType;
        else
            return false;
    }//CheckForTileType

    private void ShuffleGrid()
    {
        List<UPoint> xyList = new List<UPoint>();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridWidth; y++)
            {
                xyList.Add(new UPoint(x, y));
            }//for
        }//for

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridWidth; y++)
            {
                System.Random rnd = new System.Random();
                int index = rnd.Next(xyList.Count);
                UPoint xy = xyList[index];
                Grid[x, y].tileControl.Move(xy);
                xyList.RemoveAt(index);
            }//for
        }//for
    }//ShuffleGrid
    */
}