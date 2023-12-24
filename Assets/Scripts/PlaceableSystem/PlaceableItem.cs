using Gameplay.GridSystem;
using Gameplay.ServiceSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.PlaceableSystem
{
    public abstract class PlaceableItem : MonoBehaviour
    {
        public Action<PlaceableItem> OnPlacementSuccess;
        public Action<PlaceableItem> OnPlacementFail;

        public PlaceableItemDataSO Data { get; protected set; }
        public List<GridCell> OccupiedCells { get; protected set; } = new();

        protected GridCell _lastPlacedInCell;

        protected GridManager _gridManager;

        private void Awake()
        {
            _gridManager = ServiceProvider.Instance.GridManager;
        }

        public void TryPlaceInCell(GridCell cell)
        {
            if (_gridManager.TryPlaceItem(this, cell, OccupiedCells))
            {
                if (_lastPlacedInCell != null)
                {
                    _lastPlacedInCell.TryRemoveItem(this);

                    if (OccupiedCells != null && OccupiedCells.Count > 0)
                    {
                        for (int i = 0; i < OccupiedCells.Count; i++)
                        {
                            OccupiedCells[i].TryRemoveItem(this);
                        }
                    }
                }

                _lastPlacedInCell = cell;

                OnPlacementSuccess?.Invoke(this);
            }
            else
            {
                OnPlacementFail?.Invoke(this);
            }
        }
    }
}