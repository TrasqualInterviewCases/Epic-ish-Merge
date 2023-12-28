using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        private Transform _target;

        private AddressablePool _pool;

        private Camera _cam;

        public void Init(Transform target, float maxValue, AddressablePool pool, Transform parent)
        {
            _target = target;
            _slider.maxValue = maxValue;
            _pool = pool;
            transform.SetParent(parent);

            _cam = Camera.main;
        }

        private void Update()
        {
            if (_target != null)
            {
                transform.position = _cam.WorldToScreenPoint(_target.position + Vector3.up * 2.5f);
            }

            if (_slider.value < _slider.maxValue)
            {
                _slider.value += Time.deltaTime;
            }
            else
            {
                ResetBar();
            }
        }

        private void ResetBar()
        {
            _target = null;

            _slider.value = 0f;
            _slider.maxValue = 1f;

            transform.SetParent(_pool.transform);
            _pool.ReturnItem(gameObject);
        }
    }
}