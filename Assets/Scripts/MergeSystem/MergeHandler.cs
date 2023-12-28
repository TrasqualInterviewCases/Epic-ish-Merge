using Cysharp.Threading.Tasks;
using Gameplay.GameData;
using Gameplay.GridSystem;
using Gameplay.ServiceSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utilities.Events;

namespace Gameplay.MergeableSystem
{
    public static class MergeHandler
    {
        public static async void Merge(GridCell centerCell)
        {
            EventManager.Instance.TriggerEvent<MergeStartedEvent>();

            const int MAX_ITEM_LEVEL = StaticGameData.MAX_ITEM_LEVEL;
            const int MIN_ITEM_TO_MERGE = StaticGameData.MIN_ITEM_TO_MERGE;

            List<MergeableItem> mergeables = GridHelper.MergeableItems.OrderBy(x => x.Level).ToList();

            UniTask[] tasks = new UniTask[mergeables.Count];

            for (int i = 0; i < mergeables.Count; i++)
            {
                tasks[i] = mergeables[i].MoveWithAnimation(centerCell.GetWorldPosition());
            }

            await UniTask.WhenAll(tasks);

            int itemLevels = mergeables.Select(x => x.Level).Distinct().Count();

            for (int i = 0; i < MAX_ITEM_LEVEL; i++)
            {
                List<MergeableItem> sameLevelMergeables = mergeables.FindAll(x => x.Level == i).ToList();

                if (sameLevelMergeables.Count >= MIN_ITEM_TO_MERGE)
                {
                    int amountOfHigherLevelMergeablesToCreate = Mathf.FloorToInt(sameLevelMergeables.Count / MIN_ITEM_TO_MERGE);

                    for (int j = 0; j < amountOfHigherLevelMergeablesToCreate; j++)
                    {
                        MergeableItem higherLevelMergeable = await ServiceProvider.Instance.MergeableFactory.GetMergableItem(mergeables[0].MergeType, sameLevelMergeables[0].Level + 1);

                        mergeables.Add(higherLevelMergeable);
                    }

                    for (int k = 0; k < amountOfHigherLevelMergeablesToCreate * MIN_ITEM_TO_MERGE; k++)
                    {
                        mergeables.Remove(sameLevelMergeables[k]);
                        sameLevelMergeables[k].Merge();
                    }
                }
            }

            for (int l = 0; l < mergeables.Count; l++)
            {
                if (mergeables[l].TryPlaceInCell(centerCell))
                {
                    mergeables[l].Move(centerCell.GetWorldPosition());
                }
                else
                {
                    PlaceMergeable(mergeables[l]);
                }
            }

            await UniTask.Delay(mergeables.Count * 20);

            EventManager.Instance.TriggerEvent<MergeEndedEvent>();
        }

        private static void PlaceMergeable(MergeableItem mergeable)
        {
            var availableCell = ServiceProvider.Instance.GridManager.GetRandomAvailableCell();

            if (mergeable.TryPlaceInCell(availableCell))
            {
                mergeable.Move(availableCell.GetWorldPosition());
            }
            else
            {
                PlaceMergeable(mergeable);
            }
        }
    }
}