using DG.Tweening;
using TMPro;
using UnityEngine;

public class AnimationTarget : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    public Vector3 position => transform.position;

    public void TargetReached()
    {
        Animate();
    }

    private void Animate()
    {
        int currentValue = int.Parse(_text.text);
        currentValue--;
        currentValue = Mathf.Max(0, currentValue);

        _text.SetText(currentValue.ToString());

        transform.DOPunchScale(Vector3.one * 1.2f, 0.2f);
    }
}
