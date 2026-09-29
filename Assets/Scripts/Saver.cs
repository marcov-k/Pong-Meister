using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Web;

public static class Saver
{
    static string dataEnding = "/Data.json";
    static string settingsEnding = "/Settings.json";
    static string persEnding = "/PersistentData.json";
    static string infEnding = "/InfiniteData.json";
    static string scoreEnding = "/ScoreData.json";

    public static void SaveData()
    {
        string path = Application.persistentDataPath + dataEnding;
        DataFile data = new DataFile(Data.matchNumber, Data.upgrades, Data.pings, Data.numUpgrades);
        string saveData = JsonUtility.ToJson(data);
        File.WriteAllText(path, saveData);
    }

    public static void SaveSettings()
    {
        string path = Application.persistentDataPath + settingsEnding;
        Settings settings = new Settings(StaticSettings.masterVolume, StaticSettings.musicVolume, StaticSettings.effectsVolume);
        string saveSettings = JsonUtility.ToJson(settings);
        File.WriteAllText(path, saveSettings);
    }

    public static void SavePersistentData()
    {
        string path = Application.persistentDataPath + persEnding;
        PersDataFile persData = new PersDataFile(PersData.unlockedUpgrades, PersData.gameCompleted);
        string savePersData = JsonUtility.ToJson(persData);
        File.WriteAllText(path, savePersData);
    }

    public static void SaveInfiniteData()
    {
        string path = Application.persistentDataPath + infEnding;
        InfDataFile infData = new InfDataFile(InfData.upgradesUsed);
        string saveInfData = JsonUtility.ToJson(infData);
        File.WriteAllText(path, saveInfData);
    }

    public static void SaveScoreData()
    {
        string path = Application.persistentDataPath + scoreEnding;
        ScoreDataFile scoreData = new ScoreDataFile(ScoreData.highScore, ScoreData.upgradeHighScores);
        string saveScoreData = JsonUtility.ToJson(scoreData);
        File.WriteAllText(path, saveScoreData);
    }

    public static void LoadData()
    {
        string path = Application.persistentDataPath + dataEnding;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            DataFile data = JsonUtility.FromJson<DataFile>(json);

            Data.matchNumber = data.GetMatchNumber();
            Data.upgrades = data.GetUpgrades();
            Data.pings = data.GetPings();
            Data.numUpgrades = data.GetNumUpgrades();
        }
    }

    public static void LoadSettings()
    {
        string path = Application.persistentDataPath + settingsEnding;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            Settings settings = JsonUtility.FromJson<Settings>(json);

            StaticSettings.masterVolume = settings.GetMasterVolume();
            StaticSettings.musicVolume = settings.GetMusicVolume();
            StaticSettings.effectsVolume = settings.GetEffectsVolume();
        }
    }

    public static void LoadPersistentData()
    {
        string path = Application.persistentDataPath + persEnding;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PersDataFile persData = JsonUtility.FromJson<PersDataFile>(json);

            PersData.unlockedUpgrades = persData.GetUnlockedUpgrades();
            PersData.gameCompleted = persData.GetGameCompleted();
        }
    }

    public static void LoadInfiniteData()
    {
        string path = Application.persistentDataPath + infEnding;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            InfDataFile infData = JsonUtility.FromJson<InfDataFile>(json);

            InfData.upgradesUsed = infData.GetUpgradesUsed();
        }
    }

    public static void LoadScoreData()
    {
        string path = Application.persistentDataPath + scoreEnding;

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ScoreDataFile scoreData = JsonUtility.FromJson<ScoreDataFile>(json);

            ScoreData.highScore = scoreData.GetHighScore();
            ScoreData.upgradeHighScores = scoreData.GetUpgradeHighScores();
        }
    }

    public static void DeleteData()
    {
        string path = Application.persistentDataPath + dataEnding;

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        Data.matchNumber = 0;
        Data.upgrades.Clear();
        Data.pings = 0;
        Data.numUpgrades = 0;
    }

    public static void DeleteSettings()
    {
        string path = Application.persistentDataPath + settingsEnding;

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static void DeleteInfiniteData()
    {
        string path = Application.persistentDataPath + infEnding;

        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    public static bool DataExists()
    {
        bool exists = false;
        string path = Application.persistentDataPath + dataEnding;

        if (File.Exists(path))
        {
            exists = true;
        }

        return exists;
    }

    public static bool SettingsExists()
    {
        bool exists = false;
        string path = Application.persistentDataPath + settingsEnding;
        
        if (File.Exists(path))
        {
            exists = true;
        }

        return exists;
    }
}
