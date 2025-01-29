using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MazeGenerator : MonoBehaviour
{
    public int width, height;
    public TileBase wallTile;
    public Tilemap tilemap;
    public TileBase floorTile;
    public Tilemap floorTilemap;
    public GameObject player;
    public GameObject exitTrigger;
    public Camera minimapCamera;
    public RawImage minimapRawImage;
    public float minimapRawImageAdjuster;

    private int[,] Maze;
    private Stack<Vector2> _tiletoTry = new Stack<Vector2>();
    private List<Vector2> offsets = new List<Vector2>() { new Vector2(0,1), new Vector2(0,-1), new Vector2(1,0), new Vector2(-1,0) };
    private System.Random rnd = new System.Random();
    private Vector2 entrance;
    private Vector2 exit;

    private Vector2 CurrentTile;

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.orthographic = true;
        AdjustCameraToMaze();
        AdjustMinimapCameraToMaze();
        AdjustRawImage();
        tilemap.ClearAllTiles();
        GenerateMaze();
        SetFloorTiles();
        BlockMazeEntrance();
        PlacePlayerAtEntrance();
        PlaceExitTrigger();
    }

    void AdjustRawImage()
    {
        minimapRawImage.transform.localScale = new Vector2 (width/minimapRawImageAdjuster, height/minimapRawImageAdjuster);
    }

    void AdjustCameraToMaze()
    {
        Camera.main.orthographicSize = (height / 2f) + 1f;
        float screenAspect = (float)Screen.width / Screen.height;
        float cameraWidth = Camera.main.orthographicSize * screenAspect;
        if (cameraWidth < (width / 1f) + 1f)
        {
            Camera.main.orthographicSize = ((width / 3f) + 0.5f) / screenAspect;
        }
        Camera.main.transform.position = new Vector3((width / 2f) - 0.5f, (height / 2f) - 0.5f, -10f);
        Camera.main.transform.position = new Vector3(player.gameObject.transform.position.x, player.gameObject.transform.position.y, Camera.main.transform.position.z);
    }

    void AdjustMinimapCameraToMaze()
    {
        minimapCamera.orthographicSize = (height / 2f) + 5f;
        float screenAspect = (float)Screen.width / Screen.height;
        float cameraWidth = minimapCamera.orthographicSize * screenAspect;
        if (cameraWidth < (width / 2f) + 0.5f)
        {
            minimapCamera.orthographicSize = ((width / 2f) + 0.5f) / screenAspect;
        }
        minimapCamera.transform.position = new Vector3((width / 2f) + 0.5f, (height / 2f) - 0.5f, -10f);
    }


    void GenerateMaze()
    {
        Maze = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Maze[x, y] = 1;
            }
        }

        Vector2 startTile = new Vector2(1, 0);
        _tiletoTry.Push(startTile);
        Maze = CreateMaze();

        SetEntranceAndExit();

        EnsureCorrectBoundary();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) 
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                if (Maze[x, y] == 1)
                {
                    tilemap.SetTile(tilePosition, wallTile);
                }
            }
        }
    }


    private void EnsureCorrectBoundary()
    {
        int floorTileCount = 0;

        for (int x = 0; x < width; x++)
        {
            if (Maze[x,0] == 0) floorTileCount++;
            if (Maze[x,height-1] == 0) floorTileCount++;
        }
        for (int y = 0; y < height; y++) 
        {
            if (Maze[0,y] == 0) floorTileCount++;
            if (Maze[width - 1,y] == 0) floorTileCount++;
        }

        if (floorTileCount > 2)
        {
            Maze[1, 0] = 1;
        }
    }


    private void SetEntranceAndExit()
    {
        if (rnd.Next(2) == 0)
        {
            entrance = new Vector2(1, 0);
            exit = new Vector2(width - 2, height - 1);
        }
        else 
        {
            entrance = new Vector2(width - 2, 0);
            exit = new Vector2(1, height - 1);
        }

        Maze[(int)entrance.x, (int)entrance.y] = 0;
        Maze[(int)exit.x, (int)exit.y] = 0;

        Debug.Log("Entrance: " + entrance);
        Debug.Log("Exit: " + exit);
    }


    public int[,] CreateMaze()
    {
        List<Vector2> neighbors;
        CurrentTile = new Vector2(1, 0);
        _tiletoTry.Push(CurrentTile);

        while (_tiletoTry.Count > 0)
        {
            Maze[(int)CurrentTile.x, (int)CurrentTile.y] = 0;

            neighbors = GetValidNeighbors(CurrentTile);

            if (neighbors.Count > 0) 
            {
                _tiletoTry.Push(CurrentTile);
                CurrentTile = neighbors[rnd.Next(neighbors.Count)];
                _tiletoTry.Push(CurrentTile);
                Maze[(int)CurrentTile.x, (int)CurrentTile.y] = 0;
            }
            else 
            {
                CurrentTile = _tiletoTry.Pop();
            }
        }

        return Maze;
    }

    private List<Vector2> GetValidNeighbors(Vector2 centerTile)
    {
        List<Vector2> validNeighbors = new List<Vector2>();

        foreach (var offset in offsets)
        {
            Vector2 toCheck = new Vector2(centerTile.x + offset.x, centerTile.y + offset.y);

            if (IsInside(toCheck)) { 
                if ((toCheck.x % 2 == 1 || toCheck.y % 2 == 1) && Maze[(int)toCheck.x, (int)toCheck.y] == 1)
                {
                    if (HasThreeWallsIntact(toCheck))
                    {
                        validNeighbors.Add(toCheck);
                    }
                }
            }
        }

        return validNeighbors;
    }


    private bool IsInside(Vector2 p)
    {
        return p.x >= 0 && p.y >= 0 && p.x < width && p.y < height;
    }


    private bool HasThreeWallsIntact(Vector2 Vector2ToCheck)
    {
        int intactWallCounter = 0;

        foreach (var offset in offsets)
        {
            Vector2 neighborToCheck = new Vector2(Vector2ToCheck.x + offset.x, Vector2ToCheck.y + offset.y);

            if (IsInside(neighborToCheck) && Maze[(int)neighborToCheck.x, (int)neighborToCheck.y] == 1)
            {
                intactWallCounter++;
            } 
        }

        return intactWallCounter == 3;
    }


    private void SetFloorTiles()
    {
        // Clear any existing tiles on the floor tilemap
        floorTilemap.ClearAllTiles();

        // Iterate through the maze array
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);

                // If the maze tile is a floor (0), set the floor tile
                if (Maze[x, y] == 0)
                {
                    floorTilemap.SetTile(tilePosition, floorTile);
                }
            }
        }

    }


    private void BlockMazeEntrance()
    {
        EdgeCollider2D edgeCollider = floorTilemap.GetComponent<EdgeCollider2D>();
        if (edgeCollider == null)
        {
            edgeCollider = floorTilemap.gameObject.AddComponent<EdgeCollider2D>();
        }

        Vector3 entranceWorldPos = tilemap.CellToWorld(new Vector3Int((int)entrance.x, (int)entrance.y, 0));
        Vector2[] edgePoints = new Vector2[]
        {
            new Vector2(entranceWorldPos.x, entranceWorldPos.y),
            new Vector2(entranceWorldPos.x + 1f, entranceWorldPos.y)
        };

        edgeCollider.points = edgePoints;
    }


    private void PlacePlayerAtEntrance()
    {
        if (player != null) 
        {
            Vector3 worldPosition = tilemap.CellToWorld(new Vector3Int((int)entrance.x, (int)entrance.y, 0));

            player.transform.position = worldPosition + new Vector3(0.5f, 0.5f, 0);
        }
        else
        {
            Debug.LogError("Player GameObject is not assigned!");
        }
    }


    private void PlaceExitTrigger()
    {
        if (exitTrigger != null) 
        { 
            Vector3 exitWorldPos = tilemap.CellToWorld(new Vector3Int((int)exit.x, (int)exit.y, 0));

            exitTrigger.transform.position = exitWorldPos + new Vector3(0.5f, 0.5f, 0);
        }
        else
        {
            Debug.LogError("ExitTrigger GameObject not assigned!");
        }
    }

    //Update is called once per frame
    void Update()
    {
        
    }
}
