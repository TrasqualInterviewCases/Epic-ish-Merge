using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.MovementSystem;
using Gameplay.PlaceableSystem;
using System;
using UnityEngine;

namespace Gameplay.MergeableSystem
{
    public class MergeableItem : PlaceableItem, IMergeable, IMoveable, IAnimatedMoveable
    {
        public Action<MergeableItem> OnMergeableReset;

        public MergeableDataSO MergeableData { get; set; }

        public MergeableType MergeType => MergeableData.MergeType;

        public int Level => MergeableData.Level;

        private GameObject _visual;

        private AddressablePool _mergeablePool;
        private AddressablePool _visualPool;

        public IShoveable Shoveable { get; private set; }

        public async void Init(AddressablePool mergeablePool, AddressablePool visualPool)
        {
            _mergeablePool = mergeablePool;
            _visualPool = visualPool;

            _visual = await visualPool.GetItem();
            _visual.transform.SetParent(transform);

            Vector2 centerPos = Vector2.zero;
            for (int i = 0; i < MergeableData.PlacementMap.Count; i++)
            {
                centerPos += MergeableData.PlacementMap[i];
            }
            centerPos /= MergeableData.PlacementMap.Count;

            _visual.transform.localPosition = new Vector3(centerPos.x, 0f, centerPos.y) * _gridManager.CellSize;

            if (MergeableData.Level < 2)
            {
                Shoveable = gameObject.AddComponent<ShoveableItem>();

                Shoveable.Initialize(this);
            }
        }

        public void ResetItem()
        {
            OnMergeableReset?.Invoke(this);

            ReleasePreviousCells();

            _visualPool.ReturnItem(_visual);

            Destroy(Shoveable as ShoveableItem);
            Shoveable = null;

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
    }
}