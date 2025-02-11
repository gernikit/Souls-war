using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using I2.Loc;

public class ResultWindowHandler : MonoBehaviour
{
    public bool IsWin = false;
    public bool IsCustomBattle = false;

    [SerializeField]
    private GameObject scrollOfCreation; //must be set in inspector!!!

    [SerializeField]
    public GameObject winElements; //must be set in inspector!!!
    [SerializeField]
    public GameObject controlButton; //must be set in inspector!!!
    [SerializeField]
    public GameObject resultText; //must be set in inspector!!!
    [SerializeField]
    public GameObject mainImage; //must be set in inspector!!!
    [SerializeField]
    public GameObject optionButton; //must be set in inspector!!!
    [SerializeField]
    public GameObject timerHoldOn; //must be set in inspector!!! (optional)

    private void Start()
    {
        winElements.SetActive(false);
    }
    public GameObject GetResultElems()
    {
        return winElements;
    }
    public void LoadWin()
    {
        Mob.gameIsStop = true;
        Time.timeScale = 0f;
        MusicPlayer.PlayWinMusic();

        mainImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites\\Cup");
        controlButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites\\ButtonNext1");
        SpriteState state = new SpriteState();
        state.pressedSprite = Resources.Load<Sprite>("Sprites\\ButtonNext2");
        controlButton.GetComponent<Button>().spriteState = state;

        resultText.GetComponent<Localize>().SetTerm("You have won");
        IsWin = true;
        optionButton.SetActive(false);
        if (timerHoldOn != null)
            timerHoldOn.SetActive(false);
        winElements.SetActive(true);
        if (SaveManager.data.levelsData[LevelsController.typeLevel] < LevelsController.currentLevel + 1 &&
            SaveManager.data.maxLevelsData[LevelsController.typeLevel] >= LevelsController.currentLevel + 1)
            SaveManager.data.levelsData[LevelsController.typeLevel] = LevelsController.currentLevel + 1;
        SaveManager.SaveGame();
    }

    public void LoadLose()
    {
        MusicPlayer.PlayLoseMusic();
        Mob.gameIsStop = true;
        Time.timeScale = 0f;
        mainImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites\\BrokenCup");
        controlButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites\\ButtonReset1");
        SpriteState state = new SpriteState();
        state.pressedSprite = Resources.Load<Sprite>("Sprites\\ButtonReset2");
        controlButton.GetComponent<Button>().spriteState = state;
        resultText.GetComponent<Localize>().SetTerm("You have lost");
        IsWin = false;
        optionButton.SetActive(false);
        if (timerHoldOn != null)
            timerHoldOn.SetActive(false);
        winElements.SetActive(true);
    }
    public void LoadWinCustomBattle()
    {
        MusicPlayer.PlayWinMusic();
        Mob.gameIsStop = true;
        Time.timeScale = 0f;
        mainImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites\\Cup");
        controlButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites\\ButtonReset1");
        SpriteState state = new SpriteState();
        state.pressedSprite = Resources.Load<Sprite>("Sprites\\ButtonReset2");
        controlButton.GetComponent<Button>().spriteState = state;
        resultText.GetComponent<Localize>().SetTerm("You have won");
        IsCustomBattle = true;
        optionButton.SetActive(false);
        if (timerHoldOn != null)
            timerHoldOn.SetActive(false);
        winElements.SetActive(true);
        SaveManager.SaveGame();
    }

    public void LoadLoseCustomBattle()
    {
        MusicPlayer.PlayLoseMusic();
        Mob.gameIsStop = true;
        Time.timeScale = 0f;
        mainImage.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites\\BrokenCup");
        controlButton.GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprites\\ButtonReset1");
        SpriteState state = new SpriteState();
        state.pressedSprite = Resources.Load<Sprite>("Sprites\\ButtonReset2");
        controlButton.GetComponent<Button>().spriteState = state;
        resultText.GetComponent<Localize>().SetTerm("You have lost");
        IsCustomBattle = true;
        optionButton.SetActive(false);
        if (timerHoldOn != null)
            timerHoldOn.SetActive(false);
        winElements.SetActive(true);
        SaveManager.SaveGame();
    }

    public void OnResultAction()
    {
        Mob.gameIsStop = true;
        if (IsCustomBattle)
        {
            ScrollViewOfCreation.restartLevel = true;
            SceneManager.LoadScene("CustomBattle");
        }
        else if (IsWin)
        {
            if (SaveManager.data.maxLevelsData[LevelsController.typeLevel] >= LevelsController.currentLevel + 1)
                LevelsController.currentLevel += 1;
            SceneManager.LoadScene(LevelsController.typeLevel.ToString() + (LevelsController.currentLevel).ToString());
        }
        else//lose
        {
            ScrollViewOfCreation.restartLevel = true;
            SceneManager.LoadScene(LevelsController.typeLevel.ToString() + (LevelsController.currentLevel).ToString());
        }
        Time.timeScale = 1f;
        winElements.SetActive(false);
        WinConditionalHandler.gameIsRun = false;
    }

    public void OnGoMainMenu()
    {
        SceneManager.LoadScene("MenuMain");
        Time.timeScale = 1f;
        Mob.gameIsStop = true; //important!!!
        winElements.SetActive(false);
        optionButton.SetActive(true);
        WinConditionalHandler.gameIsRun = false;
    }
}
