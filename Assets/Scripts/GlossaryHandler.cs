using System.Collections.Generic;
using UnityEngine;

public class LevelUnlockMobs
{
    public LevelType levelType;

    private List<TypeOfMob> typeOfMobs;
    private List<int> levelUnlock;

    public int this[TypeOfMob el]
    {
        get
        {
            return levelUnlock[typeOfMobs.IndexOf(el)];
        }

        set
        {
            if (typeOfMobs == null)
            {
                typeOfMobs = new List<TypeOfMob>();
                levelUnlock = new List<int>();
            }

            if (typeOfMobs.Contains(el))
            {
                Debug.LogError("Wrong control!!!");
            }
            else
            {
                typeOfMobs.Add(el);
                levelUnlock.Add(value);
            }
        }
    }
    public LevelUnlockMobs()
    {
        levelUnlock = new List<int>();
        typeOfMobs = new List<TypeOfMob>();
        levelType = LevelType.Lawn;

        ChangeLevelType(levelType);//default Lawn
    }

    private void ChangeLevelType(LevelType type)
    {
        levelType = type;
        if (type == LevelType.Lawn)
        {
            foreach (TypeOfMob elem in System.Enum.GetValues(typeof(TypeOfMob)))
            {
                if (elem == TypeOfMob.Peasant)
                {
                    levelUnlock.Add(1);
                    typeOfMobs.Add(TypeOfMob.Peasant);
                }
                else if (elem == TypeOfMob.Rogue)
                {
                    levelUnlock.Add(2);
                    typeOfMobs.Add(TypeOfMob.Rogue);
                }
                else if (elem == TypeOfMob.Slingshooter)
                {
                    levelUnlock.Add(6);
                    typeOfMobs.Add(TypeOfMob.Slingshooter);
                }
                else if (elem == TypeOfMob.Knight)
                {
                    levelUnlock.Add(10);
                    typeOfMobs.Add(TypeOfMob.Knight);
                }
                else if (elem == TypeOfMob.Archer)
                {
                    levelUnlock.Add(14);
                    typeOfMobs.Add(TypeOfMob.Archer);
                }
                else if (elem == TypeOfMob.Healer)
                {
                    levelUnlock.Add(19);
                    typeOfMobs.Add(TypeOfMob.Healer);
                }
            }
        }
    }
}
public class GlossaryMobs
{
    public List<TypeOfMob> typeOfMobs;
    public List<bool> typeOfMobsAvailable;

    public bool this[TypeOfMob el]
    {
        get
        {
            return typeOfMobsAvailable[typeOfMobs.IndexOf(el)];
        }

        set
        {
            if (typeOfMobs == null)
            {
                typeOfMobs = new List<TypeOfMob>();
                typeOfMobsAvailable = new List<bool>();
            }

            if (typeOfMobs.Contains(el))
            {
                Debug.LogError("Wrong control!!!");
            }
            else
            {
                typeOfMobs.Add(el);
                typeOfMobsAvailable.Add(value);
            }
        }
    }
}
public class GlossaryHandler : MonoBehaviour
{
    public GameObject informationWindow;//must be set in inspector!!!
    public GameObject mobsGridWindow;//must be set in inspector!!!
    public GameObject contentOfMobs;//must be set in inspector!!!

    public GlossaryMobs glossaryMobs;
    public LevelUnlockMobs levelUnlockMobs;

    bool updatedAvailableMobs = false;

    private void Start()
    {
        OnShowGrid();

        glossaryMobs = new GlossaryMobs();
        levelUnlockMobs = new LevelUnlockMobs();
    }

    private void Update()
    {
        if (!updatedAvailableMobs)
            ConfigurateAvailableMobs();
    }
    public void OnSelectMob(string type)
    {
        if (!System.Enum.IsDefined(typeof(TypeOfMob), type))
        {
            Debug.LogError("Wrong type!!!");
            return;
        }

        //TypeOfMob activeType = (TypeOfMob)System.Enum.Parse(typeof(TypeOfMob), type);

        informationWindow.GetComponent<InformationWinHandler>().LoadInfoForMob(type);
        //do something with info

        mobsGridWindow.SetActive(false);
        informationWindow.SetActive(true);
    }

    public void OnShowGrid()
    {
        mobsGridWindow.SetActive(true);
        informationWindow.SetActive(false);
    }

    public void ConfigurateAvailableMobs()//needed all names MOBS!!!!
    {
        foreach (TypeOfMob elem in System.Enum.GetValues(typeof(TypeOfMob)))
        {
            if (elem == TypeOfMob.All || elem == TypeOfMob.None)
                continue;

            if (levelUnlockMobs[elem] <= SaveManager.data.levelsData[levelUnlockMobs.levelType])
                glossaryMobs[elem] = true;
            else
                glossaryMobs[elem] = false;
        }

        foreach (TypeOfMob elem in System.Enum.GetValues(typeof(TypeOfMob)))
        {

            if (elem == TypeOfMob.All || elem == TypeOfMob.None)
                continue;

            if (glossaryMobs.typeOfMobsAvailable[glossaryMobs.typeOfMobs.IndexOf(elem)])
                contentOfMobs.transform.Find(elem.ToString()).gameObject.SetActive(true);
            else
                contentOfMobs.transform.Find(elem.ToString()).gameObject.SetActive(false);
        }
        updatedAvailableMobs = true;
    }
}
