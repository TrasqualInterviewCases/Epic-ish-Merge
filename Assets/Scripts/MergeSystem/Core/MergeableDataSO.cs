using Gameplay.PlaceableSystem;
using UnityEngine;

namespace Gameplay.MergeableSystem
{
    [CreateAssetMenu(menuName = "MergeableData")]
    public class MergeableDataSO : PlaceableItemDataSO
    {
        public GameObject ItemPrefab;
        public MergeableType MergeType;
        public int Level;
    }
}
