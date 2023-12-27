using Gameplay.PlaceableSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Gameplay.MergeableSystem
{
    [CreateAssetMenu(menuName = "MergeableData")]
    public class MergeableDataSO : PlaceableItemDataSO
    {
        public AssetReference ItemPrefab;
        public MergeableType MergeType;
        public int Level;
    }
}
