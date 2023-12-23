using Gameplay.InputSystem;
using Gameplay.ServiceSystem;
using UnityEngine;

namespace Gameplay.CameraUtilities
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float _camMovementSpeed = 10f;

        private Vector3 _tapStartPosition;
        private Vector3 _tapCurrentPosition;

        private Camera _cam;

        private InputBase _input;

        private Plane _plane = new Plane(Vector3.up, Vector3.zero);

        private void Start()
        {
            _input = ServiceProvider.Instance.InputManager.Input;
            _input.OnTapDown += OnTapStart;
            _input.OnTapHold += OnTapMoved;

            _cam = Camera.main;
        }

        private void OnTapStart(Vector3 startPosition)
        {
            if (_input.IsPointerOverUI())
            {
                return;
            }

            Ray ray = _cam.ScreenPointToRay(startPosition);

            if (_plane.Raycast(ray, out float entry))
            {
                _tapStartPosition = ray.GetPoint(entry);
            }
        }

        private void OnTapMoved(Vector3 currentPosition)
        {
            Ray ray = _cam.ScreenPointToRay(currentPosition);

            if (_plane.Raycast(ray, out float entry))
            {
                _tapCurrentPosition = ray.GetPoint(entry);
            }

            MoveCamera();
        }

        private void MoveCamera()
        {
            Vector3 dragDelta = _tapStartPosition - _tapCurrentPosition;
            Vector3 targetPosition = _cam.transform.position + dragDelta;
            _cam.transform.position = Vector3.Lerp(_cam.transform.position, targetPosition, Time.deltaTime * _camMovementSpeed);
        }
    }
}