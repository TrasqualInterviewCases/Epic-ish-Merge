using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.GridSystem;
using Gameplay.MovementSystem;
using Gameplay.PlaceableSystem;
using System.Threading.Tasks;
using UnityEngine;

namespace Gameplay.MergeableSystem
{
    public class MergeableItem : PlaceableItem, IMergeable, IMoveable, IAnimatedMoveable, IShoveable
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
            ReleasePreviousCells();
            Destroy(_visual.gameObject);
            _visual = null;
            MergeableData = null;
            Data = null;

            //change with pool
            Destroy(gameObject);
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