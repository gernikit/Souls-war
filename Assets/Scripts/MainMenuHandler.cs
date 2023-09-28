using UnityEngine;
using I2.Loc;
using UnityEngine.UI;
using YG;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject chooseLevels;
    [SerializeField] private GameObject arena;
    [SerializeField] private GameObject glossary;
    [SerializeField] private GameObject volumeSlider;
    [SerializeField] private GameObject authButton;
    
    void Start()
    {
        if (YandexGame.SDKEnabled)
            GetLoad();

        OnMainMenuShow();
    }

    private void OnEnable()
    {
        YandexGame.GetDataEvent += GetLoad;
    }

    private void OnDisable()
    {
        YandexGame.GetDataEvent -= GetLoad;
    }
    

    private void GetLoad()
    {
        if (YandexGame.auth == false)
            authButton.SetActive(true);
        
        if (YandexGame.savesData.isFirstLoad == true)
        {
            YandexGame.savesData.isFirstLoad = false;
            YandexGame.savesData.gameData = new GameData();
            YandexGame.savesData.gameData.SetDefaultData();
            
            YandexGame.SaveProgress();
        }
        else
        {
            volumeSlider.GetComponent<Slider>().onValueChanged.Invoke(YandexGame.savesData.gameData.volume);
            volumeSlider.GetComponent<Slider>().value = YandexGame.savesData.gameData.volume;
        }
        
        DefineLanguage();
    }

    private void DefineLanguage()
    {
        if (YandexGame.EnvironmentData.language == "ru")
            OnChangedLanguage(1);
        else if (YandexGame.EnvironmentData.language == "en")
            OnChangedLanguage(0);
        else if (YandexGame.EnvironmentData.language == "tr")
            OnChangedLanguage(2);
    }
    
    public void OnMainMenuShow()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);
        chooseLevels.SetActive(false);
        arena.SetActive(false);
        glossary.SetActive(false);
    }
    public void OnOptionsShow()
    {
        mainMenu.SetActive(false);
        options.SetActive(true);
        chooseLevels.SetActive(false);
        arena.SetActive(false);
        glossary.SetActive(false);
    }

    public void OnChooseLevelsShow()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        chooseLevels.SetActive(true);
        arena.SetActive(false);
        glossary.SetActive(false);
    }

    public void OnArenaShow()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        chooseLevels.SetActive(false);
        arena.SetActive(true);
        glossary.SetActive(false);
    }

    public void OnGlossaryShow()
    {
        mainMenu.SetActive(false);
        options.SetActive(false);
        chooseLevels.SetActive(false);
        arena.SetActive(false);
        glossary.SetActive(true);
    }

    public void SaveOptions()
    {
        YandexGame.SaveProgress();
    }

    public void OnChangedVolume(float value)
    {
        YandexGame.savesData.gameData.volume = value;
        AudioListener.volume = value;
    }

    public void OnChangedLanguage(int index)
    {
        LocalizationManager.CurrentLanguage = ((Languages)index).ToString();
    }

    public void UnlockAllLevel()
    {
        YandexGame.savesData.gameData.UnlockAllLevels();
    }
}
