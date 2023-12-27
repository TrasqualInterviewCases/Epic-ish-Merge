using Gameplay.MergeableSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MergeableFactory : MonoBehaviour
{
    [SerializeField] private MergeableItem _mergeableItemPrefab;

    [SerializeField] private MergeableDataSO[] _mergeableDatas;

    private MergeableDataSO GetMergeableData(MergeableType type, int level)
    {
        return _mergeableDatas.FirstOrDefault(x => x.MergeType == type && x.Level == level);
    }

    public MergeableItem GetMergableItem(MergeableType type, int level)
    {
        MergeableDataSO mergeableData = GetMergeableData(type, level);

        if (mergeableData == null)
        {
            return null;
        }

        return SpawnMergeable(mergeableData);
    }

    public MergeableItem GetRandomMergeable(int level)
    {
        MergeableDataSO[] datas = Array.FindAll(_mergeableDatas, x => x.Level == level);

        if (datas.Length <= 0)
        {
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, datas.Length);
        MergeableDataSO mergeableData = _mergeableDatas[randomIndex];

        return SpawnMergeable(mergeableData);
    }

    private MergeableItem SpawnMergeable(MergeableDataSO data)
    {
        MergeableItem mergeable = Instantiate(_mergeableItemPrefab);
        mergeable.SetData(data);
        mergeable.Init();

        return mergeable;
    }
}
