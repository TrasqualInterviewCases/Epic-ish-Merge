using Cysharp.Threading.Tasks;
using Gameplay.MergeableSystem;
using Gameplay.ServiceSystem;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class MergeableFactory : MonoBehaviour
{
    [SerializeField] private AssetReference _mergeableItemPrefab;

    [SerializeField] private MergeableDataSO[] _mergeableDatas;

    private AddressablePoolManager _poolManager;

    public bool AllPoolsAreReady { get; private set; }

    private async void Awake()
    {
        _poolManager = ServiceProvider.Instance.AddressablePoolManager;

        await _poolManager.GeneratePool(_mergeableItemPrefab);

        for (int i = 0; i < _mergeableDatas.Length; i++)
        {
            await _poolManager.GeneratePool(_mergeableDatas[i].ItemPrefab);
        }

        AllPoolsAreReady = true;
    }

    private MergeableDataSO GetMergeableData(MergeableType type, int level)
    {
        return _mergeableDatas.FirstOrDefault(x => x.MergeType == type && x.Level == level);
    }

    public async UniTask<MergeableItem> GetMergableItem(MergeableType type, int level)
    {
        MergeableDataSO mergeableData = GetMergeableData(type, level);

        if (mergeableData == null)
        {
            return null;
        }

        return await SpawnMergeable(mergeableData);
    }

    public async UniTask<MergeableItem> GetRandomMergeable(int level)
    {
        MergeableDataSO[] datas = Array.FindAll(_mergeableDatas, x => x.Level == level);

        if (datas.Length <= 0)
        {
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, datas.Length);
        MergeableDataSO mergeableData = _mergeableDatas[randomIndex];

        return await SpawnMergeable(mergeableData);
    }

    private async UniTask<MergeableItem> SpawnMergeable(MergeableDataSO data)
    {
        AddressablePool mergeablePool = _poolManager.GetPool(_mergeableItemPrefab);
        GameObject mergeableObject = await mergeablePool.GetItem();
        MergeableItem mergeable = mergeableObject.GetComponent<MergeableItem>();
        mergeable.SetData(data);
        mergeable.Init(mergeablePool, _poolManager.GetPool(data.ItemPrefab));

        return mergeable;
    }
}
