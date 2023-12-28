using Cysharp.Threading.Tasks;
using Gameplay.ServiceSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TextPopUpManager : MonoBehaviour
{
    [SerializeField] private AssetReference _textPopUpPrefab;

    [SerializeField] private Transform _canvasTransform;

    AddressablePoolManager _poolManager;

    public void Start()
    {
        _poolManager = ServiceProvider.Instance.AddressablePoolManager;

        GeneratePool();
    }

    private void GeneratePool()
    {
        _poolManager.GeneratePool(_textPopUpPrefab).Forget();
    }

    public async void GetTextPopUp(string text, Vector3 position)
    {
        AddressablePool pool = _poolManager.GetPool(_textPopUpPrefab);

        GameObject textPopUpGameObject = await pool.GetItem();

        TextPopUp textPopUp = textPopUpGameObject.GetComponent<TextPopUp>();

        textPopUp.PlayText(text, position, pool, _canvasTransform);
    }
}
