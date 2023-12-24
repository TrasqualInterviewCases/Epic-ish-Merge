using Gameplay.PlaceableSystem;
using Gameplay.ServiceSystem;
using System.Collections.Generic;
using UnityEngine;
using Utilities.Events;

namespace Gameplay.GridSystem
{
    public class GridManager : MonoBehaviour
    {
        public int Width { get; private set; } = 5;
        public int Height { get; private set; } = 5;

        public float CellSize { get; private set; } = 1;

        public Vector3 CenterPosition { get; private set; } = Vector3.zero;

        public Vector3 OriginPosition => new Vector3(CenterPosition.x - Dimensions.x / 2f, 0f, CenterPosition.y - Dimensions.y / 2f);

        public Vector2 Dimensions => new Vector2(Width, Height) * CellSize;

        public GridCell[,] Cells { get; private set; }

        public Plane Plane;

        private Column[] _columns;

        private void Start()
        {
            GridData gridData = ServiceProvider.Instance.LevelManager.GridData;

            Width = gridData.Width;
            Height = gridData.Height;
            CellSize = gridData.CellSize;
            CenterPosition = gridData.Center;
            _columns = gridData.Columns;
            Plane = new Plane(Vector3.up, CenterPosition);

            GenerateGrid();
        }

        private void GenerateGrid()
        {
            Cells = new GridCell[Width, Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Cells[i, j] = new GridCell(i, j, _columns[j].Row[i], this);
                }
            }

            EventManager.Instance.TriggerEvent<GridGeneratedEvent>(new GridGeneratedEvent { GridManager = this });
        }

        public bool TryGetCellFromPosition(Vector3 position, out GridCell cell)
        {
            int x = Mathf.FloorToInt((position.x - OriginPosition.x) / CellSize);
            int y = Mathf.FloorToInt((position.z - OriginPosition.z) / CellSize);

            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                cell = Cells[x, y];
                return true;
            }

            cell = null;
            return false;
        }

        public bool CanCellAtIndexAcceptItem(Vector2Int cellIndex)
        {
            if (!IsIndexWithinGrid(cellIndex))
            {
                return false;
            }

            if (Cells[cellIndex.x, cellIndex.y].CanAcceptItem())
            {
                return true;
            }

            return false;
        }

        public bool IsIndexWithinGrid(Vector2 cellIndex)
        {
            if (cellIndex.x > 0 && cellIndex.x < Width && cellIndex.y > 0 && cellIndex.y < Height)
            {
                return true;
            }

            return false;
        }

        public bool TryPlaceItem(PlaceableItem item, GridCell cell, List<GridCell> occupiedCells)
        {
            occupiedCells.Clear();

            if (item.Data.PlacementMap == null || item.Data.PlacementMap.Count <= 0)
            {
                if (cell.CanAcceptItem())
                {
                    cell.AcceptItem(item);
                    occupiedCells.Add(cell);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            for (int i = 0; i < item.Data.PlacementMap.Count; i++)
            {
                Vector2Int testedCellIndex = cell.Index + item.Data.PlacementMap[i];
                if (!CanCellAtIndexAcceptItem(testedCellIndex))
                {
                    Debug.Log("Cell can NOT accept item");
                    foreach (GridCell occupiedCell in occupiedCells)
                    {
                        occupiedCell.TryRemoveItem(item);
                    }
                    occupiedCells.Clear();
                    return false;
                }
                else
                {
                    occupiedCells.Add(Cells[testedCellIndex.x, testedCellIndex.y]);
                    Debug.Log("Cell can accept item");
                }
            }

            return true;
        }

        public GridCell GetRandomActiveCell()
        {
            int randX = Random.Range(0, Width);
            int randY = Random.Range(0, Height);

            GridCell randomCell = Cells[randX, randY];
            if ((randomCell.State & GridCellState.Active) != 0)
            {
                return randomCell;
            }
            else
            {
                return GetRandomActiveCell();
            }
        }
    }
}