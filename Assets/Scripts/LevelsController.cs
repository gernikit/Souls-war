using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using I2.Loc;

[System.Serializable]
public enum LevelType
{
    Lawn
}
public class LevelsController : MonoBehaviour
{
    public GameObject levelInput;
    public GameObject levelIcon;
    static public int maxLevel = 30;
    static public LevelType typeLevel = LevelType.Lawn;
    static public int currentLevel = 1;

    void Start()
    {
        maxLevel = 30;
        SaveManager.LoadGame();
        LocalizationManager.CurrentLanguage = SaveManager.data.strLanguage;
        currentLevel = SaveManager.data.levelsData[typeLevel];
        levelInput.GetComponent<InputField>().text = currentLevel.ToString();
        levelIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Levels\\" + typeLevel.ToString() + "\\Icons\\" + typeLevel.ToString() + currentLevel.ToString());
    }

    
    void Update()
    {
        
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
            value = Mathf.Clamp(value, 1, SaveManager.data.levelsData[typeLevel]);
            levelInput.GetComponent<InputField>().text = value.ToString();
        }
        ChangeCurrentLevelValue(value.ToString());

    }

    public bool IsValidLevelValue(int value)
    {
        if (value >= 1 && value <= SaveManager.data.levelsData[typeLevel])
            return true;
        else
            return false;
    }

    public void ChangeCurrentLevelValue(string value)
    {
        levelInput.GetComponent<InputField>().text = value;
        currentLevel = int.Parse(value);
        levelIcon.GetComponent<Image>().sprite = Resources.Load<Sprite>("Levels\\" + typeLevel.ToString() + "\\Icons\\" + typeLevel.ToString() + currentLevel.ToString());
    }

    public void OnSelectLevel()
    {
        if (currentLevel <= SaveManager.data.levelsData[typeLevel])
            SceneManager.LoadScene(typeLevel.ToString() + currentLevel.ToString());
    }

}
