using Gameplay.MovementSystem;
using Gameplay.PlaceableSystem;
using UnityEngine;

namespace Gameplay.MergeableSystem
{
    public class MergeableItem : PlaceableItem, IMergeable, IMoveable
    {
        public MergeableDataSO MergeableData { get; set; }

        public MergeableType MergeType => MergeableData.MergeType;
        public int Level => MergeableData.Level;

        private GameObject _visual;

        private void Awake()
        {
            _visual = Instantiate(Data.ItemPrefab, transform.position, transform.rotation, transform);
        }

        public void ResetItem()
        {
            Destroy(_visual.gameObject);
            _visual = null;
            MergeableData = null;
            Data = null;
        }

        public void Merge()
        {
            ResetItem();
        }

        public void Move(Vector3 position)
        {
            transform.position = position;
        }
    }
}