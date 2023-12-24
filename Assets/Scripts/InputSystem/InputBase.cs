using System;
using UnityEngine;

namespace Gameplay.InputSystem
{
    public abstract class InputBase : MonoBehaviour
    {
        public Action<Vector3> OnTapDown;
        public Action<Vector3> OnTapUp;
        public Action<Vector3> OnTapHold;
        public Action<float> OnScroll;
        public abstract bool IsPointerOverUI();
    }
}