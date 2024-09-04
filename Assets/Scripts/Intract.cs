using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intract : MonoBehaviour
{
    [SerializeField] UIManager uiManager;

    void OnTriggerStay(Collider other)
    {
        Door door = other.GetComponent<Door>();

        if (door != null)
        {
            if (door.GetOpen() == true)
                uiManager.DisplayIntractUI("Close:E");
            else
                uiManager.DisplayIntractUI("Open:E");

            if (Input.GetKeyDown(KeyCode.E) == true)
            {
                door.OpenDoor();
            }
        }

        Item item = other.GetComponent<Item>();

        if (item != null)
        {
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        uiManager.DisableIntractUI();
    }
}
