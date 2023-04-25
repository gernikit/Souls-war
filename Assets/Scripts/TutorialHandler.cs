using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialHandler : MonoBehaviour
{
    [SerializeField]
    bool enable = true;
    int currentStep = 0;

    [SerializeField]
    GameObject hpPanel;//must be set in inspector!!!
    bool hpPanelFirstShow = false;

    [SerializeField]
    GameObject X2Scaler; //must be set in inspector!!!

    [SerializeField]
    GameObject resultWindow; //must be set in inspector!!!
    bool resultWinFirstSwowed = false;

    //List<TutoralSteps> tutoralSteps;
    [SerializeField]
    GameObject mainCamera; //must be set in Inspector!!!
    [SerializeField]
    Vector3 posEnemy; //must be set in Inspector!!!
    Vector3 startPosCamera;
    Vector3 targetPosTo;

    bool moveCamera = false;
    float speedCamera = 1f;
    float progress = 0f;

    [SerializeField]
    GameObject buttonSkip; //must be set in Inspector!!!
    [SerializeField]
    GameObject countOfSouls; //must be set in inspector!!!
    [SerializeField]
    GameObject textSouls;//must be set in inspector!!!
    bool allSoulsSpent = false;

    [SerializeField]
    List<GameObject> windowSteps; //must be set in inspector!!!

    [SerializeField]
    GameObject loseStep; //must be set!!!
    [SerializeField]
    GameObject winStep; //must be set!!!

    // Start is called before the first frame update
    void Start()
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

            //tutoralSteps = new List<TutoralSteps>();
            //foreach (TutoralSteps el in System.Enum.GetValues(typeof(TutoralSteps)))
            //{
            //    tutoralSteps.Add(el);
            //}
        }
        else
            gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!allSoulsSpent && currentStep == 6)//Place units
        {
            if (textSouls.GetComponent<Text>().text == 0.ToString())
            {
                allSoulsSpent = true;
                countOfSouls.GetComponent<Animator>().enabled = false;
                OnNextStep();
            }
        }
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
        //windowSteps[currentStep-1].SetActive(false);
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

    IEnumerator WaitForNextStep(float sec)
    {
        yield return new WaitForSeconds(sec);
        if (currentStep <= windowSteps.Count)
        {
            if (currentStep == 9)//X2 Scale  
            {
                Time.timeScale = 0.5f;
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
        mainCamera.GetComponent<Zoom>().enabled = enabled;
    }

    public void ResetPosCamera()
    {
        startPosCamera.z = mainCamera.transform.position.z;
        moveCamera = true;
        targetPosTo = startPosCamera;
        /*
        Vector3 pos = startPosCamera;
        pos.z = mainCamera.transform.position.z;
        float speed = 0.00001f;
        float progress = 0;
        pos.z = mainCamera.transform.position.z;

        while (mainCamera.transform.position.x != pos.x)
        {
            progress += speed;
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, pos, progress);
        }
        */
    }

    public void OnGoToPosEnemy()
    {
        posEnemy.z = mainCamera.transform.position.z;
        moveCamera = true;
        targetPosTo = posEnemy;
        /*
        Vector3 pos = posEnemy;
        pos.z = mainCamera.transform.position.z;
        float speed = 0.00001f;
        float progress = 0;
        pos.z = mainCamera.transform.position.z;

        while (mainCamera.transform.position.x != pos.x)
        {
            Debug.Log(progress);
            progress += speed;
            mainCamera.transform.Translate()
            mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, pos, progress);
        }
        */
    }

}
