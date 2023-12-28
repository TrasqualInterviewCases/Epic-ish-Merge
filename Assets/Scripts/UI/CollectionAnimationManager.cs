using Gameplay.CollectableSystem;
using Gameplay.ServiceSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CollectionAnimationManager : MonoBehaviour
{
    [SerializeField] private AnimationTarget _target;

    [SerializeField] private Transform _canvasTransform;

    [SerializeField] private AssetReference _colletionVisualPrefab;

    private AddressablePoolManager _poolManager;

    private AddressablePool _pool;

    private async void Start()
    {
        _poolManager = ServiceProvider.Instance.AddressablePoolManager;

        await _poolManager.GeneratePool(_colletionVisualPrefab);

        _pool = _poolManager.GetPool(_colletionVisualPrefab);

        ListenEvents();
    }

    private void ListenEvents()
    {
        CollectableItem.OnCollectionEnded += OnCollectionEnded;
    }

    private async void OnCollectionEnded(CollectableItem item)
    {
        GameObject collectionVisualGameObject = await _pool.GetItem();

        CollectionAnimationVisual collectionVisual = collectionVisualGameObject.GetComponent<CollectionAnimationVisual>();

        collectionVisual.Animate(item.Transform.position, _target, _pool, _canvasTransform);
    }

    private void UnsubscribeToEvents()
    {
        CollectableItem.OnCollectionEnded += OnCollectionEnded;
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }
}
