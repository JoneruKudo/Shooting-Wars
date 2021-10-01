using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    public static GameSession instance;

    public string currentRoomName = "";

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

    public void SetRoomName(string name)
    {
        currentRoomName = name;
    }

}
