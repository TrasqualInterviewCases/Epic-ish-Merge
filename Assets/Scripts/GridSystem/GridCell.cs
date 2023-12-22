using UnityEngine;

public class GridCell
{
    public int X { get; private set; }
    public int Y { get; private set; }

    private GridManager _gridManager;

    public Vector2Int Index => new Vector2Int(X, Y);

    public GridCell(int x, int y, GridManager gridManager)
    {
        X = x;
        Y = y;

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
}
