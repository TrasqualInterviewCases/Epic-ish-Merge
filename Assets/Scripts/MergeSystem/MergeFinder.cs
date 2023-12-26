using Gameplay.GridSystem;
using System.Collections.Generic;

namespace Gameplay.MergeableSystem
{
    public static class MergeFinder
    {
        public static List<GridCell> _searchedCells = new List<GridCell>();
        public static List<GridCell> _mergeableCells = new List<GridCell>();

        public static bool CanMerge(GridCell cell)
        {
            _searchedCells.Clear();
            _mergeableCells.Clear();

            _searchedCells.Add(cell);
            _mergeableCells.Add(cell);

            cell.FindMergeableCells(_searchedCells, _mergeableCells);

            return _mergeableCells.Count >= 3;
        }
    }
}