using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utilities.Events;

public class EndGameCanvas : MonoBehaviour
{
    [SerializeField] private Button _playAgainButton;
    [SerializeField] private GameObject _panel;

    private void Awake()
    {
        _playAgainButton.onClick.AddListener(OnPlayAgainPressed);

        Activate(false);

        ListenEvents();
    }

    private void OnPlayAgainPressed()
    {
        SceneManager.LoadScene(0);
    }

    private void ListenEvents()
    {
        EventManager.Instance.AddListener<TasksCompletedEvent>(OnTasksCompleted);
    }

    private void Activate(bool isActive)
    {
        Time.timeScale = isActive ? 0 : 1;
        _panel.SetActive(isActive);
    }

    private void OnTasksCompleted(object data)
    {
        Activate(true);
    }

    private void UnsubscribeToEvents()
    {
        EventManager.Instance.RemoveListener<TasksCompletedEvent>(OnTasksCompleted);
    }

    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }
}
