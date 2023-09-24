using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using I2.Loc;
using YG;

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

    [SerializeField] private GameObject reviewWindow;

    private void Start()
    {
        winElements.SetActive(false);
        
        if (reviewWindow != null)
            reviewWindow.SetActive(false);
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
        if (YandexGame.savesData.gameData.levelsData[LevelsController.typeLevel] < LevelsController.currentLevel + 1 &&
            YandexGame.savesData.gameData.maxLevelsData[LevelsController.typeLevel] >= LevelsController.currentLevel + 1)
            YandexGame.savesData.gameData.levelsData[LevelsController.typeLevel] = LevelsController.currentLevel + 1;
        YandexGame.SaveProgress();
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
        YandexGame.SaveProgress();
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
        YandexGame.SaveProgress();
    }

    public void OnShowReviewLater()
    {
        YandexGame.savesData.canReviewThisSession = false;
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
            if (YandexGame.savesData.gameData.maxLevelsData[LevelsController.typeLevel] >= LevelsController.currentLevel + 1)
                LevelsController.currentLevel += 1;
            SceneManager.LoadScene(LevelsController.typeLevel.ToString() + (LevelsController.currentLevel).ToString());
            ScrollViewOfCreation.awardsReceived = 0;
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
        ScrollViewOfCreation.awardsReceived = 0;
        ScrollViewOfCreation.currentAward = 6;
        WinConditionalHandler.gameIsRun = false;
    }
}
