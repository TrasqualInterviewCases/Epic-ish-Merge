using Gameplay.GridSystem;
using Gameplay.PlaceableSystem;

public static class EnumExtensions
{
    public static GridCellState ConvertToGridCellState(this PlacementFillType placementType)
    {
        GridCellState cellState = placementType switch
        {
            PlacementFillType.Filler => GridCellState.HasFillerObject,
            PlacementFillType.NonFiller => GridCellState.HasNoneFillerObject,
            _ => GridCellState.Active
        };

        return cellState;
    }
}
