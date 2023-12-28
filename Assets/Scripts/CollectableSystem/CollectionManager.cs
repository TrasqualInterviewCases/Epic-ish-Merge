using Gameplay.CollectableSystem;
using Gameplay.ServiceSystem;
using Gameplay.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CollectionManager : MonoBehaviour
{
    [SerializeField] private AssetReference _progressBarPrefab;

    [SerializeField] private Transform _canvasTransform;

    private AddressablePoolManager _poolManager;

    private AddressablePool _progressBarPool;

    private async void Start()
    {
        _poolManager = ServiceProvider.Instance.AddressablePoolManager;

        await _poolManager.GeneratePool(_progressBarPrefab);

        _progressBarPool = _poolManager.GetPool(_progressBarPrefab);

        ListenEvents();
    }

    private void ListenEvents()
    {
        CollectableItem.OnCollectionStarted += OnCollectionStarted;
    }

    private async void OnCollectionStarted(CollectableItem item)
    {
        GameObject progressBarGameObject = await _progressBarPool.GetItem();

        ProgressBar progressBar = progressBarGameObject.GetComponent<ProgressBar>();

        progressBar.Init(item.Transform, item.Duration, _progressBarPool, _canvasTransform);
    }

    private void UnsubscribeToEvents()
    {
        CollectableItem.OnCollectionStarted -= OnCollectionStarted;
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }
}
