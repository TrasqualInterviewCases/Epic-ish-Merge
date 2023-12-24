using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.PlaceableSystem
{
    [CreateAssetMenu(menuName = "PlaceableItemData")]
    public class PlaceableItemDataSO : ScriptableObject
    {
        public List<Vector2Int> PlacementMap;
        public PlacementFillType PlacementType;
    }
}
