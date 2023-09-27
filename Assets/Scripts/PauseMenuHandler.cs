using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject pauseMenuElements; //must be set in inspector!!!
    [SerializeField]
    private GameObject otherElements;    //must be set in inspector!!!
    [SerializeField]
    private GameObject pauseMenuButton; //must be set in inspector!!!
    [SerializeField]
    private Button restartButton;//must be set in inspector!!!
    [SerializeField]
    private GameObject X2Button; //must be set in inspector!!!

    private bool pause = false;

    private void PauseGame()
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
            
            if (Mob.gameIsStop == true)
            {
                otherElements.SetActive(true);
            }
            else
            {
                X2Button.SetActive(true);
            }
            
            Time.timeScale = X2Button.activeSelf ? X2Button.GetComponent<TimeSpeedHandler>().GetCurrentSpeed() : 1f;
        }
    }

    public void OnGoToSelectLevel()
    {
        SceneManager.LoadScene("MenuMain");
        PauseGame();
        Mob.gameIsStop = true; //important!!!
    }

}
