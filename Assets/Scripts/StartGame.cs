using UnityEngine;

public class StartGame : MonoBehaviour
{
    [SerializeField]
    private GameObject elementsForHide; //must be set in inspector!!!
    [SerializeField]
    private GameObject scrollOfCreation; //must be set in inspector!!!
    [SerializeField]
    private GameObject availableZone;//must be set in inspector!!!
    [SerializeField]
    private GameObject X2Button;//must be set in inspector!!!
    [SerializeField]
    private GameObject dungerousMark;//must be set in inspector!!!

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
