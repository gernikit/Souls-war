using UnityEngine;

public class TasksWindowHandler : MonoBehaviour
{
    [SerializeField]
    protected GameObject tasksWindowElements; //must be set in inspector!!!
    [SerializeField]
    protected GameObject otherElements;    //must be set in inspector!!!
    [SerializeField]
    protected GameObject pauseMenuButton; //must be set in inspector!!!
    [SerializeField]
    protected GameObject timerHoldOn;//must be set in inspector!!! (optional)
    
    private bool pause = false;
    private void Start()
    {
        PauseGame();
    }

    public void PauseGame()
    {
        pause = !pause;
        if (pause)
        {
            tasksWindowElements.SetActive(true);
            pauseMenuButton.SetActive(false);
            otherElements.SetActive(false);
            if (timerHoldOn != null)
                timerHoldOn.SetActive(false);
            Time.timeScale = 0f;

        }
        else
        {
            tasksWindowElements.SetActive(false);
            pauseMenuButton.SetActive(true);
            otherElements.SetActive(true);
            if (timerHoldOn != null)
                timerHoldOn.SetActive(true);
            Time.timeScale = 1f;
        }
    }
}
