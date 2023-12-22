using UnityEngine;

namespace Scripts.InputSystem
{
    public class MouseInput : InputBase
    {
        [SerializeField] private GridManager _gridManager;
        [SerializeField] private Transform vCam;

        private Plane _groundPlane;
        private Camera _cam;
        private float _scrollDelta = 50f;

        private Vector3 _projectedPos;

        private void Awake()
        {
            _groundPlane = new Plane(Vector3.up, _gridManager.CenterPosition);
            _cam = Camera.main;
        }

        private void Update()
        {
            MouseLeftButtonDown();
            MouseLeftButtonDrag();
            MouseLeftButtonUp();
            MouseMiddleButtonDrag();
            MouseRightDrag();
            MouseScroll();
        }

        private void MouseLeftButtonDrag()
        {
            if (Input.GetMouseButton(0))
            {
                var mousePos = Input.mousePosition;
                var mouseRay = _cam.ScreenPointToRay(mousePos);

                if (_groundPlane.Raycast(mouseRay, out float distance))
                {
                    _projectedPos = mouseRay.GetPoint(distance);
                }

                var curCell = _gridManager.GetCellFromPosition(_projectedPos);

                OnMouseDrag?.Invoke(curCell);
            }
        }

        private void MouseLeftButtonDown()
        {
            if (Input.GetMouseButtonDown(0))
            {
                var mousePos = Input.mousePosition;
                var mouseRay = _cam.ScreenPointToRay(mousePos);

                if (_groundPlane.Raycast(mouseRay, out float distance))
                {
                    _projectedPos = mouseRay.GetPoint(distance);
                }

                var curCell = _gridManager.GetCellFromPosition(_projectedPos);

                if (curCell != null)
                {
                    //if (curCell.Placeable != null)
                    //{
                    //    curCell.Placeable.SubscribeToInput();
                    //}
                    Debug.Log(curCell.Index + " : " + _projectedPos);
                }
            }
        }

        private void MouseMiddleButtonDrag()
        {
            if (Input.GetMouseButton(2))
            {
                OnRotateDrag?.Invoke(Input.GetAxis("Mouse X"));
            }
        }

        private void MouseRightDrag()
        {
            if (Input.GetMouseButton(1))
            {
                var direction = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                OnMouseRightDrag?.Invoke(direction);
            }
        }

        private void MouseLeftButtonUp()
        {
            if (Input.GetMouseButtonUp(0))
            {
                var mousePos = Input.mousePosition;
                var mouseRay = _cam.ScreenPointToRay(mousePos);

                if (_groundPlane.Raycast(mouseRay, out float distance))
                {
                    _projectedPos = mouseRay.GetPoint(distance);
                }

                var curCell = _gridManager.GetCellFromPosition(_projectedPos);

                OnMouseUp?.Invoke(curCell);
            }
        }

        private void MouseScroll()
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                _scrollDelta--;
                OnMouseScroll?.Invoke(_scrollDelta);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                _scrollDelta++;
                OnMouseScroll?.Invoke(_scrollDelta);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.DrawSphere(_projectedPos, 0.25f);
        }
    }
#endif
}
