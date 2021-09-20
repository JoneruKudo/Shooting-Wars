using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController instance;

    private void Awake()
    {
        instance = this;
    }
}
