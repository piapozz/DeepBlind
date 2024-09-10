using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Intract : MonoBehaviour
{
    [SerializeField] UIManager uiManager;

    bool inputStay = false;                 // ���͂��󂯕t���邩���Ǘ�����

    public bool intract = false;

    private void Update()
    {
        if (inputStay == true) Debug.Log("true");
        else Debug.Log("false");
    }

    // Actions��Fire�ɓo�^����Ă���L�[�������ꂽ�Ƃ��ɓ��͒l���擾
    public void OnFire(InputValue inputValue)
    {
        if (inputStay == true) intract = true;
        Debug.Log("�C���^");
    }

    void OnTriggerStay(Collider other)
    {
        Door door = other.GetComponent<Door>();

        if (door != null)
        {
            // �C���^���N�g�̓��͂��󂯕t����悤�ɂ���
            inputStay = true;
 
            if (door.GetOpen() == true)
                uiManager.DisplayIntractUI("Close:E");
            else
                uiManager.DisplayIntractUI("Open:E");
            
            // if (Input.GetKeyDown(KeyCode.E) == true)
            if(intract == true)
            {
                Debug.Log("�J��");
                door.OpenDoor();
            }

            // �C���^���N�g�𖳌��ɂ���
            intract = false;
        }

        Item item = other.GetComponent<Item>();

        if (item != null)
        {
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        // �C���^���N�g�̓��͎�t�𖳌�������
        inputStay = false;

        uiManager.DisableIntractUI();
    }
}
