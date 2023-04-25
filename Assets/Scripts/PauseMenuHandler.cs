using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuHandler : MonoBehaviour
{
    bool pause = false;

    public GameObject pauseMenuElements; //must be set in inspector!!!
    public GameObject otherElements;    //must be set in inspector!!!
    public GameObject pauseMenuButton; //must be set in inspector!!!
    public Button restartButton;//must be set in inspector!!!
    public GameObject X2Button; //must be set in inspector!!!

    // Start is called before the first frame update
    void Start()
    {
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
            pauseMenuElements.SetActive(true);
            if (Mob.gameIsStop)
            {
                restartButton.interactable = false;
                Color col = restartButton.gameObject.GetComponent<Image>().color;
                col.a = 0.7f;
                restartButton.gameObject.GetComponent<Image>().color = col;
            }
            else
            {
                restartButton.interactable = true;
                Color col = restartButton.gameObject.GetComponent<Image>().color;
                col.a = 1;
                restartButton.gameObject.GetComponent<Image>().color = col;
            }
            pauseMenuButton.SetActive(false);
            otherElements.SetActive(false);
            X2Button.SetActive(false);
            Time.timeScale = 0f;

        }
        else
        {
            pauseMenuElements.SetActive(false);
            pauseMenuButton.SetActive(true);
            X2Button.SetActive(true);
            if (Mob.gameIsStop == true)
                otherElements.SetActive(true);
            Time.timeScale = 1f;
        }
    }

    public void OnGoToSelectLevel()
    {
        SceneManager.LoadScene("MenuMain");
        PauseGame();
        Mob.gameIsStop = true; //important!!!
    }

}
