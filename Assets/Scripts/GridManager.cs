using UnityEngine;

public class GridManager : MonoBehaviour
{
    [Header("Settings")]
    public int width = 5;
    public int height = 5;
    public float cellSize = 1.1f; 
    
    [Header("References")]
    public GameObject tilePrefab; 
    // Removed: public Transform cam; (We don't need to control the camera anymore)

    public Tile[,] _gridArray; // Made public so Spawner can read it

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        _gridArray = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float posX = x * cellSize;
                float posY = y * cellSize;
                
                GameObject newTileObj = Instantiate(tilePrefab, new Vector3(posX, posY), Quaternion.identity);
                newTileObj.name = $"Tile {x},{y}";
                
                // IMPORTANT: Parent them to THIS object so they move when we move the manager
                newTileObj.transform.parent = this.transform; 
                newTileObj.transform.localPosition = new Vector3(posX, posY, 0); // Use localPosition!

                Tile tileScript = newTileObj.GetComponent<Tile>();
                tileScript.x = x;
                tileScript.y = y;

                _gridArray[x, y] = tileScript;
            }
        }
        // Camera code deleted here. You are free now!
    }

    public Tile GetTileAtPosition(Vector2 worldPosition)
    {
        // Now we need to account for the grid moving! 
        // We subtract the grid's own position to find the "local" position of the mouse
        Vector2 localPos = worldPosition - (Vector2)transform.position;

        int x = Mathf.RoundToInt(localPos.x / cellSize);
        int y = Mathf.RoundToInt(localPos.y / cellSize);

        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return _gridArray[x, y];
        }

        return null;
    }
}