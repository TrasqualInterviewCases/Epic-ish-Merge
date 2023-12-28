using Gameplay.ServiceSystem;
using TMPro;
using UnityEngine;
using Utilities.Events;

public class TaskManager : MonoBehaviour
{
    [SerializeField] private int _requiredCount = 3;
    [SerializeField] private TMP_Text _taskText;

    private int _currentCollected = 0;

    private void Awake()
    {
        _taskText.SetText(_requiredCount.ToString());
    }

    private void Start()
    {
        ServiceProvider.Instance.TextPopUpManager.GetTextPopUp($"Collect {_requiredCount} Level 2 items to finish level", Vector3.zero, 3.5f, 2f);
    }

    public void TaskItemCollected()
    {
        _currentCollected++;

        if (_currentCollected >= _requiredCount)
        {
            EventManager.Instance.TriggerEvent<TasksCompletedEvent>();
        }
    }
}
