using Cysharp.Threading.Tasks;
using Gameplay.GridSystem;
using Gameplay.MergeableSystem;
using Gameplay.ServiceSystem;
using UnityEngine;
using Utilities.Events;

namespace Gameplay.LevelManagement
{
    public class LevelItemsCreator : MonoBehaviour
    {
        [SerializeField] private int _itemCount;

        private void Awake()
        {
            ListenEvents();
        }

        private void ListenEvents()
        {
            EventManager.Instance.AddListener<GridGeneratedEvent>(OnGridGenerated);
        }

        private async void OnGridGenerated(object data)
        {
            GridManager gridManager = ((GridGeneratedEvent)data).GridManager;

            await UniTask.WaitUntil(() => ServiceProvider.Instance.MergeableFactory.AllPoolsAreReady);

            for (int i = 0; i < _itemCount; i++)
            {
                GridCell cell = gridManager.GetRandomActiveCell();

                MergeableItem mergeable = await ServiceProvider.Instance.MergeableFactory.GetRandomMergeable(0);

                if (mergeable.TryPlaceInCell(cell))
                {
                    mergeable.Move(cell.GetWorldPosition());
                }         
            }
        }

        private void UnsubscribeToEvents()
        {
            EventManager.Instance.RemoveListener<GridGeneratedEvent>(OnGridGenerated);
        }

        private void OnDestroy()
        {
            UnsubscribeToEvents();
        }
    }
}