using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class MenuManager : MonoBehaviour
{
    SettingsManager settingsManager;
    [SerializeField] GameObject infiniteWarning;
    [SerializeField] GameObject newGameWarning;

    void Awake()
    {
        settingsManager = FindFirstObjectByType<SettingsManager>();
    }

    void Start()
    {
        HandleSettings();
        HideInfiniteWarning();
        HideNewWarning();
        Saver.LoadPersistentData();
    }

    void HandleSettings()
    {
        if (Saver.SettingsExists())
        {
            Saver.LoadSettings();
        }
        else
        {
            StaticSettings.masterVolume = 1;
            StaticSettings.musicVolume = 1;
            StaticSettings.effectsVolume = 1;
            Saver.SaveSettings();
        }
    }

    public void QuitGame()
    {
        Saver.SaveData();
        Saver.SaveSettings();
        Application.Quit();
    }

    public void PlayGame()
    {
        ShowNewWarning();
    }

    void ShowNewWarning()
    {
        newGameWarning.SetActive(true);
        GameObject continueButton = newGameWarning.transform.GetChild(1).GetChild(1).gameObject;
        TextMeshProUGUI text = newGameWarning.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if (Saver.DataExists())
        {
            continueButton.SetActive(true);
            text.text = "Continue game?";
        }
        else
        {
            continueButton.SetActive(false);
            text.text = "Start new game?";
        }
    }

    public void NewGame()
    {
        Saver.DeleteData();
        SceneManager.LoadScene("Upgrades");
    }

    public void ContinueGame()
    {
        Saver.LoadData();
        SceneManager.LoadScene("Upgrades");
    }

    public void OpenSettings()
    {
        Saver.LoadSettings();
        settingsManager.OpenSettings();
    }

    public void LoadInfinite()
    {
        if (PersData.gameCompleted)
        {
            Saver.LoadScoreData();
            SceneManager.LoadScene("Infinite");
        }
        else
        {
            ShowInfiniteWarning();
        }
    }

    void ShowInfiniteWarning()
    {
        infiniteWarning.SetActive(true);
    }

    public void HideInfiniteWarning()
    {
        infiniteWarning.SetActive(false);
    }

    public void HideNewWarning()
    {
        newGameWarning.SetActive(false);
    }

    public void ResetData()
    {
        Saver.DeleteData();
        Saver.DeleteSettings();
    }
}
