using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Intract : MonoBehaviour
{
    [SerializeField] UIManager uiManager;

    bool inputStay = false;                 // 入力を受け付けるかを管理する

    public bool intract = false;

    private void Update()
    {
        if (inputStay == true) Debug.Log("true");
        else Debug.Log("false");
    }

    // ActionsのFireに登録されているキーが押されたときに入力値を取得
    public void OnFire(InputValue inputValue)
    {
        if (inputStay == true) intract = true;
        Debug.Log("インタ");
    }

    void OnTriggerStay(Collider other)
    {
        Door door = other.GetComponent<Door>();

        if (door != null)
        {
            // インタラクトの入力を受け付けるようにする
            inputStay = true;
 
            if (door.GetOpen() == true)
                uiManager.DisplayIntractUI("Close:E");
            else
                uiManager.DisplayIntractUI("Open:E");
            
            // if (Input.GetKeyDown(KeyCode.E) == true)
            if(intract == true)
            {
                Debug.Log("開閉");
                door.OpenDoor();
            }

            // インタラクトを無効にする
            intract = false;
        }

        Item item = other.GetComponent<Item>();

        if (item != null)
        {
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        // インタラクトの入力受付を無効化する
        inputStay = false;

        uiManager.DisableIntractUI();
    }
}
