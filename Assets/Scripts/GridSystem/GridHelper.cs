using Gameplay.GameData;
using Gameplay.GridSystem;
using System.Collections.Generic;
using System.Linq;

namespace Gameplay.MergeableSystem
{
    public static class GridHelper
    {
        public static List<GridCell> SearchedCells = new List<GridCell>();
        public static List<MergeableItem> MergeableItems = new List<MergeableItem>();

        public static bool CanMerge(GridCell cell)
        {
            const int MAX_ITEM_LEVEL = StaticGameData.MAX_ITEM_LEVEL;
            const int MIN_ITEM_TO_MERGE = StaticGameData.MIN_ITEM_TO_MERGE;

            SearchedCells.Clear();
            MergeableItems.Clear();

            MergeableItem mergeable = cell.GetMergeableItem();

            if (mergeable != null)
            {
                cell.FindMergeableCells(mergeable.MergeType, SearchedCells, MergeableItems);
            }

            if (MergeableItems.Count < MIN_ITEM_TO_MERGE)
            {
                return false;
            }

            for (int i = 0; i < MAX_ITEM_LEVEL; i++)
            {
                if (MergeableItems.FindAll(x => x.Level == i).Count() >= MIN_ITEM_TO_MERGE)
                {
                    return true;
                }
            }


            return false;
        }


        public static GridCell FindNearestEmptyCell(GridCell cell)
        {
            SearchedCells.Clear();

            return cell.FindNearestEmptyCell(SearchedCells);
        }
    }
}