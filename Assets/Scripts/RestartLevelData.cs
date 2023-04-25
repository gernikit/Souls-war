using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RestartLevelData", menuName = "Data/RestartLevelData")]
public class RestartLevelData : ScriptableObject
{
    public Dictionary<TypeOfMob, int> countOfMobs;
    public List<Vector2> posMobs;
    public List<TypeOfMob> typeOfMobs;
    public List<string> tagMobs;
    public int countOfMoney;

    RestartLevelData()
    {
        countOfMobs = new Dictionary<TypeOfMob, int>();
        posMobs = new List<Vector2>();
        typeOfMobs = new List<TypeOfMob>();
        tagMobs = new List<string>();
    }
    public void ClearData()
    {
        countOfMobs.Clear();
        posMobs.Clear();
        typeOfMobs.Clear();
        tagMobs.Clear();
    }
}
