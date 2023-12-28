using Cysharp.Threading.Tasks;
using Gameplay.GridSystem;
using Gameplay.MergeableSystem;
using UnityEngine;

namespace Gameplay.MovementSystem
{
    public class ShoveableItem : MonoBehaviour, IShoveable
    {
        private MergeableItem _mergeable;

        public void Initialize(MergeableItem mergeable)
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
    }
}