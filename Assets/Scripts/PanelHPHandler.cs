using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelHPHandler : MonoBehaviour
{
    public bool interactable = true;

    [SerializeField]
    private GameObject panelHP;//must be set in inspector!!!
    [SerializeField]
    private Text textHP;//must be set in inspector!!!

    private GameObject targetObj;
    private void Start()
    {
        panelHP.SetActive(false);
    }

    private void Update()
    {
        if (!Mob.gameIsStop)
        {
            CheckTouch();
            if (targetObj != null)
                MoveForTarget(targetObj);
            else if (panelHP.activeSelf)
                panelHP.SetActive(false);
        }
    }
    public GameObject GetPanelHp() { return panelHP; }

    private void CheckTouch()
    {
        if (interactable)
        {
#if UNITY_ANDROID
            if (Input.touchCount > 0)//Input.mousePosition.y < gameObject.GetComponent<RectTransform>().anchoredPosition.y) //can create only under scroll bar
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                //RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("UI") | LayerMask.GetMask("Mobs"));//UI!!!

                if (EventSystem.current.IsPointerOverGameObject())//check it
                    return;

                if (hit.collider != null && hit.collider.gameObject.name == "model")
                {
                    Touch touch = Input.GetTouch(0);
                    if (touch.phase == TouchPhase.Began)
                    {
                        targetObj = hit.collider.gameObject;
                    }
                }
                else
                {
                    targetObj = null;
                    panelHP.SetActive(false);
                }
            }
#endif

#if UNITY_STANDALONE || UNITY_WEBGL//UNITY_EDITOR
            if (Input.GetMouseButtonDown(0)) //can create only under scroll bar
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, LayerMask.GetMask("UI") | LayerMask.GetMask("Mobs"));//UI!!!

                if (EventSystem.current.IsPointerOverGameObject())
                    return;

                if (hit.collider != null && hit.collider.gameObject.name == "model")
                {
                    targetObj = hit.collider.gameObject;
                }
                else
                {
                    targetObj = null;
                    panelHP.SetActive(false);
                }
            }
#endif
        }
    }

    private void MoveForTarget(GameObject target)
    {
        Mob temp = targetObj.GetComponent<Mob>();
        if (temp != null)
        {
            var yHalfExtents = targetObj.GetComponent<CapsuleCollider2D>().bounds.extents.y;

            float fullOffsetX = targetObj.transform.position.x;
            float fullOffsetY = targetObj.transform.position.y + yHalfExtents;
            panelHP.transform.position = (new Vector2(fullOffsetX, fullOffsetY));

            ChangeTextHP();//maybe will make action for changing hp mob?

            if (!panelHP.activeSelf)
                panelHP.SetActive(true);
        }
    }

    private void ChangeTextHP()
    {
        Mob temp = targetObj.GetComponent<Mob>();
        textHP.text = ((float)System.Math.Round(temp.Health, 2)).ToString();
    }
}