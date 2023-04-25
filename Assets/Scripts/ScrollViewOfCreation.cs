using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using I2.Loc;

public enum TypeOfMob
{
    None,
    Peasant,
    Rogue,
    Slingshooter,
    Knight,
    Archer,
    Healer,
    All
}

//information class
public class TroopsCost
{
    private Dictionary<TypeOfMob, int> costInSouls;
    private Dictionary<TypeOfMob, int> possibleCountOfTroops;
    public TroopsCost()
    {
        costInSouls = new Dictionary<TypeOfMob, int>();
        costInSouls.Add(TypeOfMob.Peasant, 5);
        costInSouls.Add(TypeOfMob.Rogue, 6);
        costInSouls.Add(TypeOfMob.Slingshooter, 8);
        costInSouls.Add(TypeOfMob.Knight, 12);
        costInSouls.Add(TypeOfMob.Archer, 10);
        costInSouls.Add(TypeOfMob.Healer, 13);

        possibleCountOfTroops = new Dictionary<TypeOfMob, int>();
        possibleCountOfTroops.Add(TypeOfMob.Peasant, 100);
        possibleCountOfTroops.Add(TypeOfMob.Rogue, 100);
        possibleCountOfTroops.Add(TypeOfMob.Slingshooter, 100);
        possibleCountOfTroops.Add(TypeOfMob.Knight, 100);
        possibleCountOfTroops.Add(TypeOfMob.Archer, 100);
        possibleCountOfTroops.Add(TypeOfMob.Healer, 100);
    }

    public int GetCostInSoul(TypeOfMob typeName)
    {
        return costInSouls[typeName];
    }

    public int GetCountOfTroops(TypeOfMob typeName)
    {
        return possibleCountOfTroops[typeName];
    }
}

public class ScrollViewOfCreation : MonoBehaviour
{
    public static bool restartLevel = false;

    public RestartLevelData restartLevelData;//must be set in inspector!!!

    public TypeOfMob activeType;
    public Dictionary<TypeOfMob, int> countOfMobs;
    public List<GameObject> playerMobs;//not logical name!
    public TroopsCost troopsCost;

    public bool removeMob = false;

    public int countOfMoney = 5; //must be set in inspector before game!!!
    public Text moneyTextBox;  //must be set in inspector text of money!!!

    public GameObject mobsButtonContent; //must be set in inspector before game!!!
    public List<TypeOfMob> unavailableMobs; //must be set in inspector before game!!! default == all

    void Start()
    {
        activeType = TypeOfMob.None;
        troopsCost = new TroopsCost();

        if (restartLevel)
        {
            restartLevel = false;
            LoadRestartData();
        }
        else 
        {
            playerMobs = new List<GameObject>();
            countOfMobs = new Dictionary<TypeOfMob, int>();

            foreach (TypeOfMob el in Enum.GetValues(typeof(TypeOfMob)))
            {
                countOfMobs.Add(el, 0);
            }
        }

        moneyTextBox.text = countOfMoney.ToString();
        ChangeAvailableMobsButtons();
    }

    private void LoadRestartData()
    {
        countOfMobs = new Dictionary<TypeOfMob, int>(restartLevelData.countOfMobs);
        countOfMoney = restartLevelData.countOfMoney;

        for (int i = 0; i < restartLevelData.posMobs.Count; i++)
        {
            GameObject mob = Instantiate(Resources.Load<GameObject>("Mobs\\" + restartLevelData.typeOfMobs[i].ToString()), restartLevelData.posMobs[i], new Quaternion());
            if (restartLevelData.tagMobs[i] == "Enemy")
            {
                mob.transform.Find("model").tag = "Enemy";
                mob.transform.Find("model").GetComponent<Mob>().SetOutline(Resources.Load<Material>("Outline/SpritesheetMaterial_Outline"));
            }
            playerMobs.Add(mob);
        }
    }

    public void SaveRestartData()
    {
        restartLevelData.ClearData();
        restartLevelData.countOfMobs = new Dictionary<TypeOfMob, int>(countOfMobs);
        restartLevelData.countOfMoney = countOfMoney;

        foreach (GameObject el in playerMobs)
        {
            restartLevelData.posMobs.Add(el.transform.position);                //check copy!!!
            restartLevelData.typeOfMobs.Add(el.transform.Find("model").GetComponent<Mob>().typeOfMob); //check copy!!!
            restartLevelData.tagMobs.Add(el.transform.Find("model").tag);
        }
    }


    void Update()
    {
#if UNITY_ANDROID
        if (Input.touchCount > 0)//Input.mousePosition.y < gameObject.GetComponent<RectTransform>().anchoredPosition.y) //can create only under scroll bar
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("UI") | LayerMask.GetMask("Mobs"));//UI!!!

            if (EventSystem.current.IsPointerOverGameObject())//check it
                return;

            if (removeMob)
            {
                List<GameObject> playerMobsCopy = new List<GameObject>(playerMobs); //slowly
                foreach (GameObject el in playerMobsCopy) //slowly using RAYCAST!!!
                {
                    if (Vector2.Distance(new Vector2(ray.origin.x, ray.origin.y), el.transform.position) < 1.0f)
                    {
                        TypeOfMob mobType = el.transform.Find("model").GetComponent<Mob>().typeOfMob;
                        countOfMoney += troopsCost.GetCostInSoul(mobType);
                        moneyTextBox.text = countOfMoney.ToString();
                        playerMobs.Remove(el);
                        countOfMobs[mobType] -= 1;
                        Destroy(el);
                    }
                }
            }
            else if (countOfMoney > 0 && activeType != TypeOfMob.None && hit.collider != null && hit.collider.gameObject.tag == "SpawnZone")
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    if (troopsCost.GetCostInSoul(activeType) <= countOfMoney && countOfMobs[activeType] <= troopsCost.GetCountOfTroops(activeType))
                    {
                        GameObject mob = Instantiate(Resources.Load<GameObject>("Mobs\\" + activeType.ToString()), new Vector2(ray.origin.x, ray.origin.y), new Quaternion());
                        countOfMoney -= troopsCost.GetCostInSoul(activeType);
                        moneyTextBox.text = countOfMoney.ToString();
                        playerMobs.Add(mob);
                        countOfMobs[activeType] += 1;
                    }
                }
            }
            else if (countOfMoney > 0 && activeType != TypeOfMob.None && hit.collider != null && hit.collider.gameObject.tag == "SpawnEnemyZone")
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    if (troopsCost.GetCostInSoul(activeType) <= countOfMoney && countOfMobs[activeType] <= troopsCost.GetCountOfTroops(activeType))
                    {
                        GameObject mob = Instantiate(Resources.Load<GameObject>("Mobs\\" + activeType.ToString()), new Vector2(ray.origin.x, ray.origin.y), new Quaternion());
                        mob.transform.Find("model").GetComponent<Mob>().tag = "Enemy";
                        mob.transform.Find("model").GetComponent<Mob>().SetOutline(Resources.Load<Material>("Outline/SpritesheetMaterial_Outline"));
                        countOfMoney -= troopsCost.GetCostInSoul(activeType);
                        moneyTextBox.text = countOfMoney.ToString();
                        playerMobs.Add(mob);
                        countOfMobs[activeType] += 1;
                    }
                }
            }
        }
#endif


#if UNITY_STANDALONE//UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))//Input.mousePosition.y < gameObject.GetComponent<RectTransform>().anchoredPosition.y) //can create only under scroll bar
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("UI") | LayerMask.GetMask("Mobs"));//UI!!!
            //Debug.Log(hit.collider.gameObject.name);

            if (EventSystem.current.IsPointerOverGameObject())
                return;

            if (removeMob)
            {
                List<GameObject> playerMobsCopy = new List<GameObject>(playerMobs); //slowly
                foreach (GameObject el in playerMobsCopy) //slowly using RAYCAST!!!
                {
                    if (Vector2.Distance(new Vector2(ray.origin.x, ray.origin.y), el.transform.position) < 1.0f)
                    {
                        TypeOfMob mobType = el.transform.Find("model").GetComponent<Mob>().typeOfMob;
                        countOfMoney += troopsCost.GetCostInSoul(mobType);
                        moneyTextBox.text = countOfMoney.ToString();
                        playerMobs.Remove(el);
                        countOfMobs[mobType] -= 1;
                        Destroy(el);
                    }
                }
            }
            else if (countOfMoney > 0 && activeType != TypeOfMob.None && hit.collider != null && hit.collider.gameObject.tag == "SpawnZone")
            {
                if (troopsCost.GetCostInSoul(activeType) <= countOfMoney && countOfMobs[activeType] <= troopsCost.GetCountOfTroops(activeType))
                {
                    GameObject mob = Instantiate(Resources.Load<GameObject>("Mobs\\" + activeType.ToString()), new Vector2(ray.origin.x, ray.origin.y), new Quaternion());
                    countOfMoney -= troopsCost.GetCostInSoul(activeType);
                    moneyTextBox.text = countOfMoney.ToString();
                    playerMobs.Add(mob);
                    countOfMobs[activeType] += 1;
                }
            }
            else if (countOfMoney > 0 && activeType != TypeOfMob.None && hit.collider != null && hit.collider.gameObject.tag == "SpawnEnemyZone")
            {
                if (troopsCost.GetCostInSoul(activeType) <= countOfMoney && countOfMobs[activeType] <= troopsCost.GetCountOfTroops(activeType))
                {
                    GameObject mob = Instantiate(Resources.Load<GameObject>("Mobs\\" + activeType.ToString()), new Vector2(ray.origin.x, ray.origin.y), new Quaternion());
                    mob.transform.Find("model").GetComponent<Mob>().tag = "Enemy";
                    mob.transform.Find("model").GetComponent<Mob>().SetOutline(Resources.Load<Material>("Outline/SpritesheetMaterial_Outline"));
                    countOfMoney -= troopsCost.GetCostInSoul(activeType);
                    moneyTextBox.text = countOfMoney.ToString();
                    playerMobs.Add(mob);
                    countOfMobs[activeType] += 1;
                }
            }
        }
#endif

    }

    public void ChooseMobType(string type)
    {
        activeType = (TypeOfMob) System.Enum.Parse(typeof(TypeOfMob), type);
        removeMob = false;
    }

    public void OnRemoveButton()
    {
        removeMob = !removeMob;
    }

    public void OnRemoveAll()
    {
        List<GameObject> playerMobsCopy = new List<GameObject>(playerMobs); //slowly
        foreach (GameObject el in playerMobsCopy)
        {
            TypeOfMob mobType = el.transform.Find("model").GetComponent<Mob>().typeOfMob;
            countOfMoney += troopsCost.GetCostInSoul(mobType);
            moneyTextBox.text = countOfMoney.ToString();
            playerMobs.Remove(el);
            countOfMobs[mobType] -= 1;
            Destroy(el);
        }
    }

    private void ChangeAvailableMobsButtons()
    {
        if (unavailableMobs == null)
            return;

        foreach(TypeOfMob el in unavailableMobs)
        {
            mobsButtonContent.transform.Find(el.ToString()).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Mobs\\Icons\\" + el.ToString() + "Locked");
            mobsButtonContent.transform.Find(el.ToString()).gameObject.GetComponent<Button>().interactable = false;
        }
    }
}
