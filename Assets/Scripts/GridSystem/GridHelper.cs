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
            const int MAX_LEVEL = 2;
            const int MIN_REQUIERED = 3;

            SearchedCells.Clear();
            MergeableItems.Clear();

            MergeableItem mergeable = cell.GetMergeableItem();

            if (mergeable != null)
            {
                cell.FindMergeableCells(mergeable.MergeType, SearchedCells, MergeableItems);
            }

            if (MergeableItems.Count < MIN_REQUIERED)
            {
                return false;
            }

            for (int i = 0; i < MAX_LEVEL - 1; i++)
            {
                if (MergeableItems.FindAll(x => x.Level == i).Count() >= MIN_REQUIERED)
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