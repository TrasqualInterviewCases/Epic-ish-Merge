using UnityEngine;

namespace Gameplay.InputSystem
{
    [DefaultExecutionOrder(-20)]
    public class InputManager : MonoBehaviour
    {
        public InputBase Input { get; private set; }

        private void Awake()
        {
#if UNITY_IOS || UNITY_ANDROID
            Input = gameObject.AddComponent<TouchInput>();
#else
            Input = gameObject.AddComponent<MouseInput>();
#endif
        }
    }
}