using Cysharp.Threading.Tasks;
using DG.Tweening;
using Gameplay.CollectableSystem;
using Gameplay.GameData;
using Gameplay.MovementSystem;
using Gameplay.PlaceableSystem;
using UnityEngine;

namespace Gameplay.MergeableSystem
{
    public class MergeableItem : PlaceableItem, IMergeable, IMoveable, IAnimatedMoveable
    {
        public MergeableDataSO MergeableData { get; set; }

        public MergeableType MergeType => MergeableData.MergeType;

        public int Level => MergeableData.Level;

        private GameObject _visual;

        public Transform VisualTransform => _visual.transform;

        private AddressablePool _mergeablePool;
        private AddressablePool _visualPool;

        public IShoveable Shoveable { get; private set; }
        public ICollectable Collectable { get; private set; }

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

            if (MergeableData.Level < StaticGameData.MAX_ITEM_LEVEL)
            {
                Shoveable = new ShoveableItem(this);
            }
            else
            {
                Collectable = new CollectableItem(this);
            }
        }

        public override void ResetItem()
        {
            base.ResetItem();

            ReleasePreviousCells();

            _visualPool.ReturnItem(_visual);

            Shoveable = null;
            Collectable = null;

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