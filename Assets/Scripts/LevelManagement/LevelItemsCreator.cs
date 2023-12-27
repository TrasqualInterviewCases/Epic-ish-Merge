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

        private void OnGridGenerated(object data)
        {
            GridManager gridManager = ((GridGeneratedEvent)data).GridManager;

            for (int i = 0; i < _itemCount; i++)
            {
                GridCell cell = gridManager.GetRandomActiveCell();

                MergeableItem mergeable = ServiceProvider.Instance.MergeableFactory.GetRandomMergeable(0);
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