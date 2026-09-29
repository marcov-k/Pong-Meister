using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Settings
{
    public float masterVolume;
    public float musicVolume;
    public float effectsVolume;

    public float GetMasterVolume()
    {
        return masterVolume;
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }

    public float GetEffectsVolume()
    {
        return effectsVolume;
    }

    public Settings(float newMasterVolume, float newMusicVolume, float newEffectsVolume)
    {
        masterVolume = newMasterVolume;
        musicVolume = newMusicVolume;
        effectsVolume = newEffectsVolume;
    }
}
