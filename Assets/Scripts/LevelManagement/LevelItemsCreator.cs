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

        MergeableFactory _mergeableFactory;

        private void Awake()
        {
            ListenEvents();
        }

        private void Start()
        {
            _mergeableFactory = ServiceProvider.Instance.MergeableFactory;
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
  
                MergeableItem mergeable = _mergeableFactory.GetRandomMergeable();
                mergeable.TryPlaceInCell(cell);
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