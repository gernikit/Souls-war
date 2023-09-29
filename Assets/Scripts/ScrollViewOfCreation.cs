using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using YG;

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
    public static int awardsReceived = 0;
    public static int untilNextReward = 3;
    public static int currentAward = 6;
    

    [SerializeField]
    private RestartLevelData restartLevelData;//must be set in inspector!!!
    [SerializeField]
    private int countOfMoney = 5; //must be set in inspector before game!!!
    [SerializeField]
    private Text moneyTextBox;  //must be set in inspector text of money!!!
    [SerializeField]
    private GameCameraHandler cameraHandler;

    [SerializeField]
    private GameObject mobsButtonContent; //must be set in inspector before game!!!
    [SerializeField]
    private List<TypeOfMob> unavailableMobs; //must be set in inspector before game!!! default == all

    [SerializeField] private Text soulsAdText;

    [SerializeField] private List<Sprite> bodyImagePool;
    [SerializeField] private Image currentMobBody;

    [SerializeField] private GameObject rewardAd;

    private TypeOfMob activeType;
    private Dictionary<TypeOfMob, int> countOfMobs;
    private List<GameObject> playerMobs;
    private TroopsCost troopsCost;

    private bool holdingCreation = false;
    private bool removeMob = false;
    private bool showModBody = false;
    private bool showRedMobBody = false;

    private void OnEnable() => YandexGame.RewardVideoEvent += Rewarded;

    private void OnDisable() => YandexGame.RewardVideoEvent -= Rewarded;

    private void Start()
    {
        activeType = TypeOfMob.None;
        troopsCost = new TroopsCost();

        VisualizeBodyMob(false);

        if (restartLevel)
        {
            untilNextReward -= 1;
            
            if (awardsReceived < 2 && rewardAd != null && untilNextReward == 0)
            {
                soulsAdText.text = "+" + currentAward.ToString();
                rewardAd.SetActive(true);
                untilNextReward = 3;
            }
            
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
    private void Update()
    {
        ProcessInput();
        
        if (showModBody == true)
            ProcessVizualizationBodyMob();
    }
    void Rewarded(int id)
    {
        awardsReceived += 1;
        untilNextReward = 3;
        
        if (awardsReceived == 2)
            rewardAd.SetActive(false);
            
        countOfMoney += currentAward;
        currentAward += 1;
        
        moneyTextBox.text = countOfMoney.ToString();
        rewardAd.SetActive(false);
    }
    public void SaveRestartData()
    {
        restartLevelData.ClearData();
        restartLevelData.countOfMobs = new Dictionary<TypeOfMob, int>(countOfMobs);
        restartLevelData.countOfMoney = countOfMoney;

        foreach (GameObject el in playerMobs)
        {
            restartLevelData.posMobs.Add(el.transform.position); //check copy!!!
            restartLevelData.typeOfMobs.Add(el.transform.Find("model").GetComponent<Mob>().TypeOfMob); //check copy!!!
            restartLevelData.tagMobs.Add(el.transform.Find("model").tag);
        }
    }
    public void ChooseMobType(string type)
    {
        activeType = (TypeOfMob)System.Enum.Parse(typeof(TypeOfMob), type);

        VisualizeBodyMob(true);
        
        removeMob = false;
    }

    private void LoadRestartData()
    {
        countOfMobs = new Dictionary<TypeOfMob, int>(restartLevelData.countOfMobs);
        countOfMoney = restartLevelData.countOfMoney;

        playerMobs = new List<GameObject>();

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

    private void VisualizeBodyMob(bool isActive)
    {
        if (isActive == true)
        {
            currentMobBody.sprite = bodyImagePool.First(body => body.name == activeType.ToString());
            currentMobBody.SetNativeSize();
        }

        currentMobBody.enabled = isActive;
        showModBody = isActive;
    }

    private void HoldCreation(bool isActive)
    {
        cameraHandler.CanMove = !isActive;
        holdingCreation = isActive;
    }

    private void ProcessVizualizationBodyMob()
    {
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        cursorPosition.z = 0;
        currentMobBody.transform.position = cursorPosition;
        
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("UI") | LayerMask.GetMask("Mobs"));
        
        if (countOfMoney > 0 && activeType != TypeOfMob.None && hit.collider != null && hit.collider.gameObject.tag == "SpawnZone")
        {
            if (troopsCost.GetCostInSoul(activeType) <= countOfMoney && countOfMobs[activeType] <= troopsCost.GetCountOfTroops(activeType))
            {
                if (showRedMobBody == true)
                {
                    currentMobBody.color = new Color(255, 255, 255, 0.5f);
                    showRedMobBody = false;
                }
            }
        }
        else if (countOfMoney > 0 && activeType != TypeOfMob.None && hit.collider != null && hit.collider.gameObject.tag == "SpawnEnemyZone")
        {
            if (troopsCost.GetCostInSoul(activeType) <= countOfMoney && countOfMobs[activeType] <= troopsCost.GetCountOfTroops(activeType))
            {
                if (showRedMobBody == true)
                {
                    currentMobBody.color = new Color(255, 255, 255, 0.5f);
                    showRedMobBody = false;
                }
            }
        }
        else
        {
            if (showRedMobBody == false)
            {
                currentMobBody.color = new Color(255 / 255f, 135 / 255f, 135 / 255f, 0f);
                showRedMobBody = true;
            }
        }
    }

    private void ProcessInput()
    {
#if UNITY_STANDALONE  || UNITY_WEBGL
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("UI") | LayerMask.GetMask("Mobs"));
        
            if (removeMob)
            {
                List<GameObject> playerMobsCopy = new List<GameObject>(playerMobs); //slowly
                foreach (GameObject el in playerMobsCopy) //slowly using RAYCAST!!!
                {
                    if (Vector2.Distance(new Vector2(ray.origin.x, ray.origin.y), el.transform.position) < 1.0f)
                    {
                        TypeOfMob mobType = el.transform.Find("model").GetComponent<Mob>().TypeOfMob;
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
                    HoldCreation(true);
                }
            }
            else if (countOfMoney > 0 && activeType != TypeOfMob.None && hit.collider != null && hit.collider.gameObject.tag == "SpawnEnemyZone")
            {
                if (troopsCost.GetCostInSoul(activeType) <= countOfMoney && countOfMobs[activeType] <= troopsCost.GetCountOfTroops(activeType))
                {
                    HoldCreation(true);
                }
            }
        }
        
        if (Input.GetMouseButtonUp(0) && holdingCreation)
        {
            HoldCreation(false);
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("UI") | LayerMask.GetMask("Mobs"));//UI!!!

            if (EventSystem.current.IsPointerOverGameObject())
                return;
                
            if (countOfMoney > 0 && activeType != TypeOfMob.None && hit.collider != null && hit.collider.gameObject.tag == "SpawnZone")
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

    private void ChangeAvailableMobsButtons()
    {
        if (unavailableMobs == null)
            return;

        foreach (TypeOfMob el in unavailableMobs)
        {
            for (int i = 0; i < mobsButtonContent.transform.childCount; i++)
            {
                Transform layoutItem = mobsButtonContent.transform.GetChild(i);
                Transform icon= layoutItem.Find(el.ToString());

                if (icon != null)
                {
                    icon.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("Mobs\\Icons\\" + el.ToString() + "Locked");
                    icon.gameObject.GetComponent<Button>().interactable = false;
                }
            }
        }
    }

    public void OnRemoveButton()
    {
        removeMob = true;
        
        HoldCreation(false);
        VisualizeBodyMob(false);
    }

    public void OnRemoveAll()
    {
        HoldCreation(false);
        
        List<GameObject> playerMobsCopy = new List<GameObject>(playerMobs); //slowly
        foreach (GameObject el in playerMobsCopy)
        {
            TypeOfMob mobType = el.transform.Find("model").GetComponent<Mob>().TypeOfMob;
            countOfMoney += troopsCost.GetCostInSoul(mobType);
            moneyTextBox.text = countOfMoney.ToString();
            playerMobs.Remove(el);
            countOfMobs[mobType] -= 1;
            Destroy(el);
        }
    }
}
