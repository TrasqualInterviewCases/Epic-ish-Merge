namespace Gameplay.MergeableSystem
{
    public interface IMergeable
    {
        public MergeableDataSO MergeableData { get; set; }

        public void Merge();
    }
}