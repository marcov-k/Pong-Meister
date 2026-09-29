using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class HighScore
{
    public int score;
    public List<string> upgrades = new List<string>();

    public int GetScore()
    {
        return score;
    }

    public List<string> GetUpgrades()
    {
        return upgrades;
    }

    public HighScore(int newScore, List<string> newUpgrades)
    {
        score = newScore;
        upgrades = newUpgrades;
    }
}
