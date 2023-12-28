using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextPopUp : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private Camera _cam;

    private AddressablePool _pool;

    private void Awake()
    {
        _cam = Camera.main;
        _text.color = new Color(1f, 1f, 1f, 0f);
        gameObject.SetActive(false);
    }

    public void PlayText(string text, Vector3 worldPosition, AddressablePool pool, Transform parent, float duration = 1.5f, float size = 1f)
    {
        transform.localScale = Vector3.one * size;

        _pool = pool;

        transform.SetParent(parent);

        gameObject.SetActive(true);

        transform.position = _cam.WorldToScreenPoint(worldPosition);

        _text.SetText(text);

        Sequence s = DOTween.Sequence();
        s.Append(_text.transform.DOLocalMoveY(3f, duration));
        s.Join(_text.DOFade(1f, 0.5f));
        s.Insert(duration * 0.85f, _text.DOFade(0f, 0.5f));

        s.OnComplete(ResetText);
    }

    private void ResetText()
    {
        transform.localScale = Vector3.one;
        transform.SetParent(_pool.transform);
        _text.SetText("");
        _text.transform.localPosition = Vector3.zero;
        gameObject.SetActive(false);
        _pool.ReturnItem(gameObject);
    }
}
