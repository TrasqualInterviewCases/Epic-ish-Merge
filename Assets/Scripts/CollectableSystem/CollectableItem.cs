using Cysharp.Threading.Tasks;
using Gameplay.MergeableSystem;
using System;
using UnityEngine;

namespace Gameplay.CollectableSystem
{
    public class CollectableItem : ICollectable, IDisposable
    {
        public static Action<CollectableItem> OnCollectionStarted;

        private MergeableItem _mergeable;

        private bool _isCollecting;

        private const int COLLECTION_TIME = 4000;

        public float Duration => COLLECTION_TIME / 1000f;

        public Transform Transform => _mergeable.VisualTransform;

        public CollectableItem(MergeableItem mergeable)
        {
            _mergeable = mergeable;
        }

        public async void StartCollecting()
        {
            if (_isCollecting)
            {
                return;
            }

            _isCollecting = true;

            OnCollectionStarted?.Invoke(this);

            await UniTask.Delay(COLLECTION_TIME);

            FinishCollecting();
        }

        public void FinishCollecting()
        {
            _mergeable.ResetItem();
        }

        public void Dispose()
        {
            _isCollecting = false;
            _mergeable = null;
        }
    }
}