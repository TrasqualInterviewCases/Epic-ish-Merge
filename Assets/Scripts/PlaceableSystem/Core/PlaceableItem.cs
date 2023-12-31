using Gameplay.GridSystem;
using Gameplay.ServiceSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.PlaceableSystem
{
    public abstract class PlaceableItem : MonoBehaviour
    {
        public Action<PlaceableItem> OnPlacedInCell;
        public Action<PlaceableItem> OnItemReset;

        public PlaceableItemDataSO Data { get; protected set; }

        public Vector3 LastKnownPosition { get; protected set; }

        protected List<GridCell> OccupiedCells = new();

        private GridCell LastPlacedInCell;
        private List<GridCell> LastOccupiedCells = new();

        protected GridManager _gridManager;

        private void Awake()
        {
            _gridManager = ServiceProvider.Instance.GridManager;
        }

        private void Start()
        {
            LastKnownPosition = transform.position;
        }

        public bool TryPlaceInCell(GridCell cell)
        {
            if (_gridManager.TryPlaceItemWithInput(this, cell, OccupiedCells))
            {
                ReleasePreviousCells();

                LastPlacedInCell = cell;
                LastOccupiedCells = new List<GridCell>(OccupiedCells);

                LastKnownPosition = LastPlacedInCell.GetWorldPosition();

                OnPlacedInCell?.Invoke(this);

                return true;
            }
            else
            {
                return false;
            }
        }

        protected void ReleasePreviousCells()
        {
            if (LastPlacedInCell != null)
            {
                if (LastOccupiedCells != null && LastOccupiedCells.Count > 0)
                {
                    for (int i = 0; i < LastOccupiedCells.Count; i++)
                    {
                        LastOccupiedCells[i].TryRemoveItem(this);
                        _gridManager.ReportCellWasEmptied();
                    }
                }
            }
        }

        public virtual void SetData(PlaceableItemDataSO placeableItemData)
        {
            Data = placeableItemData;
        }

        public virtual void ResetItem()
        {
            OnItemReset?.Invoke(this);
        }
    }
}