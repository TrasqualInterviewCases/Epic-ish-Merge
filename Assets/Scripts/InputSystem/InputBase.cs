using System;
using UnityEngine;

namespace Scripts.InputSystem
{
    public class InputBase : MonoBehaviour
    {
        public static Action<Transform> OnMouseDown;
        public static Action<float> OnRotateDrag;
        public static Action<Vector2> OnMouseRightDrag;
        public static Action<float> OnMouseScroll;
        public static Action<GridCell> OnMouseDrag;
        public static Action<GridCell> OnMouseUp;
    }
}
