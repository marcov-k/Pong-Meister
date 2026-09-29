using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DataFile
{
    public int matchNumber;
    public List<string> upgrades = new List<string>();
    public int pings;
    public int numUpgrades;

    public int GetMatchNumber()
    {
        return matchNumber;
    }

    public List<string> GetUpgrades()
    {
        return upgrades;
    }

    public int GetPings()
    {
        return pings;
    }

    public int GetNumUpgrades()
    {
        return numUpgrades;
    }

    public DataFile(int newMatchNumber, List<string> newUpgrades, int newPings, int newNumUpgrades)
    {
        matchNumber = newMatchNumber;
        upgrades = newUpgrades;
        pings = newPings;
        numUpgrades = newNumUpgrades;
    }
}
