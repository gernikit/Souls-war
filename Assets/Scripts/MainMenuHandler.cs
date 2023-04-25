using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using I2.Loc;
using UnityEditor;
using UnityEngine.UI;

public class MainMenuHandler : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject options;
    public GameObject chooseLevels;
    public GameObject arena;
    public GameObject glossary;
    public GameObject volumeSlider;

    bool loadOptions = false;


    void Start()
    {
        OnMainMenuShow();
    }

    // Update is called once per frame
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
