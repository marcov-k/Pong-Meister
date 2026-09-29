using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class InfiniteManager : MonoBehaviour
{
    List<string> selectedUpgrades = new List<string>();
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] TextMeshProUGUI upHighScoreText;
    [SerializeField] GameObject instructions;

    void Start()
    {
        ShowInstructions();
        UpdateScores();
    }

    void ShowInstructions()
    {
        instructions.SetActive(true);
    }

    void UpdateScores()
    {
        Saver.LoadScoreData();
        if (ScoreData.highScore > 0)
        {
            highScoreText.text = "High Score: " + ScoreData.highScore;
        }
        else
        {
            highScoreText.text = "High Score: N/A";
        }
        UpdateUpgradeScore();
    }

    void UpdateUpgradeScore()
    {
        int score = 0;
        foreach(HighScore highScore in ScoreData.upgradeHighScores)
        {
            if (selectedUpgrades.Count == 0 && highScore.upgrades.Count == 0)
            {
                score = highScore.score;
            }
            else if (selectedUpgrades.Count == highScore.upgrades.Count)
            {
                if (selectedUpgrades.All(highScore.upgrades.Contains))
                {
                    score = highScore.score;
                }
            }
        }
        SetUpgradeScoreText(score);
    }

    void SetUpgradeScoreText(int score)
    {
        if (score > 0)
        {
            upHighScoreText.text = "High Score with Current Upgrades: " + score;
        }
        else
        {
            upHighScoreText.text = "High Score with Current Upgrades: N/A";
        }
    }

    void UpdateUpgradeList(Upgrade upgrade, bool added)
    {
        if (added)
        {
            if (selectedUpgrades.Count < upgrade.upgradeNumber)
            {
                selectedUpgrades.Add(upgrade.upgradeName);
            }
            else
            {
                selectedUpgrades[upgrade.upgradeNumber - 1] = upgrade.upgradeName;
            }
        }
        else
        {
            selectedUpgrades.Remove(upgrade.upgradeName);
        }
        UpdateUpgradeScore();
    }

    public void AddUpgrade(Upgrade upgrade)
    {
        UpdateUpgradeList(upgrade, true);
    }

    public void RemoveUpgrade(Upgrade upgrade)
    {
        UpdateUpgradeList(upgrade, false);
    }

    public void HideInstructions()
    {
        instructions.SetActive(false);
    }

    public void StartInfinite()
    {
        SceneManager.LoadScene("InfiniteGame");
    }
}
