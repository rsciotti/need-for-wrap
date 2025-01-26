using Main.Scripts;
using Ricky.Scripts;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

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

    public int PopWithinRadius(Vector2 center, float radius) {
        int poppedBubbles = 0;

        // Higher scale will leave some bubbles unpopped (0.5f to pop all bubbles)
        IEnumerable<Vector2> pointsInCircle = PointsInCircle(center, radius, 1f);
        foreach (Vector2 point in pointsInCircle) {
            if (PopAtLocation(point)) {
                poppedBubbles++;
            }
        }

        return poppedBubbles;
    }

    private bool InCircle(Vector2 point, Vector2 circlePoint, float radius) {
        return (point - circlePoint).sqrMagnitude <= radius * radius;
    }

    private IEnumerable<Vector2> PointsInCircle(Vector2 circlePos, float radius, float scale) {
        var minX = circlePos.x - radius;
        var maxX = circlePos.x + radius;
    
        var minY = circlePos.y - radius;
        var maxY = circlePos.y + radius;

        for (var y = minY; y <= maxY; y += scale) {
            for (var x = minX; x <= maxX; x += scale) {
                if (InCircle(new Vector2(x, y), circlePos, radius)) {
                    yield return new Vector2(x, y);
                }
            }
        }
    }
}
