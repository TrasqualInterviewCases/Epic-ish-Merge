using Gameplay.InputSystem;
using Gameplay.MovementSystem;
using Gameplay.ServiceSystem;
using UnityEngine;

namespace Gameplay.CameraUtilities
{
    public class CameraMovement : MonoBehaviour
    {
        [SerializeField] private float _camMovementSpeed = 10f;
        [SerializeField] private float _camZoomSpeed = 5f;
        [SerializeField] private float _camZoomAmount = 5f;

        private Vector3 _tapStartPosition;
        private Vector3 _tapCurrentPosition;

        private Camera _cam;

        private InputBase _input;

        private ItemMovementManager _itemMovementManager;

        private Plane _plane = new Plane(Vector3.up, Vector3.zero);

        private void Start()
        {
            _input = ServiceProvider.Instance.InputManager.Input;

            _itemMovementManager = ServiceProvider.Instance.ItemMovementManager;

            _cam = Camera.main;

            ListenEvents();
        }

        private void ListenEvents()
        {
            _input.OnTapDown += OnTapStart;
            _input.OnTapHold += OnTapMoved;
            _input.OnScroll += OnScroll;
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
                _tapStartPosition = _tapCurrentPosition = ray.GetPoint(entry);
            }
        }

        private void OnTapMoved(Vector3 currentPosition)
        {
            if (_itemMovementManager.IsMovingItem())
            {
                //MoveCamera if the pointer is on the edge of the screen
            }
            else
            {
                Ray ray = _cam.ScreenPointToRay(currentPosition);

                if (_plane.Raycast(ray, out float entry))
                {
                    _tapCurrentPosition = ray.GetPoint(entry);
                }

                MoveCamera();
            }
        }

        private void MoveCamera()
        {
            Vector3 dragDelta = _tapStartPosition - _tapCurrentPosition;
            Vector3 targetPosition = transform.position + dragDelta;
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _camMovementSpeed);
        }

        private void OnScroll(float scrollAmount)
        {
            Vector3 targetCameraLocalPosition = _cam.transform.localPosition + new Vector3(0f, 0f, _camZoomAmount) * scrollAmount;
            _cam.transform.localPosition = Vector3.Lerp(_cam.transform.localPosition, targetCameraLocalPosition, Time.deltaTime * _camZoomSpeed);
        }

        private void UnSubscribeToEvents()
        {
            _input.OnTapDown -= OnTapStart;
            _input.OnTapHold -= OnTapMoved;
            _input.OnScroll -= OnScroll;
        }

        private void OnDestroy()
        {
            UnSubscribeToEvents();
        }
    }
}