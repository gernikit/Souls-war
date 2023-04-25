using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MonInfo", menuName = "")]
public class MobInformation : ScriptableObject
{
    public Sprite mainSprite;
    public Sprite icon;
    public string mobNameTerm;
    public string descriptionTerm;
    public string statsTerm;
}
