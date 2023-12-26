using Gameplay.GridSystem;
using Gameplay.PlaceableSystem;
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

            if (cell.TryGetItem(out PlaceableItem placeable))
            {
                if (placeable is MergeableItem mergeable)
                {
                    cell.FindMergeableCells(mergeable.MergeableData.MergeType, _searchedCells, _mergeableCells);
                }
            }

            foreach (var item in _mergeableCells)
            {
                Debug.Log(item.Index);
            }

            return _mergeableCells.Count >= 3;
        }
    }
}