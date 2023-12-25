using Gameplay.GridSystem;
using Gameplay.InputSystem;
using Gameplay.PlaceableSystem;
using Gameplay.ServiceSystem;
using UnityEngine;

namespace Gameplay.MovementSystem
{
    public class ItemMovementManager : MonoBehaviour
    {
        private GridManager _gridManager;

        private InputManager _inputManager;

        private PlaceableItem _draggedItem;

        private void Awake()
        {
            _gridManager = ServiceProvider.Instance.GridManager;
            _inputManager = ServiceProvider.Instance.InputManager;

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
            GridCell cell = _gridManager.GetCellFromTapPosition(position);

            if (cell != null && cell.TryGetItem(out PlaceableItem item))
            {
                if (item is IMoveable)
                {
                    _draggedItem = item;
                }
            }
        }

        private void OnTapHold(Vector3 position)
        {
            if (_draggedItem != null)
            {
                GridCell cell = _gridManager.GetCellFromTapPosition(position);
                Vector3 targetPosition;
                if (cell != null && (cell.State & GridCellState.InActive) != GridCellState.InActive)
                {
                    targetPosition = cell.GetWorldPosition();
                }
                else
                {
                    targetPosition = _gridManager.GetPointerGridPlanePosition(position);
                }

                ((IMoveable)_draggedItem).Move(targetPosition);
            }
        }

        private void OnTapUp(Vector3 position)
        {
            if (_draggedItem != null)
            {
                GridCell cell = _gridManager.GetCellFromTapPosition(position);

                if (cell != null && cell.CanAcceptItem())
                {
                    if (!_draggedItem.TryPlaceInCell(cell))
                    {
                        ((IMoveable)_draggedItem).Move(_draggedItem.LastKnownPosition);
                    }
                    _draggedItem = null;
                }
                else
                {
                    ((IMoveable)_draggedItem).Move(_draggedItem.LastKnownPosition);
                    _draggedItem = null;
                }
            }
        }

        public bool IsPointerOverPlaceable(Vector3 position)
        {
            GridCell cell = _gridManager.GetCellFromTapPosition(position);

            if (cell == null && cell.TryGetItem(out PlaceableItem placeableItem))
            {
                return true;
            }

            return false;
        }

        public bool IsMovingItem()
        {
            return _draggedItem != null;
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