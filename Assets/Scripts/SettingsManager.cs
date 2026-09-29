using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] GameObject settingsHolder;
    bool settingsOpen = false;
    List<Slider> sliders = new List<Slider>();

    void Start()
    {
        settingsHolder.SetActive(false);
        InitializeSliders();
    }

    void InitializeSliders()
    {
        Transform organizer = settingsHolder.transform.GetChild(1);
        for (int i = 0; i < organizer.childCount; i++)
        {
            sliders.Add(organizer.GetChild(i).GetComponent<Slider>());
        }
        UpdateSliders();
    }

    public void OpenSettings()
    {
        settingsOpen = true;
        UpdateSliders();
        settingsHolder.SetActive(true);
    }

    void UpdateSliders()
    {
        sliders[0].value = StaticSettings.masterVolume;
        sliders[1].value = StaticSettings.musicVolume;
        sliders[2].value = StaticSettings.effectsVolume;
    }

    public void SetMasterVolume()
    {
        StaticSettings.masterVolume = sliders[0].value;
    }

    public void SetMusicVolume()
    {
        StaticSettings.musicVolume = sliders[1].value;
    }

    public void SetEffectVolume()
    {
        StaticSettings.effectsVolume = sliders[2].value;
    }

    public void CloseSettings()
    {
        Saver.SaveSettings();
        settingsOpen = false;
        settingsHolder.SetActive(false);
    }

    public bool GetSettingsOpen()
    {
        return settingsOpen;
    }
}
