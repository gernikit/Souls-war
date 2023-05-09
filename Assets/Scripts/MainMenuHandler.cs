using UnityEngine;
using I2.Loc;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject mainMenu;
    [SerializeField]
    private GameObject options;
    [SerializeField]
    private GameObject chooseLevels;
    [SerializeField]
    private GameObject arena;
    [SerializeField]
    private GameObject glossary;
    [SerializeField]
    private GameObject volumeSlider;

    private bool loadOptions = false;


    void Start()
    {
        OnMainMenuShow();
    }

    void Update()
    {
        if (!loadOptions)
        {
            volumeSlider.GetComponent<Slider>().value = SaveManager.data.volume;
            volumeSlider.GetComponent<Slider>().onValueChanged.Invoke(SaveManager.data.volume);
            loadOptions = true;
        }
    }
    public void OnMainMenuShow()
    {
        mainMenu.SetActive(true);
        options.SetActive(false);
        chooseLevels.SetActive(false);
        arena.SetActive(false);
        glossary.SetActive(false);

        SaveManager.SaveGame();
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

    public void OnChangedVolume(float value)
    {
        SaveManager.data.volume = value;
        AudioListener.volume = value;
    }

    public void OnChangedLanguage(int index)
    {
        SaveManager.data.strLanguage = ((Languages)index).ToString();
        LocalizationManager.CurrentLanguage = ((Languages)index).ToString();
    }

    public void OnExit()
    {
        Application.Quit();
    }

    public void OnGoCommunities(string url)
    {
        Application.OpenURL(url);
    }
}
