using Gameplay.GridSystem;
using Gameplay.InputSystem;
using Gameplay.PlaceableSystem;
using Gameplay.ServiceSystem;
using UnityEngine;

namespace Gameplay.MovementSystem
{
    public class ObjectMovementManager : MonoBehaviour
    {
        private GridManager _gridManager;

        private InputManager _inputManager;

        private PlaceableItem _draggedItem;

        private Camera _cam;

        private void Awake()
        {
            _gridManager = ServiceProvider.Instance.GridManager;
            _inputManager = ServiceProvider.Instance.InputManager;
            _cam = Camera.main;

            ListenEvents();
        }

        private void ListenEvents()
        {
            _inputManager.Input.OnTapDown += OnTapDown;
            _inputManager.Input.OnTapHold += OnTapHold;
            _inputManager.Input.OnTapUp += OnTapUp;
        }

        private void OnTapDown(Vector3 position)
        {
            GridCell cell = GetCellFromTapPosition(position);

            if (cell != null && cell.TryGetItem(out PlaceableItem item))
            {
                if (item is IMoveable)
                {
                    _draggedItem = item;
                }
            }
        }

        private GridCell GetCellFromTapPosition(Vector3 position)
        {
            Ray ray = _cam.ScreenPointToRay(position);

            Vector3 projectedPos = Vector3.zero;

            if (_gridManager.Plane.Raycast(ray, out float distance))
            {
                projectedPos = ray.GetPoint(distance);
            }
            else
            {
                return null;
            }

            if (_gridManager.TryGetCellFromPosition(projectedPos, out GridCell cell))
            {
                return cell;
            }

            return null;
        }

        private void OnTapHold(Vector3 position)
        {
            if (_draggedItem != null)
            {
                GridCell cell = GetCellFromTapPosition(position);
                Vector3 targetPosition;
                if (cell != null)
                {
                    targetPosition = cell.GetWorldPosition();
                }
                else
                {
                    Ray ray = _cam.ScreenPointToRay(position);

                    Vector3 projectedPos = Vector3.zero;

                    if (_gridManager.Plane.Raycast(ray, out float distance))
                    {
                        projectedPos = ray.GetPoint(distance);
                    }

                    targetPosition = projectedPos + Vector3.up;
                }

                ((IMoveable)_draggedItem).Move(targetPosition);
            }
        }

        private void OnTapUp(Vector3 position)
        {
            if (_draggedItem != null)
            {
                GridCell cell = GetCellFromTapPosition(position);

                if (cell != null && cell.CanAcceptItem())
                {
                    if (!_draggedItem.TryPlaceInCell(cell))
                    {
                        if (_draggedItem.LastKnownPosition != null)
                        {
                            ((IMoveable)_draggedItem).Move(_draggedItem.LastKnownPosition);
                            _draggedItem = null;
                        }
                    }
                }
            }
        }

        private void UnsubscribeToEvents()
        {
            _inputManager.Input.OnTapDown -= OnTapDown;
            _inputManager.Input.OnTapHold -= OnTapHold;
            _inputManager.Input.OnTapUp -= OnTapUp;
        }

        private void OnDestroy()
        {
            UnsubscribeToEvents();
        }
    }
}