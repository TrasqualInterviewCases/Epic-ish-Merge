using Gameplay.GridSystem;
using Gameplay.MergeableSystem;

namespace Gameplay.MovementSystem
{
    public interface IShoveable
    {
        public void Initialize(MergeableItem mergeable);
        public void Shove(GridCell cell);
    }
}