using Gameplay.GridSystem;
using UnityEngine;

namespace Gameplay.LevelManagement
{
    [DefaultExecutionOrder(-10)]
    public class LevelManager : MonoBehaviour
    {
        public GridData GridData = new ();
    }
}