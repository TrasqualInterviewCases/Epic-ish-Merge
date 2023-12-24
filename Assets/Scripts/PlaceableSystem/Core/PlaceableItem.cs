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

        public Vector3 LastKnownPosition { get; protected set; }

        private GridCell LastPlacedInCell;

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
            if (_gridManager.TryPlaceItem(this, cell, OccupiedCells))
            {
                if (LastPlacedInCell != null)
                {
                    LastPlacedInCell.TryRemoveItem(this);

                    if (OccupiedCells != null && OccupiedCells.Count > 0)
                    {
                        for (int i = 0; i < OccupiedCells.Count; i++)
                        {
                            OccupiedCells[i].TryRemoveItem(this);
                        }
                    }
                }

                LastPlacedInCell = cell;

                LastKnownPosition = LastPlacedInCell.GetWorldPosition();

                OnPlacementSuccess?.Invoke(this);
                return true;
            }
            else
            {
                OnPlacementFail?.Invoke(this);
                return false;
            }
        }

        public virtual void SetData(PlaceableItemDataSO placeableItemData)
        {
            Data = placeableItemData;
        }
    }
}