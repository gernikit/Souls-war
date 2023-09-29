using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using I2.Loc;
using YG;

[System.Serializable]
public enum LevelType
{
    Lawn
}
public class LevelsController : MonoBehaviour
{
    static public LevelType typeLevel = LevelType.Lawn;
    static public int currentLevel = 1;

    [SerializeField] private GameObject levelInput;
    [SerializeField] private GameObject levelIcon;
    [SerializeField] private GameObject lastLevelReach;

    private void OnEnable()
    {
        lastLevelReach.SetActive(false);
        currentLevel = YandexGame.savesData.gameData.levelsData[typeLevel];
        EnableLastLevelReach();
        levelInput.GetComponent<InputField>().text = currentLevel.ToString();
        levelIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Levels\\" + typeLevel.ToString() + "\\Icons\\" + typeLevel.ToString() + currentLevel.ToString());
    }
    
    public void ChangeCurrentLevelValue(string value)
    {
        levelInput.GetComponent<InputField>().text = value;
        currentLevel = int.Parse(value);
        EnableLastLevelReach();
        levelIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Levels\\" + typeLevel.ToString() + "\\Icons\\" + typeLevel.ToString() + currentLevel.ToString());
    }

    private void EnableLastLevelReach()
    {
        if (YandexGame.savesData.isLastLevelWon && currentLevel == YandexGame.savesData.gameData.maxLevelsData[typeLevel])
            lastLevelReach.SetActive(true);
        else
            lastLevelReach.SetActive(false);
    }
    private bool IsValidLevelValue(int value)
    {
        if (value >= 1 && value <= YandexGame.savesData.gameData.levelsData[typeLevel])
            return true;
        else
            return false;
    }

    public void OnChangeLevelValue(int offset)
    {
        int tempLevel = currentLevel + offset;
        if (IsValidLevelValue(tempLevel))
        {
            ChangeCurrentLevelValue(tempLevel.ToString());
        }
    }
    public void OnLevelInputValueChanged()
    {
        int value = currentLevel;
        if (!int.TryParse(levelInput.GetComponent<InputField>().text, out value) && levelInput.GetComponent<InputField>().text != "")
            return;

        if (value == currentLevel)
            return;

        if (!IsValidLevelValue(value))
        {
            value = Mathf.Clamp(value, 1, YandexGame.savesData.gameData.levelsData[typeLevel]);
            levelInput.GetComponent<InputField>().text = value.ToString();
        }
        ChangeCurrentLevelValue(value.ToString());

    }

    public void OnSelectLevel()
    {
        if (currentLevel <= YandexGame.savesData.gameData.levelsData[typeLevel])
            SceneManager.LoadScene(typeLevel.ToString() + currentLevel.ToString());
    }
}
