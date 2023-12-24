using Gameplay.MovementSystem;
using Gameplay.PlaceableSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.GridSystem
{
    public class GridCell
    {
        public Action<GridCell> OnCellSelected;

        public int X { get; private set; }
        public int Y { get; private set; }

        private GridManager _gridManager;

        public Vector2Int Index => new Vector2Int(X, Y);

        public GridCellState State { get; private set; }

        private List<PlaceableItem> _items = new();

        public GridCell(int x, int y, GridCellState state, GridManager gridManager)
        {
            X = x;
            Y = y;

            AddState(state);

            _gridManager = gridManager;
        }

        public Vector3 GetWorldPosition()
        {
            float gridWidth = _gridManager.Dimensions.x;
            float gridHeight = _gridManager.Dimensions.y;

            float xPosition = _gridManager.CenterPosition.x + X * _gridManager.CellSize + _gridManager.CellSize / 2f - gridWidth / 2f;
            float zPosition = _gridManager.CenterPosition.y + Y * _gridManager.CellSize + _gridManager.CellSize / 2f - gridHeight / 2f;

            return new Vector3(xPosition, 0f, zPosition);
        }

        public List<Vector3> GetWorldCorners()
        {
            float halfSize = _gridManager.CellSize / 2f;

            List<Vector3> corners = new()
            {
            GetWorldPosition() + new Vector3(-halfSize, 0f, -halfSize),
            GetWorldPosition() + new Vector3(halfSize, 0f, -halfSize),
            GetWorldPosition() + new Vector3(-halfSize, 0f, halfSize),
            GetWorldPosition() + new Vector3(halfSize, 0f, halfSize),
            };

            return corners;
        }

        public void AddState(GridCellState state)
        {
            if ((State & state) == state)
            {
                return;
            }

            State |= state;
        }

        public void RemoveState(GridCellState state)
        {
            if ((State & state) != state)
            {
                return;
            }

            State &= ~state;
        }

        private bool HasMoveableItem()
        {
            return _items.Any(x => x is IMoveable);
        }

        public bool CanAcceptItem()
        {
            return (State & GridCellState.Empty) != 0;
        }

        public void AcceptItem(PlaceableItem item)
        {
            _items.Add(item);

            RemoveState(GridCellState.Active);
            AddState(item.Data.PlacementType.ConvertToGridCellState());
        }

        public void TryRemoveItem(PlaceableItem item)
        {
            if (!_items.Contains(item))
            {
                return;
            }

            _items.Remove(item);

            RemoveState(item.Data.PlacementType.ConvertToGridCellState());
        }

        public bool TryGetItem(out PlaceableItem item)
        {
            if (HasMoveableItem())
            {
                item = GetPlacedItem();
                return true;
            }

            item = null;
            return false;
        }

        public PlaceableItem GetPlacedItem()
        {
            return _items.FirstOrDefault(x => x is IMoveable);
        }

        public void SelectCell()
        {
            OnCellSelected?.Invoke(this);
        }
    }
}