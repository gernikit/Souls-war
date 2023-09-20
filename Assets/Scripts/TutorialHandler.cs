using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHandler : MonoBehaviour
{
    [SerializeField]
    private bool enable = true;
    private int currentStep = 0;

    [SerializeField]
    private GameObject hpPanel;//must be set in inspector!!!
    private bool hpPanelFirstShow = false;

    [SerializeField]
    private GameObject X2Scaler; //must be set in inspector!!!

    [SerializeField]
    private GameObject resultWindow; //must be set in inspector!!!
    private bool resultWinFirstSwowed = false;

    [SerializeField]
    private GameObject mainCamera; //must be set in Inspector!!!
    [SerializeField]
    private Vector3 posEnemy; //must be set in Inspector!!!
    private Vector3 startPosCamera;
    private Vector3 targetPosTo;

    private bool moveCamera = false;
    private float speedCamera = 1f;
    private float progress = 0f;

    [SerializeField]
    private GameObject buttonSkip; //must be set in Inspector!!!
    [SerializeField]
    private GameObject countOfSouls; //must be set in inspector!!!
    [SerializeField]
    private GameObject textSouls;//must be set in inspector!!!
    private bool allSoulsSpent = false;

    [SerializeField]
    private List<GameObject> windowSteps; //must be set in inspector!!!

    [SerializeField]
    private GameObject loseStep; //must be set!!!
    [SerializeField]
    private GameObject winStep; //must be set!!!

    private void Start()
    {
        InitHandler();
    }
    private void Update()
    {
        ProcessingTutorial();
    }

    private void InitHandler()
    {
        if (enable)
        {
            gameObject.SetActive(true);

            startPosCamera = mainCamera.transform.position;

            foreach (GameObject el in windowSteps)
            {
                if (windowSteps.IndexOf(el) != 0)
                    el.SetActive(false);
                else
                    el.SetActive(true);
            }
        }
        else
            gameObject.SetActive(false);
    }

    private void ProcessingTutorial()
    {
        if (moveCamera)
        {
            if (mainCamera.transform.position != targetPosTo)
            {
                progress += speedCamera * Time.deltaTime;
                mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, targetPosTo, progress);
            }
            else
            {
                moveCamera = false;
                progress = 0f;
            }
        }
        if (!allSoulsSpent && currentStep == 6)//Place units
        {
            if (textSouls.GetComponent<Text>().text == 0.ToString())
            {
                allSoulsSpent = true;
                countOfSouls.GetComponent<Animator>().enabled = false;
                OnNextStep();
            }
        }
        if (hpPanel.GetComponent<PanelHPHandler>().GetPanelHp().activeSelf && currentStep == 10 && !hpPanelFirstShow)//HPBar
        {
            windowSteps[currentStep].SetActive(false);
            hpPanelFirstShow = true;
            SetTimeFromX2();
        }
        if (resultWindow.GetComponent<ResultWindowHandler>().GetResultElems().activeSelf && !resultWinFirstSwowed)
        {
            resultWinFirstSwowed = true;
            OnFinalStep();
        }
    }
    private IEnumerator WaitForNextStep(float sec)
    {
        yield return new WaitForSeconds(sec);
        if (currentStep <= windowSteps.Count)
        {
            if (currentStep == 9)//X2 Scale  
            {
                X2Scaler.SetActive(true);
            }
            else if (currentStep == 10)//HpBar
            {
                Time.timeScale = 0.5f;
                hpPanel.GetComponent<PanelHPHandler>().interactable = true;
            }
            windowSteps[currentStep].SetActive(true);
        }
        else
            gameObject.SetActive(false);
    }

    /*
     * Then there are events!
     */
    public void OnSkipTutorial()
    {
        gameObject.SetActive(false);
    }

    public void OnNextStep()
    {
        if (buttonSkip.activeSelf)//can optimize
            buttonSkip.SetActive(false);
        windowSteps[currentStep].SetActive(false);
        currentStep++;
        if (currentStep == 5)//count of souls
            countOfSouls.GetComponent<Animator>().enabled = true;
        if (currentStep <= windowSteps.Count)
            windowSteps[currentStep].SetActive(true);
        else
            gameObject.SetActive(false);
    }

    public void OnNextStep(float seconds)
    {
        if (hpPanel.GetComponent<PanelHPHandler>().interactable != false)
            hpPanel.GetComponent<PanelHPHandler>().interactable = false;
        if (buttonSkip.activeSelf)
            buttonSkip.SetActive(false);
        windowSteps[currentStep].SetActive(false);
        currentStep++;
        StartCoroutine(WaitForNextStep(seconds));
    }

    public void OnFinalStep()
    {
        if (windowSteps[currentStep].activeSelf)
            windowSteps[currentStep].SetActive(false);

        if (resultWindow.GetComponent<ResultWindowHandler>().IsWin)
            winStep.SetActive(true);
        else
            loseStep.SetActive(true);
    }

    public void SetTime(float scale)
    {
        Time.timeScale = scale;
    }

    public void SetTimeFromX2()
    {
        Time.timeScale = X2Scaler.GetComponent<TimeSpeedHandler>().GetCurrentSpeed();
    }

    public void OnTurnZoom(bool enabled)
    {
        mainCamera.GetComponent<GameCameraHandler>().enabled = enabled;
    }

    public void ResetPosCamera()
    {
        startPosCamera.z = mainCamera.transform.position.z;
        moveCamera = true;
        targetPosTo = startPosCamera;
    }

    public void OnGoToPosEnemy()
    {
        posEnemy.z = mainCamera.transform.position.z;
        moveCamera = true;
        targetPosTo = posEnemy;
    }
}
