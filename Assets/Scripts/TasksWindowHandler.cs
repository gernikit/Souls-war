using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasksWindowHandler : MonoBehaviour
{
    public GameObject tasksWindowElements; //must be set in inspector!!!
    public GameObject otherElements;    //must be set in inspector!!!
    public GameObject pauseMenuButton; //must be set in inspector!!!
    public GameObject timerHoldOn;//must be set in inspector!!! (optional)
    bool pause = false;
    // Start is called before the first frame update
    void Start()
    {
        PauseGame();
    }

    // Update is called once per frame
    void Update()
    {
        
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
