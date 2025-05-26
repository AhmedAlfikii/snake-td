using System.Collections.Generic;
using UnityEngine;

namespace TafraKit
{
    [System.Serializable]
    public class TilesGroup
    {
        [SerializeField] private List<Vector2Int> additionalTiles = new List<Vector2Int>();

        public List<Vector2Int> AdditionalTiles => additionalTiles;
    }
}