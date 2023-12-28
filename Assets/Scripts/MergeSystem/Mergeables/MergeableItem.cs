using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.GridSystem;
using Gameplay.MovementSystem;
using Gameplay.PlaceableSystem;
using UnityEngine;

namespace Gameplay.MergeableSystem
{
    public class MergeableItem : PlaceableItem, IMergeable, IMoveable, IAnimatedMoveable, IShoveable
    {
        public MergeableDataSO MergeableData { get; set; }

        public MergeableType MergeType => MergeableData.MergeType;

        public int Level => MergeableData.Level;

        private GameObject _visual;

        private AddressablePool _mergeablePool;
        private AddressablePool _visualPool;

        public async void Init(AddressablePool mergeablePool, AddressablePool visualPool)
        {
            _mergeablePool = mergeablePool;
            _visualPool = visualPool;

            _visual = await visualPool.GetItem();
            _visual.transform.SetParent(transform);
            _visual.transform.localPosition = Vector3.zero;
        }

        public void ResetItem()
        {
            ReleasePreviousCells();

            _visualPool.ReturnItem(_visual);

            _visual = null;
            MergeableData = null;
            Data = null;

            _mergeablePool.ReturnItem(gameObject);

            _visualPool = null;
            _mergeablePool = null;
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

        public async UniTask MoveWithAnimation(Vector3 movementVector)
        {
            await transform.DOMove(movementVector, 0.2f).AsyncWaitForCompletion();
        }

        public void Shove(GridCell cell)
        {
            GridCell nearestEmptyCell = GridHelper.FindNearestEmptyCell(cell);
            if (nearestEmptyCell != null)
            {
                TryPlaceInCell(nearestEmptyCell);
                MoveWithAnimation(nearestEmptyCell.GetWorldPosition()).Forget();
            }
        }
    }
}