using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] UpgradeBox box1;
    [SerializeField] UpgradeBox box2;
    [SerializeField] List<Upgrade> allUpgrades = new List<Upgrade>();
    [SerializeField] TextMeshProUGUI pingText;
    [SerializeField] GameObject warning;

    void Start()
    {
        UpdatePings();
        SetUpgrades();
    }

    void SetUpgrades()
    {
        List<Upgrade> currentUpgrades = FindCurrentUpgrades(Data.numUpgrades);
        box1.SetUpgrade(currentUpgrades[0]);
        box2.SetUpgrade(currentUpgrades[1]);
        warning.SetActive(false);
    }

    void UpdatePings()
    {
        pingText.text = Data.pings.ToString();
    }

    public void BuyUpgrade(Upgrade upgrade)
    {
        if (upgrade.price <= Data.pings)
        {
            Data.pings -= upgrade.price;
            Data.upgrades.Add(upgrade.tag);
            Data.numUpgrades += 1;
            UpdatePings();
            box1.gameObject.SetActive(false);
            box2.gameObject.SetActive(false);
        }
        else
        {
            warning.SetActive(true);
        }
    }

    public void StartGame()
    {
        Saver.SaveData();
        SceneManager.LoadScene(1);
    }

    List<Upgrade> FindCurrentUpgrades(int numUpgrades)
    {
        List<Upgrade> currentUpgrades = new List<Upgrade>();
        foreach (Upgrade upgrade in allUpgrades)
        {
            if (upgrade.upgradeNumber == numUpgrades + 1)
            {
                currentUpgrades.Add(upgrade);
            }
        }
        return currentUpgrades;
    }

    public void HideWarning()
    {
        warning.SetActive(false);
    }
}
