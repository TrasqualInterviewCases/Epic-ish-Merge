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

        public void Init()
        {
            _visual = Instantiate(MergeableData.ItemPrefab, transform.position, transform.rotation, transform);
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

        public override void SetData(PlaceableItemDataSO mergeableData)
        {
            base.SetData(mergeableData);

            MergeableData = mergeableData as MergeableDataSO;
        }
    }
}