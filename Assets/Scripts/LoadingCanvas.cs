using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingCanvas : MonoBehaviour
{
    public float loadingTimeSec;

    void Update()
    {
        if (loadingTimeSec <= 0)
        {
            SceneManager.LoadScene("Main Menu");
        }        
        
        loadingTimeSec -= Time.deltaTime;
    }
}
