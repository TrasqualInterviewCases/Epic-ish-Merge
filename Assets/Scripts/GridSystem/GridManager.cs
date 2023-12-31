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

        private Camera _cam;

        private int _emptyCellCount;

        private void Start()
        {
            _cam = Camera.main;

            GridData gridData = ServiceProvider.Instance.LevelManager.GridData;

            Width = gridData.Width;
            Height = gridData.Height;
            CellSize = gridData.CellSize;
            CenterPosition = gridData.Center;
            _columns = gridData.Columns;
            Plane = new Plane(Vector3.up, CenterPosition);
            _emptyCellCount = Width * Height;

            GenerateGrid();
        }

        private void GenerateGrid()
        {
            Cells = new GridCell[Width, Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    if (_columns[j].Row[i] == GridCellState.InActive)
                    {
                        _emptyCellCount--;
                    }

                    Cells[i, j] = new GridCell(i, j, _columns[j].Row[i], this);

                    if (i != 0)
                    {
                        Cells[i, j].AddNeighbour(Neighbour.Left, Cells[i - 1, j]);
                        Cells[i - 1, j].AddNeighbour(Neighbour.Right, Cells[i, j]);
                    }
                    if (j != 0)
                    {
                        Cells[i, j].AddNeighbour(Neighbour.Down, Cells[i, j - 1]);
                        Cells[i, j - 1].AddNeighbour(Neighbour.Up, Cells[i, j]);
                    }
                }
            }

            EventManager.Instance.TriggerEvent<GridGeneratedEvent>(new GridGeneratedEvent { GridManager = this });
        }

        private bool TryGetCellFromPosition(Vector3 position, out GridCell cell)
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




            return false;
        }

        public bool IsIndexWithinGrid(Vector2 cellIndex)
        {
            if (cellIndex.x >= 0 && cellIndex.x < Width && cellIndex.y >= 0 && cellIndex.y < Height)
            {
                return true;
            }

            return false;
        }

        public bool TryPlaceItemWithInput(PlaceableItem item, GridCell cell, List<GridCell> occupiedCells)
        {
            occupiedCells.Clear();

            if (CanAllCellsInPlacementMapAcceptItem(item, cell))
            {
                for (int i = 0; i < item.Data.PlacementMap.Count; i++)
                {
                    Vector2Int currentCellIndex = cell.Index + item.Data.PlacementMap[i];
                    GridCell currentCell = Cells[currentCellIndex.x, currentCellIndex.y];

                    occupiedCells.Add(currentCell);
                    currentCell.AcceptItem(item);
                    _emptyCellCount--;
                }

                return true;
            }

            return false;
        }

        private bool CanAllCellsInPlacementMapAcceptItem(PlaceableItem item, GridCell cell)
        {
            int cellsWithShovableItem = 0;

            for (int i = 0; i < item.Data.PlacementMap.Count; i++)
            {
                Vector2Int testedCellIndex = cell.Index + item.Data.PlacementMap[i];

                if (!IsIndexWithinGrid(testedCellIndex))
                {
                    return false;
                }

                GridCell testedCell = Cells[testedCellIndex.x, testedCellIndex.y];

                if (testedCell.TryGetItem(out PlaceableItem currentItem))
                {
                    if (currentItem.gameObject == item.gameObject)
                    {
                        continue;
                    }
                }

                if (!testedCell.CanAcceptItem())
                {
                    return false;
                }


                if (testedCell.IsEmpty())
                {
                    continue;
                }

                if (testedCell.HasShovableItem())
                {
                    cellsWithShovableItem++;
                }
            }

            if (cellsWithShovableItem <= _emptyCellCount)
            {
                return true;
            }

            return false;
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

        public GridCell GetRandomAvailableCell()
        {
            int randX = Random.Range(0, Width);
            int randY = Random.Range(0, Height);

            GridCell randomCell = Cells[randX, randY];
            if (randomCell.CanAcceptItem())
            {
                return randomCell;
            }
            else
            {
                return GetRandomAvailableCell();
            }
        }

        public GridCell GetCellFromTapPosition(Vector3 position)
        {
            Ray ray = _cam.ScreenPointToRay(position);

            Vector3 projectedPos = Vector3.zero;

            if (Plane.Raycast(ray, out float distance))
            {
                projectedPos = ray.GetPoint(distance);
            }
            else
            {
                return null;
            }

            if (TryGetCellFromPosition(projectedPos, out GridCell cell))
            {
                return cell;
            }

            return null;
        }


        public Vector3 GetPointerGridPlanePosition(Vector3 pointerPosition)
        {
            Ray ray = _cam.ScreenPointToRay(pointerPosition);

            Vector3 projectedPos = Vector3.zero;

            if (Plane.Raycast(ray, out float distance))
            {
                projectedPos = ray.GetPoint(distance);
            }

            return projectedPos + Vector3.up;
        }

        public void ReportCellWasEmptied()
        {
            _emptyCellCount++;
        }
    }
}