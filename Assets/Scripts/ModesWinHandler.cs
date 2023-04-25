using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ModesWinHandler : MonoBehaviour
{
    [SerializeField]
    Button cusstomButton; //must be set in inspetor
    [SerializeField]
    int levelUnlockCusttomBattle = 20;

    bool checkToUnlockCustom = true;
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
     if (checkToUnlockCustom)
        {
            checkToUnlockCustom = false;
            CheckUnlockLevel();
        }
    }

    void CheckUnlockLevel()
    {
        if (SaveManager.data.levelsData[LevelType.Lawn] < levelUnlockCusttomBattle)
        {
            cusstomButton.interactable = false;
            Color col = cusstomButton.gameObject.GetComponent<Image>().color;
            col.a = 0.7f;
            cusstomButton.gameObject.GetComponent<Image>().color = col;
        }
        else
        {
            cusstomButton.interactable = true;
            Color col = cusstomButton.gameObject.GetComponent<Image>().color;
            col.a = 1;
            cusstomButton.gameObject.GetComponent<Image>().color = col;
        }
    }

    public void OnCustomBattle()
    {
        SceneManager.LoadScene("CustomBattle");
    }
}
