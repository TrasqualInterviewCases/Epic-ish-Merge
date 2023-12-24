using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.PlaceableSystem
{
    [CreateAssetMenu(menuName = "PlaceableItemData")]
    public class PlaceableItemDataSO : ScriptableObject
    {
        public GameObject ItemPrefab;
        public List<Vector2Int> PlacementMap;
        public PlacementType PlacementType;
    }
}
