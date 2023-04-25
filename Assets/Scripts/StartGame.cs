using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public GameObject elementsForHide; //must be set in inspector!!!
    public GameObject scrollOfCreation; //must be set in inspector!!!
    public GameObject availableZone;//must be set in inspector!!!
    public GameObject X2Button;//must be set in inspector!!!
    public GameObject dungerousMark;//must be set in inspector!!!
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStart()
    {
        scrollOfCreation.GetComponent<ScrollViewOfCreation>().SaveRestartData();

        MobTargets.OnStartGame();

        Mob.OnStart();

        WinConditionalHandler.gameIsRun = true;

        X2Button.SetActive(true);

        availableZone.SetActive(false);

        elementsForHide.SetActive(false);

        dungerousMark.SetActive(false);

    }
}
