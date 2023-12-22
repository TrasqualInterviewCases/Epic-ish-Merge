using TMPro;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private TMP_Text _textPrefab;

    public int Width { get; private set; } = 7;
    public int Height { get; private set; } = 7;

    public float CellSize { get; private set; } = 3;

    public Vector3 CenterPosition { get; private set; } = Vector3.zero;

    public Vector3 OriginPosition => new Vector3(CenterPosition.x - Dimensions.x / 2f, 0f, CenterPosition.y - Dimensions.y / 2f);

    public Vector2 Dimensions => new Vector2(Width, Height) * CellSize;

    private GridCell[,] _cells;

    private void Awake()
    {
        _cells = new GridCell[Width, Height];

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                _cells[i, j] = new GridCell(i, j, this);
            }
        }

        WriteGridText();
    }

    public GridCell GetCellFromPosition(Vector3 position)
    {
        int x = Mathf.FloorToInt((position.x - OriginPosition.x) / CellSize);
        int y = Mathf.FloorToInt((position.z - OriginPosition.z) / CellSize);
        Debug.Log(x + " , " + y);
        if (x >= 0 && y >= 0 && x < Width && y < Height)
        {
            return _cells[x, y];
        }
        return null;
    }

    private void WriteGridText()
    {
        foreach (GridCell cell in _cells)
        {
            var text = Instantiate(_textPrefab, cell.GetWorldPosition(), _textPrefab.transform.rotation);
            text.SetText(cell.Index.ToString());
        }
    }
}