using Cysharp.Threading.Tasks;
using Gameplay.GridSystem;
using Gameplay.MergeableSystem;
using System;

namespace Gameplay.MovementSystem
{
    public class ShoveableItem : IShoveable, IDisposable
    {
        private MergeableItem _mergeable;

        public ShoveableItem(MergeableItem mergeable)
        {
            _mergeable = mergeable;
        }

        public void Shove(GridCell cell)
        {
            GridCell nearestEmptyCell = GridHelper.FindNearestEmptyCell(cell);
            if (nearestEmptyCell != null)
            {
                _mergeable.TryPlaceInCell(nearestEmptyCell);
                _mergeable.MoveWithAnimation(nearestEmptyCell.GetWorldPosition()).Forget();
            }
        }

        public void Dispose()
        {
            _mergeable = null;
        }
    }
}