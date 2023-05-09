using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSpeedHandler : MonoBehaviour
{
    private List<float> speedList;
    [SerializeField]
    private Text X2Button; //must be set in inspector!!!
    private int currentSpeed = 0;
    private void Start()
    {
        InitSpeedList();
        X2Button.text = "X" + speedList[currentSpeed].ToString();
    }
    public float GetCurrentSpeed()
    {
        return speedList[currentSpeed];
    }

    private void InitSpeedList()
    {
        speedList = new List<float>();
        speedList.Add(1);
        speedList.Add(2);
    }

    public void OnChangeTimeSpeed()
    {
        currentSpeed++;
        if (currentSpeed >= speedList.Count)
            currentSpeed = 0;

        Time.timeScale = speedList[currentSpeed];
        X2Button.text = "X" + speedList[currentSpeed].ToString();
    }
}
