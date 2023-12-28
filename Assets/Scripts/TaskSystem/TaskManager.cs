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

    public void TaskItemCollected()
    {
        _currentCollected++;

        if (_currentCollected >= _requiredCount)
        {
            EventManager.Instance.TriggerEvent<TasksCompletedEvent>();
        }
    }
}
