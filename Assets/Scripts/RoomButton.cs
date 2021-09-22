using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomButton : MonoBehaviour
{
    public void SelectRoom()
    {
        MainMenu.instance.SelectRoom(this);
    }
}
