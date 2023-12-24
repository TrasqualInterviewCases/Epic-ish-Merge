using Gameplay.GridSystem;
using Gameplay.InputSystem;
using Gameplay.LevelManagement;
using Gameplay.MovementSystem;
using UnityEngine;

namespace Gameplay.ServiceSystem
{
    public class ServiceProvider : Singleton<ServiceProvider>
    {
        [field: SerializeField] public InputManager InputManager { get; private set; }
        [field: SerializeField] public LevelManager LevelManager { get; private set; }
        [field: SerializeField] public GridManager GridManager { get; private set; }
        [field: SerializeField] public MergeableFactory MergeableFactory { get; private set; }
        [field: SerializeField] public ItemMovementManager ItemMovementManager { get; private set; }
    }
}