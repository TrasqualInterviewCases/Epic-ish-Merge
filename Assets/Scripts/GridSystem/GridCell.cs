using Gameplay.InputSystem;
using Gameplay.MergeableSystem;
using Gameplay.MovementSystem;
using Gameplay.PlaceableSystem;
using Gameplay.ServiceSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay.GridSystem
{
    public class GridCell : IDisposable
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        private GridManager _gridManager;

        public Vector2Int Index => new Vector2Int(X, Y);

        public GridCellState State { get; private set; }

        private List<PlaceableItem> _items = new();

        private Dictionary<Neighbour, GridCell> _neighbours = new();

        public GridCell Up => _neighbours.TryGetValue(Neighbour.Up, out GridCell cell) ? cell : null;
        public GridCell Down => _neighbours.TryGetValue(Neighbour.Down, out GridCell cell) ? cell : null;
        public GridCell Right => _neighbours.TryGetValue(Neighbour.Right, out GridCell cell) ? cell : null;
        public GridCell Left => _neighbours.TryGetValue(Neighbour.Left, out GridCell cell) ? cell : null;

        private InputBase _input;

        public GridCell(int x, int y, GridCellState state, GridManager gridManager)
        {
            X = x;
            Y = y;

            AddState(state);

            _gridManager = gridManager;

            _input = ServiceProvider.Instance.InputManager.Input;

            ListenEvents();
        }

        private void ListenEvents()
        {
            _input.OnTapUp += SelectCell;
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
            return _gridManager.FindNearestEmptyCell(this) != null;
        }

        public bool IsEmpty()
        {
            return (State & GridCellState.Empty) != 0;
        }

        public void AcceptItem(PlaceableItem item)
        {
            if (TryGetItem(out PlaceableItem currentItem))
            {
                GridCell nearestEmptyCell = _gridManager.FindNearestEmptyCell(this);
                if (nearestEmptyCell != null)
                {
                    currentItem.TryPlaceInCell(nearestEmptyCell);
                }
            }

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

            AddState(GridCellState.Active);
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

        public void AddNeighbour(Neighbour neighbourType, GridCell cell)
        {
            _neighbours.Add(neighbourType, cell);
        }

        public GridCell GetNeighbour(Neighbour neighbour)
        {
            return _neighbours[neighbour];
        }

        public void SelectCell(Vector3 tapPosition)
        {
            //if (_gridManager.GetCellFromTapPosition(tapPosition) == this)
            //{
            //    Debug.Log(GetPlacedItem());
            //}
        }

        public GridCell FindNearestEmptyCell(List<GridCell> searchedCells)
        {
            if (IsEmpty())
            {
                return this;
            }

            searchedCells.Add(this);

            foreach (GridCell neighbourCell in _neighbours.Values)
            {
                if (searchedCells.Contains(neighbourCell))
                {
                    continue;
                }
                GridCell emptyCell = neighbourCell.FindNearestEmptyCell(searchedCells);
                if (emptyCell != null)
                {
                    return emptyCell;
                }
            }

            return null;
        }

        public void FindMergeableCells(MergeableType mergeType, List<GridCell> searchedCells, List<GridCell> mergeableCells)
        {
            if (searchedCells.Contains(this))
            {
                return;
            }

            searchedCells.Add(this);

            if (HasSameTypeMergeable(mergeType))
            {
                if (!mergeableCells.Contains(this))
                {
                    mergeableCells.Add(this);
                }

                foreach (GridCell neighbourCell in _neighbours.Values)
                {
                    neighbourCell.FindMergeableCells(mergeType, searchedCells, mergeableCells);
                }
            }
        }

        public bool HasSameTypeMergeable(MergeableType mergeType)
        {
            if (TryGetItem(out PlaceableItem placeableItem))
            {
                if (placeableItem is MergeableItem mergeable)
                {
                    if (mergeable.MergeableData.MergeType == mergeType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void UnsubscribeToEvents()
        {
            _input.OnTapUp -= SelectCell;
        }

        public void Dispose()
        {
            UnsubscribeToEvents();
        }
    }
}