using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Upgrade", menuName = "Scriptable Objects/Upgrade")]
public class Upgrade : ScriptableObject
{
    public string upgradeName;
    public string description;
    public string tag;
    public int abilityIndex;
    public int price;
    public int pingBonus;
    public int cooldown;
    public int upgradeNumber;
    public Sprite icon;
}
