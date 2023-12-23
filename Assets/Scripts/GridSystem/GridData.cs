using UnityEngine;

namespace Gameplay.GridSystem
{
    [System.Serializable]
    public class GridData
    {
        public int Width = 5;
        public int Height = 5;

        public float CellSize = 1f;

        public Vector3 Center = Vector3.zero;

        public Column[] Columns;
    }

    [System.Serializable]
    public struct Column
    {
        public GridCellState[] Row;
    } 
}