using Gameplay.MergeableSystem;
using System.Linq;
using UnityEngine;

public class MergeableFactory : MonoBehaviour
{
    [SerializeField] private MergeableItem _mergeableItemPrefab;

    [SerializeField] private MergeableDataSO[] _mergeableDatas;

    public MergeableItem GetMergableItem(MergeableType type, int level)
    {
        MergeableDataSO mergeableData = GetMergeableData(type, level);

        if (mergeableData == null)
        {
            return null;
        }

        MergeableItem mergeable = Instantiate(_mergeableItemPrefab);
        mergeable.SetData(mergeableData);

        return mergeable;
    }

    private MergeableDataSO GetMergeableData(MergeableType type, int level)
    {
        return _mergeableDatas.FirstOrDefault(x => x.MergeType == type && x.Level == level);
    }
}
