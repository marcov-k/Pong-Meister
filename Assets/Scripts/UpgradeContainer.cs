using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UpgradeContainer", menuName = "Scriptable Objects/UpgradeContainer")]
public class UpgradeContainer : ScriptableObject
{
    public List<Upgrade> upgrades = new List<Upgrade>();

    public Upgrade GetUpgrade(string tag)
    {
        foreach (Upgrade upgrade in upgrades)
        {
            if (upgrade.tag == tag)
            {
                return upgrade;
            }
        }
        return null;
    }
}
