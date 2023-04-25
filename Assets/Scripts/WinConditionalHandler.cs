using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinConditionalHandler : MonoBehaviour
{
    public enum WinConditional
    {
        Annihilation,
        Defence,
        HoldOn
    }

    public WinConditional _winningConditional; // must be set in inspector!!!
    static public WinConditional winningConditional; // must be set in inspector!!!
    public GameObject resultWindow;     // must be set in inspector!!!

    public bool IsCustomBattle = false;
    public static bool gameIsRun = false;
    bool resultShowed = false;

    //Defence
    [Header("Defence")]
    public List<GameObject> _targetsDefence;// must be set in inspector!!!
    static public List<GameObject> targetsDefence;
    public bool _onlyMovingToTarget = true;// must be set in inspector!!!
    static public bool onlyMovingToTarget = true;
    [SerializeField]
    bool _saveEverybody = false;
    static public bool saveEverybody = false;

    //Hold on
    [Header("Hold On")]
    public Text secondsTimer;   // must be set in inspector!!!
    private GameObject parentPanelText;// must be set in inspector!!!
    public int secondsForHoldOn = 0;// must be set in inspector!!!
    Coroutine holdOnCoroutine;


    void Start()
    {
        winningConditional = _winningConditional;
        targetsDefence = _targetsDefence;
        onlyMovingToTarget = _onlyMovingToTarget;
        saveEverybody = _saveEverybody;

        if (secondsTimer != null)
            secondsTimer.text = (secondsForHoldOn / 60).ToString("D2") + ":" + (secondsForHoldOn % 60).ToString("D2");
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsRun && !resultShowed)
        {
            if (winningConditional == WinConditional.HoldOn && holdOnCoroutine == null)
                if (secondsForHoldOn > 0)
                    holdOnCoroutine = StartCoroutine(TimerForHoldOn());
                else
                    Debug.LogError("Incorect time for hold on!!!");

            CheckForResult();
        }
    }

    public void CheckForResult()
    {
        if (winningConditional == WinConditional.Annihilation || winningConditional == WinConditional.Defence || winningConditional == WinConditional.HoldOn)
        {
            if (MobTargets.mobsPlayer.Count == 0)
            {
                resultShowed = true;
                if (IsCustomBattle)
                {
                    resultWindow.GetComponent<ResultWindowHandler>().LoadLoseCustomBattle();
                    return;
                }
                else
                    resultWindow.GetComponent<ResultWindowHandler>().LoadLose();
            }
            else if (MobTargets.mobsEnemy.Count == 0)
            {
                resultShowed = true;
                if (IsCustomBattle)
                {
                    resultWindow.GetComponent<ResultWindowHandler>().LoadWinCustomBattle();
                    return;
                }
                else
                    resultWindow.GetComponent<ResultWindowHandler>().LoadWin();
            }
        }

        if (winningConditional == WinConditional.Defence)
        {
            if (targetsDefence == null)
            {
                Debug.LogError("TARGETS DEFENCE ARE NOT SET");
                return;
            }

            for (int i = 0; i < targetsDefence.Count; i++)
            {
                if (targetsDefence[i] == null)
                {
                    if (saveEverybody)
                    {
                        resultShowed = true;
                        resultWindow.GetComponent<ResultWindowHandler>().LoadLose();
                        return;
                    }
                    else
                        targetsDefence.RemoveAt(i);//check
                }
            }

            if (targetsDefence.Count == 0)
            {
                resultShowed = true;
                resultWindow.GetComponent<ResultWindowHandler>().LoadLose();
            }
        }

        if (winningConditional == WinConditional.HoldOn)
        {
            if (secondsForHoldOn <= 0)
            {
                resultShowed = true;
                resultWindow.GetComponent<ResultWindowHandler>().LoadWin();
            }
        }
    }

    IEnumerator TimerForHoldOn()
    {
        int min = secondsForHoldOn / 60;
        int sec = secondsForHoldOn % 60;
        secondsTimer.text = min.ToString("D2") + ":" + sec.ToString("D2");

        while (secondsForHoldOn > 0)
        {
            yield return new WaitForSeconds(1);

            secondsForHoldOn -= 1;

            min = secondsForHoldOn / 60;
            sec = secondsForHoldOn % 60;
            secondsTimer.text = min.ToString("D2") + ":" + sec.ToString("D2");
        }
    }
}
