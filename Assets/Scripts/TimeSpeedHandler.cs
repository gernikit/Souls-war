using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeSpeedHandler : MonoBehaviour
{
    List<float> speedList;
    [SerializeField]
    Text X2Button; //must be set in inspector!!!
    int currentSpeed = 0;
    // Start is called before the first frame update
    void Start()
    {
        InitSpeedList();
        X2Button.text = "X" + speedList[currentSpeed].ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitSpeedList()
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

    public float GetCurrentSpeed()
    {
        return speedList[currentSpeed];
    }
}
