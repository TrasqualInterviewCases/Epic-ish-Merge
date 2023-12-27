using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AddressablePool : MonoBehaviour
{
    [SerializeField] private int _poolSize = 10;
    [SerializeField] private AssetReference _assetReference;

    private Stack<GameObject> _pooledItems = new();

    public async UniTask InitializePool(AssetReference reference)
    {
        _assetReference = reference;

        for (int i = 0; i < _poolSize; i++)
        {
            await AddItemToPool();
        }
    }

    private async UniTask AddItemToPool()
    {
        var handle = await _assetReference.InstantiateAsync(transform);
        handle.SetActive(false);

        _pooledItems.Push(handle);
    }

    public async UniTask<GameObject> GetItem()
    {
        if (_pooledItems.Count <= 0)
        {
            await AddItemToPool();
        }
        GameObject item = _pooledItems.Pop();

        item.SetActive(true);

        return item;
    }

    public void ReturnItem(GameObject item)
    {
        item.SetActive(false);
        item.transform.SetParent(transform);
        _pooledItems.Push(item);
    }

    private void OnDestroy()
    {
        foreach (var item in _pooledItems)
        {
            Addressables.ReleaseInstance(item);
        }

        _pooledItems.Clear();
    }
}
