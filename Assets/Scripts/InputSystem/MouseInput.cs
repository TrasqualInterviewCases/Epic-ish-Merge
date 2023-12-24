using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.InputSystem
{
    public class MouseInput : InputBase
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnTapDown?.Invoke(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f));
            }
            else if (Input.GetMouseButtonUp(0))
            {
                OnTapUp?.Invoke();
            }
            else if (Input.GetMouseButton(0))
            {
                OnTapHold?.Invoke(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f));
            }

            if(Input.mouseScrollDelta.y != 0)
            {
                OnScroll?.Invoke(Input.mouseScrollDelta.y);
            }
        }

        public override bool IsPointerOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}