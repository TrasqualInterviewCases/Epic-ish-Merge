using Gameplay.GridSystem;
using Gameplay.InputSystem;
using Gameplay.MergeableSystem;
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
                    _draggedItem.OnItemReset += OnDraggedItemReset;
                }
            }
        }

        private void OnTapHold(Vector3 position)
        {
            if (_draggedItem != null)
            {
                GridCell cell = _gridManager.GetCellFromTapPosition(position);
                Vector3 targetPosition;
                if (cell != null && !cell.IsInActive())
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

                if (cell != null && !cell.IsInActive())
                {
                    if (!_draggedItem.TryPlaceInCell(cell))
                    {
                        ServiceProvider.Instance.TextPopUpManager.GetTextPopUp("Can't place there.", _draggedItem.transform.position);
                        ((IMoveable)_draggedItem).Move(_draggedItem.LastKnownPosition);
                    }
                    else
                    {
                        if (GridHelper.CanMerge(cell))
                        {
                            MergeHandler.Merge(cell);
                        }
                    }
                    _draggedItem = null;
                }
                else
                {
                    ServiceProvider.Instance.TextPopUpManager.GetTextPopUp("Can't place there.", _draggedItem.transform.position);

                    ((IMoveable)_draggedItem).Move(_draggedItem.LastKnownPosition);
                    _draggedItem = null;
                }
            }
        }

        private void OnDraggedItemReset(PlaceableItem item)
        {
            _draggedItem.OnItemReset -= OnDraggedItemReset;
            _draggedItem = null;
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