using Cysharp.Threading.Tasks;
using Gameplay.GridSystem;
using Gameplay.MergeableSystem;
using Gameplay.PlaceableSystem;
using Gameplay.ServiceSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.Events;

namespace Gameplay.LevelManagement
{
    public class LevelMergeablesCreator : MonoBehaviour
    {
        [SerializeField] private int _initialItemCount = 10;

        private GridManager _gridManager;

        private Dictionary<MergeableType, int> _mergeableCounts = new Dictionary<MergeableType, int>();

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
            _gridManager = ((GridGeneratedEvent)data).GridManager;

            await UniTask.WaitUntil(() => ServiceProvider.Instance.MergeableFactory.AllPoolsAreReady);

            for (int i = 0; i < _initialItemCount; i++)
            {
                GridCell cell = _gridManager.GetRandomActiveCell();

                MergeableItem mergeable = await ServiceProvider.Instance.MergeableFactory.GetRandomMergeable(0);

                mergeable.OnPlacedInCell += OnPlaceInCell;
                mergeable.OnMergeableReset += OnMergeableReset;

                if (mergeable.TryPlaceInCell(cell))
                {
                    mergeable.Move(cell.GetWorldPosition());
                }
                else
                {
                    mergeable.OnPlacedInCell -= OnPlaceInCell;
                    mergeable.OnMergeableReset -= OnMergeableReset;
                }
            }
        }

        private void OnPlaceInCell(PlaceableItem item)
        {
            MergeableItem mergeable = item as MergeableItem;
            AdjustMergeableCount(mergeable.MergeableData.MergeType, 1);
        }

        private void OnMergeableReset(MergeableItem item)
        {
            AdjustMergeableCount(item.MergeableData.MergeType, -1);

            item.OnPlacedInCell -= OnPlaceInCell;
            item.OnMergeableReset -= OnMergeableReset;

            GenerateNewMergeable();
        }

        private void AdjustMergeableCount(MergeableType mergeType, int amount)
        {
            if (_mergeableCounts.ContainsKey(mergeType))
            {
                _mergeableCounts[mergeType] += amount;
            }
            else
            {
                _mergeableCounts.Add(mergeType, amount);
            }
        }

        private async void GenerateNewMergeable()
        {
            List<int> counts = _mergeableCounts.Values.OrderByDescending(x => x).ToList();

            MergeableType mergeableTypeToCreate = _mergeableCounts.FirstOrDefault(x => x.Value == counts[0]).Key;

            GridCell cell = _gridManager.GetRandomActiveCell();

            MergeableItem mergeable = await ServiceProvider.Instance.MergeableFactory.GetRandomMergeable(0);

            mergeable.OnPlacedInCell += OnPlaceInCell;
            mergeable.OnMergeableReset += OnMergeableReset;

            if (mergeable.TryPlaceInCell(cell))
            {
                mergeable.Move(cell.GetWorldPosition());
            }
            else
            {
                mergeable.OnPlacedInCell -= OnPlaceInCell;
                mergeable.OnMergeableReset -= OnMergeableReset;
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