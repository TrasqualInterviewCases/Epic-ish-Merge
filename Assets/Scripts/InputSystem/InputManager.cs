using UnityEngine;
using Utilities.Events;

namespace Gameplay.InputSystem
{
    [DefaultExecutionOrder(-20)]
    public class InputManager : MonoBehaviour
    {
        public InputBase Input { get; private set; }

        private void Awake()
        {
#if (UNITY_IOS || UNITY_ANDROID)
            Input = gameObject.AddComponent<TouchInput>();
#else
            Input = gameObject.AddComponent<MouseInput>();
#endif

            ListenEvents();
        }

        private void ListenEvents()
        {
            EventManager.Instance.AddListener<MergeStartedEvent>(OnMergeStarted);
            EventManager.Instance.AddListener<MergeEndedEvent>(OnMergeEnded);
        }

        private void OnMergeStarted(object data)
        {
            ToggleInput(false);
        }

        private void OnMergeEnded(object data)
        {
            ToggleInput(true);
        }

        private void ToggleInput(bool isEnabled)
        {
            Input.enabled = isEnabled;
        }

        private void UnsubscribeToEvents()
        {
            EventManager.Instance.RemoveListener<MergeStartedEvent>(OnMergeStarted);
            EventManager.Instance.RemoveListener<MergeEndedEvent>(OnMergeEnded);
        }

        private void OnDestroy()
        {
            UnsubscribeToEvents();
            UnsubscribeToEvents();
        }
    }
}