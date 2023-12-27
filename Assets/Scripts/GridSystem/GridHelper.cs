using Gameplay.GridSystem;
using Gameplay.PlaceableSystem;
using System.Collections.Generic;

namespace Gameplay.MergeableSystem
{
    public static class GridHelper
    {
        public static List<GridCell> _searchedCells = new List<GridCell>();
        public static List<GridCell> _mergeableCells = new List<GridCell>();

        public static bool CanMerge(GridCell cell)
        {
            _searchedCells.Clear();
            _mergeableCells.Clear();

            if (cell.TryGetItem(out PlaceableItem placeable))
            {
                if (placeable is MergeableItem mergeable)
                {
                    cell.FindMergeableCells(mergeable.MergeableData.MergeType, _searchedCells, _mergeableCells);
                }
            }

            return _mergeableCells.Count >= 3;
        }


        public static GridCell FindNearestEmptyCell(GridCell cell)
        {
            _searchedCells.Clear();

            return cell.FindNearestEmptyCell(_searchedCells);
        }
    }
}