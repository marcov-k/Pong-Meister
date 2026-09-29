using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class InfDataFile
{
    public List<string> upgradesUsed = new List<string>();

    public List<string> GetUpgradesUsed()
    {
        return upgradesUsed;
    }

    public InfDataFile(List<string> newUpgradesUsed)
    {
        upgradesUsed = newUpgradesUsed;
    }
}
