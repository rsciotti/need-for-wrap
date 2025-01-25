using System;
using UnityEngine.Tilemaps;

namespace Ricky.Scripts
{
    using UnityEngine;
    
    [CreateAssetMenu]
    public class BubbleTile : TileBase
    {
        public bool popped;
        public Sprite[] spriteVariations;

        public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
        {
            // Determine variation based on position with hashing function
            int variation = Math.Abs((int)(Math.Sin(position.x + position.y * 0.5) * 1000)) % spriteVariations.Length;
            tileData.sprite = spriteVariations[variation];
        }
    }
}