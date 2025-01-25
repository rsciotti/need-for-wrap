using UnityEngine;
using UnityEngine.Tilemaps;

public class WrapController : MonoBehaviour
{

    private Tilemap _tileMap;

    [SerializeField] private TileBase[] unpoppedTiles;
    [SerializeField] private TileBase[] poppedTiles;
    
    void Start()
    {
        _tileMap = transform.Find("Tilemap").gameObject.GetComponent<Tilemap>();
    }
    
    void Update()
    {
        PopAtLocation(Camera.main.ScreenToWorldPoint(Input.mousePosition));
    }

    public void PopAtLocation(Vector2 location)
    {
        Vector3Int cell = _tileMap.WorldToCell(location);
        _tileMap.SetTile(cell, poppedTiles[Random.Range(0, 100) % poppedTiles.Length]);
    }

    /// <summary>
    /// Pops all bubble wrap tiles within a rectangle
    /// </summary>
    /// <param name="topLeft"></param>
    /// <param name="bottomRight"></param>
    /// <returns>Number of popped bubbles</returns>
    public int PopWithinBounds(Vector2 topLeft, Vector2 bottomRight)
    {
        Vector3Int topLeftBounds = _tileMap.WorldToCell(topLeft);
        Vector3Int bottomRightBounds = _tileMap.WorldToCell(bottomRight);
        int poppedBubbles = 0;
        
        for (int x = topLeftBounds.x; x < bottomRightBounds.x; x++)
        {
            for (int y = topLeftBounds.y; y < bottomRightBounds.y; y++)
            {
                PopAtLocation(new Vector2(x, y));
                poppedBubbles++;
            }
        }

        return poppedBubbles;
    }
}
