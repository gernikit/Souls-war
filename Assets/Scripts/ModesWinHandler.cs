using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using YG;

public class ModesWinHandler : MonoBehaviour
{
    [SerializeField]
    private Button cusstomButton; //must be set in inspetor
    [SerializeField]
    private int levelUnlockCusttomBattle = 20;
    [SerializeField] private GameObject toolTip;

    private bool _isCustomBattleEnabled = false;

    private void OnEnable()
    {
        CheckUnlockLevel();
    }

    private void OnDisable()
    {
        toolTip.SetActive(false);
    }

    private void CheckUnlockLevel()
    {
        if (YandexGame.savesData.gameData.levelsData[LevelType.Lawn] < levelUnlockCusttomBattle)
        {
            _isCustomBattleEnabled = false;
            Color col = cusstomButton.gameObject.GetComponent<Image>().color;
            col.a = 0.7f;
            cusstomButton.gameObject.GetComponent<Image>().color = col;
        }
        else
        {
            _isCustomBattleEnabled = true;
            Color col = cusstomButton.gameObject.GetComponent<Image>().color;
            col.a = 1;
            cusstomButton.gameObject.GetComponent<Image>().color = col;
        }
    }

    public void OnCustomBattle()
    {
        if (_isCustomBattleEnabled)
            SceneManager.LoadScene("CustomBattle");
        else
            toolTip.SetActive(true);
    }
}
