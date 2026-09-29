using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ScoreDataFile
{
    public int highScore;
    public List<HighScore> upgradeHighScores = new List<HighScore>();

    public int GetHighScore()
    {
        return highScore;
    }

    public List<HighScore> GetUpgradeHighScores()
    {
        return upgradeHighScores;
    }

    public ScoreDataFile(int newHighScore, List<HighScore> newUpgradeHighScores)
    {
        highScore = newHighScore;
        upgradeHighScores = newUpgradeHighScores;
    }
}
