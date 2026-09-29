using UnityEngine;
using UnityEngine.SceneManagement;

public class WinManager : MonoBehaviour
{
    void Start()
    {
        SaveCompletion();
    }

    void SaveCompletion()
    {
        PersData.gameCompleted = true;
        foreach (string upgrade in Data.upgrades)
        {
            if (!PersData.unlockedUpgrades.Contains(upgrade))
            {
                PersData.unlockedUpgrades.Add(upgrade);
            }
        }
        Saver.SavePersistentData();
        Saver.DeleteData();
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
