using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressablePoolManager : MonoBehaviour
{
    private Dictionary<AssetReference, AddressablePool> _pools = new();

    public async UniTask GeneratePool(AssetReference reference)
    {
        AddressablePool newPool = gameObject.AddComponent<AddressablePool>();

        await newPool.InitializePool(reference);

        _pools.Add(reference, newPool);
    }

    public AddressablePool GetPool(AssetReference reference)
    {
        _pools.TryGetValue(reference, out AddressablePool pool);

        return pool;
    }
}
