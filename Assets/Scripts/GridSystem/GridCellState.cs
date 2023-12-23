namespace Gameplay.GridSystem
{
    public enum GridCellState
    {
        None = 0,
        InActive = 1,
        Active = 2,
        HasNoneFillerObject = 4,
        HasFillerObject = 8,

        Empty = Active | HasNoneFillerObject,

        Full = InActive | HasFillerObject,

        HasObject = HasNoneFillerObject | HasFillerObject,
    }
}