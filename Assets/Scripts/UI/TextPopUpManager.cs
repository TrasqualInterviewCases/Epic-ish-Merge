using Cysharp.Threading.Tasks;
using Gameplay.ServiceSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TextPopUpManager : MonoBehaviour
{
    [SerializeField] private AssetReference _textPopUpPrefab;

    [SerializeField] private Transform _canvasTransform;

    AddressablePoolManager _poolManager;

    private bool _isPoolReady;

    public void Awake()
    {
        _poolManager = ServiceProvider.Instance.AddressablePoolManager;

        GeneratePool();
    }

    private async void GeneratePool()
    {
        await _poolManager.GeneratePool(_textPopUpPrefab);

        _isPoolReady = true;
    }

    public async void GetTextPopUp(string text, Vector3 position, float duration = 1.5f, float size = 1f)
    {
        await UniTask.WaitUntil(() => _isPoolReady);

        AddressablePool pool = _poolManager.GetPool(_textPopUpPrefab);

        GameObject textPopUpGameObject = await pool.GetItem();

        TextPopUp textPopUp = textPopUpGameObject.GetComponent<TextPopUp>();

        textPopUp.PlayText(text, position, pool, _canvasTransform, duration, size);
    }
}
