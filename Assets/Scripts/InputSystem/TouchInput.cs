using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.InputSystem
{
    public class TouchInput : InputBase
    {
        private void Update()
        {
            if (Input.touchCount > 0)
            {
                Touch firstTouch = Input.GetTouch(0);
                if (firstTouch.phase == TouchPhase.Began)
                {
                    OnTapDown?.Invoke(new Vector3(firstTouch.position.x, firstTouch.position.y, 1f));
                }
                else if (firstTouch.phase == TouchPhase.Moved)
                {
                    OnTapHold?.Invoke(new Vector3(firstTouch.position.x, firstTouch.position.y, 1f));
                }
                else if (firstTouch.phase == TouchPhase.Ended)
                {
                    OnTapUp?.Invoke();
                }
            }
        }

        public override bool IsPointerOverUI()
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; i++)
                {
                    if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(i).fingerId))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}