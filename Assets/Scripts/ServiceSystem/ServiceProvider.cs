using Gameplay.GridSystem;
using Gameplay.InputSystem;
using UnityEngine;

namespace Gameplay.ServiceSystem
{
    public class ServiceProvider : Singleton<ServiceProvider>
    {
        [field: SerializeField] public InputManager InputManager { get; private set; }
        [field: SerializeField] public GridManager GridManager { get; private set; }
    }
}