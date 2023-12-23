using System;
using UnityEngine;

namespace Gameplay.InputSystem
{
    public abstract class InputBase : MonoBehaviour
    {
        public Action<Vector3> OnTapDown;
        public Action OnTapUp;
        public Action<Vector3> OnTapHold;
        public abstract bool IsPointerOverUI();
    }
}