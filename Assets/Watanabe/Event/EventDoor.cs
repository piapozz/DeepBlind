using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDoor : MonoBehaviour, IEvent
{
    [SerializeField] UIManager uiManager;

    Animator animator;

    void Start()
    {
        GameObject targetObject = GameObject.Find("UIManager");

        uiManager = targetObject.GetComponent<UIManager>();
        animator = GetComponent<Animator>();
    }

    // �h�A�̊J���߂����s
    public void Event()
    {
        EnableInteractUI();
        OpenDoor();
    }

    // �h�A�̏�Ԃ����ēK�؂�UI��\������
    public void EnableInteractUI()
    {
        if (GetOpen() == true)
            uiManager.DisplayIntractUI("Close:E");
        else
            uiManager.DisplayIntractUI("Open:E");

    }

    // UI���\��
    public void DisableInteractUI()
    {
        uiManager.DisableIntractUI();
    }

    // �h�A�̊J����
    public void OpenDoor()
    {
        if (animator.GetBool("open") == true)
            animator.SetBool("open", false);
        else
            animator.SetBool("open", true);
    }

    // open�̃A�j���[�V�����̏�Ԃ�����Bool��Ԃ�
    public bool GetOpen()
    {
        return animator.GetBool("open");
    }
}