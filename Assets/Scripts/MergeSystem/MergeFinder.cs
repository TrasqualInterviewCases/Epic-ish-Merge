using Gameplay.GridSystem;
using System.Collections.Generic;
using UnityEngine;

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

            foreach (var item in _mergeableCells)
            {
                Debug.Log(item.Index);
            }

            return _mergeableCells.Count >= 3;
        }
    }
}