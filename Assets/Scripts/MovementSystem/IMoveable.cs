using UnityEngine;

namespace Gameplay.MovementSystem
{
    public interface IMoveable
    {
        public void Move(Vector3 direction);
    }
}