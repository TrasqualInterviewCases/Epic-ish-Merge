using Gameplay.GridSystem;
using Gameplay.ServiceSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.PlaceableSystem
{
    public abstract class PlaceableItem : MonoBehaviour
    {
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

                LastPlacedInCell = cell;
                LastOccupiedCells = new List<GridCell>(OccupiedCells);

                LastKnownPosition = LastPlacedInCell.GetWorldPosition();

                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual void SetData(PlaceableItemDataSO placeableItemData)
        {
            Data = placeableItemData;
        }
    }
}