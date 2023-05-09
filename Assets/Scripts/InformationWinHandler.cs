using UnityEngine;
using UnityEngine.UI;
using I2.Loc;

public class InformationWinHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject mobSprite;       //must be set in inspector!!!
    [SerializeField]
    private GameObject mobIcon;         //must be set in inspector!!!
    [SerializeField]
    private GameObject mobName;         //must be set in inspector!!!
    [SerializeField]
    private GameObject mobDescription;  //must be set in inspector!!!
    [SerializeField]
    private GameObject mobStats;        //must be set in inspector!!!

    public void LoadInfoForMob(string type)
    {
        MobInformation mob = Resources.Load<MobInformation>("MobInformation\\" + type.ToString());

        float difWidth = mobSprite.GetComponent<RectTransform>().rect.width / mob.mainSprite.rect.width;
        float difHeight = mobSprite.GetComponent<RectTransform>().rect.height / mob.mainSprite.rect.height;

        mobSprite.GetComponent<Image>().sprite = mob.mainSprite;
        mobSprite.GetComponent<AspectRatioFitter>().aspectRatio = mob.mainSprite.rect.width / mob.mainSprite.rect.height;

        mobIcon.GetComponent<Image>().sprite = mob.icon;

        mobDescription.GetComponent<Localize>().SetTerm(mob.descriptionTerm);
        mobName.GetComponent<Localize>().SetTerm(mob.mobNameTerm);
        mobStats.GetComponent<Localize>().SetTerm(mob.statsTerm);
    }
}
