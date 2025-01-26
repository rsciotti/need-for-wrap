using Main.Scripts;
using Ricky.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WrapController : MonoBehaviour
{

    private Tilemap _tileMap;

    [SerializeField] private BubbleTile poppedTile;
    [SerializeField] private BubbleTile unpoppedTile;

    [SerializeField] private AudioClip[] poppingSounds;

    void Start()
    {
        _tileMap = transform.Find("Tilemap").gameObject.GetComponent<Tilemap>();
    }
    
    
    /// <summary>
    /// Pops bubble at location.  Returns true if it was just popped; false if it was already popped.
    /// </summary>
    /// <param name="topLeft"></param>
    /// <param name="bottomRight"></param>
    /// <returns>Number of popped bubbles</returns>
    public bool PopAtLocation(Vector2 location)
    {
        Vector3Int cell = _tileMap.WorldToCell(location);
        BubbleTile oldTile = _tileMap.GetTile<BubbleTile>(cell);
        if (!oldTile || oldTile.popped)
        {
            return false;
        }
        _tileMap.SetTile(cell, poppedTile);
        SoundManager.Instance.PlaySoundEffect(poppingSounds[Random.Range(0, 100) % poppingSounds.Length]);
        
        return true;
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
