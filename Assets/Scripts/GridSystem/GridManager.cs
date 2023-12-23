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

        private void Awake()
        {
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            Cells = new GridCell[Width, Height];

            for (int i = 0; i < Width; i++)
            {
                for (int j = 0; j < Height; j++)
                {
                    Cells[i, j] = new GridCell(i, j, this);
                }
            }

            EventManager.Instance.TriggerEvent<GridGeneratedEvent>(new GridGeneratedEvent { GridManager = this });
        }

        public GridCell GetCellFromPosition(Vector3 position)
        {
            int x = Mathf.FloorToInt((position.x - OriginPosition.x) / CellSize);
            int y = Mathf.FloorToInt((position.z - OriginPosition.z) / CellSize);

            if (x >= 0 && y >= 0 && x < Width && y < Height)
            {
                return Cells[x, y];
            }
            return null;
        }
    }
}