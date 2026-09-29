using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class HighScoreHandler : MonoBehaviour
{
    public void HandleScore(int score)
    {
        if (score > ScoreData.highScore)
        {
            ScoreData.highScore = score;
        }
        HandleUpgradeScore(score);
        Saver.SaveScoreData();
    }

    void HandleUpgradeScore(int score)
    {
        HighScore upHighScore = new HighScore(score, InfData.upgradesUsed);
        Saver.LoadScoreData();
        int index = 0;
        foreach (HighScore highScore in ScoreData.upgradeHighScores)
        {
            if (upHighScore.upgrades.Count == highScore.upgrades.Count)
            {
                if (upHighScore.upgrades.All(highScore.upgrades.Contains))
                {
                    if (upHighScore.score > highScore.score)
                    {
                        ScoreData.upgradeHighScores[index] = upHighScore;
                    }
                    return;
                }
            }
            index++;
        }
        ScoreData.upgradeHighScores.Add(upHighScore);
    }
}
