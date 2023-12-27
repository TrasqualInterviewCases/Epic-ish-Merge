using Cysharp.Threading.Tasks;
using Gameplay.GridSystem;
using Gameplay.ServiceSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.MergeableSystem
{
    public static class MergeHandler
    {
        public static async void Merge(GridCell centerCell)
        {
            const int ITEM_MAX_LEVEL = 2;
            const int MIN_REQUIRED_TO_MERGE = 3;

            List<MergeableItem> mergeables = GridHelper.MergeableItems.OrderBy(x => x.Level).ToList();

            UniTask[] tasks = new UniTask[mergeables.Count];

            for (int i = 0; i < mergeables.Count; i++)
            {
                tasks[i] = mergeables[i].MoveWithAnimation(centerCell.GetWorldPosition());
            }

            await UniTask.WhenAll(tasks);

            int itemLevels = mergeables.Select(x => x.Level).Distinct().Count();

            for (int i = 0; i < ITEM_MAX_LEVEL - 1; i++)
            {
                List<MergeableItem> sameLevelMergeables = mergeables.FindAll(x => x.Level == i).ToList();

                if (sameLevelMergeables.Count >= MIN_REQUIRED_TO_MERGE)
                {
                    int amountOfHigherLevelMergeablesToCreate = Mathf.FloorToInt(sameLevelMergeables.Count / MIN_REQUIRED_TO_MERGE);

                    for (int j = 0; j < amountOfHigherLevelMergeablesToCreate; j++)
                    {
                        MergeableItem higherLevelMergeable = ServiceProvider.Instance.MergeableFactory.GetMergableItem(mergeables[0].MergeType, sameLevelMergeables[0].Level + 1);

                        mergeables.Add(higherLevelMergeable);
                    }

                    for (int k = 0; k < amountOfHigherLevelMergeablesToCreate * MIN_REQUIRED_TO_MERGE; k++)
                    {
                        mergeables.Remove(sameLevelMergeables[k]);
                        sameLevelMergeables[k].Merge();
                    }
                }
            }

            for (int l = 0; l < mergeables.Count; l++)
            {
                mergeables[l].TryPlaceInCell(centerCell);
                mergeables[l].Move(centerCell.GetWorldPosition());
            }
        }
    }
}