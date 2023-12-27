using Gameplay.GridSystem;
using Gameplay.ServiceSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.MergeableSystem
{
    public static class MergeHandler
    {
        public static void Merge(GridCell centerCell)
        {
            const int ITEM_MAX_LEVEL = 2;
            const int MIN_REQUIRED_TO_MERGE = 3;

            List<MergeableItem> mergeables = GridHelper.MergeableItems.OrderBy(x => x.Level).ToList();

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

                    for (int k = 0; k < sameLevelMergeables.Count; k++)
                    {
                        sameLevelMergeables[k].Merge();
                    }

                    mergeables.RemoveRange(0, amountOfHigherLevelMergeablesToCreate * MIN_REQUIRED_TO_MERGE);
                }
            }

            Debug.Log(mergeables.Count);
            for (int l = 0; l < mergeables.Count; l++)
            {
                mergeables[l].TryPlaceInCell(centerCell);
                mergeables[l].Move(centerCell.GetWorldPosition());
            }
        }
    }
}