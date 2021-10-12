using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Lovatto.MobileInput;

public class GameSession : MonoBehaviour
{
    public static GameSession instance;

    public string currentRoomName = "";
    public float matchTimeDuration;
    public int maxKill;

    public float defaultMusicVolume;
    public float defaultSFXVolume;
    public float defaultCameraSensitivity; 

    private static string PLAYER_NAME = "Player Name";
    private static string MUSIC_VOLUME = "Music Volume";
    private static string SFX_VOLUME = "SFX Volume";
    private static string CAMERA_SENSITIVITY = "Camera Sensitivity";

    public AudioMixer audioMixer;

    private void Awake()
    {
        if (FindObjectsOfType<GameSession>().Length > 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey(MUSIC_VOLUME) || !PlayerPrefs.HasKey(SFX_VOLUME) || !PlayerPrefs.HasKey(CAMERA_SENSITIVITY))
        {
            SetPlayerPrefsToDefault();
        }
        else
        {
            SetPlayerPrefs();
        }
    }

    private void SetPlayerPrefs()
    {
        audioMixer.SetFloat("Music", GetMusicVolume());

        audioMixer.SetFloat("SFX", GetSFXVolume());

        SetCameraSensitivityOnMobileInputSettings(GetCameraSensitivity());
    }

    private void SetPlayerPrefsToDefault()
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME, defaultMusicVolume);

        audioMixer.SetFloat("Music", defaultMusicVolume);

        PlayerPrefs.SetFloat(SFX_VOLUME, defaultSFXVolume);

        audioMixer.SetFloat("SFX", defaultSFXVolume);

        PlayerPrefs.SetFloat(CAMERA_SENSITIVITY, defaultCameraSensitivity);
    }

    public void SetRoomName(string name)
    {
        currentRoomName = name;
    }

    public string GetPlayerName()
    {
        return PlayerPrefs.GetString(PLAYER_NAME);
    }

    public void SetPlayerName(string playerName)
    {
        PlayerPrefs.SetString(PLAYER_NAME, playerName);
    }

    public float GetMusicVolume()
    {
        return PlayerPrefs.GetFloat(MUSIC_VOLUME);
    }

    public void SetMusicVolume(float amount)
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME, amount);
    }

    public float GetSFXVolume()
    {
        return PlayerPrefs.GetFloat(SFX_VOLUME);
    }

    public void SetSFXVolume(float amount)
    {
        PlayerPrefs.SetFloat(SFX_VOLUME, amount);
    }

    public float GetCameraSensitivity()
    {
        return PlayerPrefs.GetFloat(CAMERA_SENSITIVITY);
    }

    public void SetCameraSensitivity(float amount)
    {
        PlayerPrefs.SetFloat(CAMERA_SENSITIVITY, amount);
    }

    public void ApplySettings()
    {
        audioMixer.GetFloat("Music", out float musicVol);

        GameSession.instance.SetMusicVolume(musicVol);

        audioMixer.GetFloat("SFX", out float sfxVol);

        GameSession.instance.SetSFXVolume(sfxVol);

        GameSession.instance.SetCameraSensitivity(bl_MobileInputSettings.Instance.touchPadHorizontalSensitivity);
    }

    public void CancelSettings()
    {
        SetMusicVolumeOnAudioMixer(GetMusicVolume());

        SetSFXVolumeOnAudioMixer(GetSFXVolume());

        SetCameraSensitivityOnMobileInputSettings(GetCameraSensitivity());
    }


    public void SetMusicVolumeOnAudioMixer(float amount)
    {
        audioMixer.SetFloat("Music", amount);
    }

    public void SetSFXVolumeOnAudioMixer(float amount)
    {
        audioMixer.SetFloat("SFX", amount);
    }

    public void SetCameraSensitivityOnMobileInputSettings(float amount)
    {
        bl_MobileInputSettings.Instance.touchPadHorizontalSensitivity = amount;

        bl_MobileInputSettings.Instance.touchPadVerticalSensitivity = amount;
    }

}
