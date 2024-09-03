using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] UIManager uiManager;

    void OnTriggerStay(Collider other)
    {
        Door door = other.GetComponent<Door>();

        if (door == null) return;

        if (door.GetOpen() == true)
            uiManager.DisplayIntractUI("Close:E");
        else
            uiManager.DisplayIntractUI("Open:E");

        if (Input.GetKeyDown(KeyCode.E) == true)
        {
            door.OpenDoor();
        }
    }

    void OnTriggerExit(Collider other)
    {
        Door door = other.GetComponent<Door>();
        if (door == null) return;
        uiManager.DisableIntractUI();
    }
}
