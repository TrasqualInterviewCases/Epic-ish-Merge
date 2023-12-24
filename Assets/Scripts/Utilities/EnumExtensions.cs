using Gameplay.GridSystem;

public static class EnumExtensions
{
    public static GridCellState ConvertToGridCellState(this PlacementType placementType)
    {
        GridCellState cellState = placementType switch
        {
            PlacementType.Filler => GridCellState.HasFillerObject,
            PlacementType.NonFiller => GridCellState.HasNoneFillerObject,
            _ => GridCellState.Active
        };

        return cellState;
    }
}
