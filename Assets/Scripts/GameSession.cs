using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (!PlayerPrefs.HasKey(MUSIC_VOLUME))
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
        MainMenu.instance.audioMixer.SetFloat("Music", GetMusicVolume());

        MainMenu.instance.audioMixer.SetFloat("SFX", GetSFXVolume());
    }

    private void SetPlayerPrefsToDefault()
    {
        PlayerPrefs.SetFloat(MUSIC_VOLUME, defaultMusicVolume);

        MainMenu.instance.audioMixer.SetFloat("Music", defaultMusicVolume);

        PlayerPrefs.SetFloat(SFX_VOLUME, defaultSFXVolume);

        MainMenu.instance.audioMixer.SetFloat("SFX", defaultSFXVolume);

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

}
