using Gameplay.GridSystem;
using Gameplay.InputSystem;
using Gameplay.LevelManagement;
using Gameplay.MovementSystem;
using UnityEngine;

namespace Gameplay.ServiceSystem
{
    public class ServiceProvider : Singleton<ServiceProvider>
    {
        [field: Header("GamePlay")]
        [field: SerializeField] public InputManager InputManager { get; private set; }
        [field: SerializeField] public LevelManager LevelManager { get; private set; }
        [field: SerializeField] public GridManager GridManager { get; private set; }
        [field: SerializeField] public MergeableFactory MergeableFactory { get; private set; }
        [field: SerializeField] public ItemMovementManager ItemMovementManager { get; private set; }
        [field: SerializeField] public AddressablePoolManager AddressablePoolManager { get; private set; }
        [field: SerializeField] public TaskManager TaskManager { get; private set; }

        [field: Space(10)]
        [field: Header("UI")]
        [field: SerializeField] public TextPopUpManager TextPopUpManager { get; private set; }
    }
}