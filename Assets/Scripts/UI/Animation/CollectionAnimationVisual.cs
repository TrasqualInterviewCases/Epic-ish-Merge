using DG.Tweening;
using Gameplay.ServiceSystem;
using UnityEngine;

public class CollectionAnimationVisual : MonoBehaviour
{
    private Camera _cam;

    private AnimationTarget _target;

    private AddressablePool _pool;

    private void Awake()
    {
        _cam = Camera.main;

        transform.localScale = Vector3.zero;
    }

    public void Animate(Vector3 position, AnimationTarget target, AddressablePool pool, Transform parent)
    {
        _pool = pool;

        transform.SetParent(parent);

        _target = target;

        const float ANIMATION_DURATION = 1f;

        transform.position = _cam.WorldToScreenPoint(position);

        Sequence s = DOTween.Sequence();

        s.Append(transform.DOPath(GetBezierPoints(), ANIMATION_DURATION, PathType.CubicBezier));
        s.Insert(0f, transform.DOScale(1.2f, ANIMATION_DURATION / 2f));
        s.Insert(ANIMATION_DURATION / 2f, transform.DOScale(0.4f, ANIMATION_DURATION / 2f));

        s.OnComplete(ResetVisual);
    }

    private Vector3[] GetBezierPoints()
    {
        Vector3[] pathPoints = new Vector3[3];

        pathPoints[0] = _target.position;
        pathPoints[1] = transform.position + Vector3.right * 300f + Vector3.up * 150f;
        pathPoints[2] = _target.position + Vector3.right * 150f - Vector3.up * 150f;

        return pathPoints;
    }

    private void ResetVisual()
    {
        Debug.Log("Reseting visual");

        ServiceProvider.Instance.TaskManager.TaskItemCollected();

        _target.TargetReached();

        transform.localScale = Vector3.zero;

        _pool.ReturnItem(gameObject);
    }
}
