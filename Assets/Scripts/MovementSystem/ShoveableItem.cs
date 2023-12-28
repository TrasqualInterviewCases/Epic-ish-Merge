using Cysharp.Threading.Tasks;
using Gameplay.GridSystem;
using Gameplay.MergeableSystem;
using Gameplay.ServiceSystem;
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
                if (_mergeable.TryPlaceInCell(nearestEmptyCell))
                {
                    _mergeable.MoveWithAnimation(nearestEmptyCell.GetWorldPosition()).Forget();
                }
                else
                {
                    PlaceMergeable();
                }
            }
        }

        private void PlaceMergeable()
        {
            var availableCell = ServiceProvider.Instance.GridManager.GetRandomAvailableCell();

            if (_mergeable.TryPlaceInCell(availableCell))
            {
                _mergeable.MoveWithAnimation(availableCell.GetWorldPosition()).Forget();
            }
            else
            {
                PlaceMergeable();
            }
        }

        public void Dispose()
        {
            _mergeable = null;
        }
    }
}