using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class InformationWinHandler : MonoBehaviour
{
    public GameObject mobSprite;       //must be set in inspector!!!
    public GameObject mobIcon;         //must be set in inspector!!!
    public GameObject mobName;         //must be set in inspector!!!
    public GameObject mobDescription;  //must be set in inspector!!!
    public GameObject mobStats;        //must be set in inspector!!!

    public void LoadInfoForMob(string type)
    {
        MobInformation mob = Resources.Load<MobInformation>("MobInformation\\" + type.ToString());

        float difWidth = mobSprite.GetComponent<RectTransform>().rect.width / mob.mainSprite.rect.width;
        float difHeight = mobSprite.GetComponent<RectTransform>().rect.height / mob.mainSprite.rect.height;

        mobSprite.GetComponent<Image>().sprite = mob.mainSprite;
        mobSprite.GetComponent<AspectRatioFitter>().aspectRatio = mob.mainSprite.rect.width / mob.mainSprite.rect.height;
        //mobSprite.GetComponent<RectTransform>().sizeDelta = new Vector2(mob.mainSprite.rect.width, mob.mainSprite.rect.height);
        //mobSprite.transform.localScale = new Vector2(mobSprite.transform.localScale.x * difWidth, mobSprite.transform.localScale.y * difHeight);
        //mobSprite.GetComponent<RectTransform>().rect = //.width = mob.mainSprite.rect.width;

        mobIcon.GetComponent<Image>().sprite = mob.icon;

        mobDescription.GetComponent<Localize>().SetTerm(mob.descriptionTerm);
        mobName.GetComponent<Localize>().SetTerm(mob.mobNameTerm);
        mobStats.GetComponent<Localize>().SetTerm(mob.statsTerm);
        /*
        mobDescription.GetComponent<Text>().text = mob.description;
        mobName.GetComponent<Text>().text = mob.mobName;
        mobStats.GetComponent<Text>().text = mob.stats;
        */
    }
}
