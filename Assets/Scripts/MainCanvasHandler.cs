using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainCanvasHandler : MonoBehaviour
{
    [SerializeField]
    GameObject Interface;//must be set in inspector
    [SerializeField]
    GameObject showOrHideBut;//must be set in inspector

    bool showedInterface = true;
    float startPosY = 0;

    void Start()
    {
        startPosY = Interface.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        
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
