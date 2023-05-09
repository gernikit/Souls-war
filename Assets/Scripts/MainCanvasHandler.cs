using UnityEngine;
using UnityEngine.UI;

public class MainCanvasHandler : MonoBehaviour
{
    [SerializeField]
    GameObject Interface;//must be set in inspector
    [SerializeField]
    GameObject showOrHideBut;//must be set in inspector

    private bool showedInterface = true;
    private float startPosY = 0;

    private void Start()
    {
        startPosY = Interface.transform.position.y;
    }

    public void OnHideOrShowInterface()
    {
        if (showedInterface)
        {
            Interface.transform.position = new Vector3(Interface.transform.position.x, Interface.transform.position.y + 10000, Interface.transform.position.z);//hide, will be change to hide Layer

            showedInterface = !showedInterface;

            Color color = showOrHideBut.GetComponent<Image>().color;
            color.a = 0.2f;
            showOrHideBut.GetComponent<Image>().color = color;
        }
        else
        {
            Interface.transform.position = new Vector3(Interface.transform.position.x, startPosY, Interface.transform.position.z);//show, will be change to hide Layer

            showedInterface = !showedInterface;

            Color color = showOrHideBut.GetComponent<Image>().color;
            color.a = 1f;
            showOrHideBut.GetComponent<Image>().color = color;
        }
    }
}
