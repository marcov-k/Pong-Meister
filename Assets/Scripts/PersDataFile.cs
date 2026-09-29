using UnityEngine;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

[System.Serializable]
public class PersDataFile
{
    public List<string> unlockedUpgrades = new List<string>();
    public bool gameCompleted;

    public List<string> GetUnlockedUpgrades()
    {
        return unlockedUpgrades;
    }

    public bool GetGameCompleted()
    {
        return gameCompleted;
    }

    public PersDataFile(List<string> newUnlockedUpgrades, bool newGameCompleted)
    {
        unlockedUpgrades = newUnlockedUpgrades;
        gameCompleted = newGameCompleted;
    }
}
