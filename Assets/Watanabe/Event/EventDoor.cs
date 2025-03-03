/*
 * @file EventDoor.cs
 * @brief �J�\�ȃh�A����������
 * @author sein
 * @date 2025/1/17
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDoor : MonoBehaviour, IEvent
{
    [SerializeField] private UIManager uiManager;

    [SerializeField] private bool doorLock = false;

    Animator animator;

    void Start()
    {
        // UIManager���擾
        GameObject targetObject = GameObject.Find("UIManager");

        // �ϐ��̏�����
        uiManager = targetObject.GetComponent<UIManager>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// ���s�����C�x���g����
    /// </summary>
    public void Event()
    {
        EnableInteractUI();
        // �{������Ă��Ȃ�������h�A�̊J���߂����s
        if (doorLock == false) OpenDoor();
        // �{������Ă�����������������s
        else UnlockDoor();
    }

    /// <summary>
    /// �h�A�̏�Ԃ����ēK�؂�UI��`��
    /// </summary>
    public void EnableInteractUI()
    {
        // �{������Ă��Ȃ������珈�����s����悤�ɂ���
        if (doorLock == false)
        {
            if (GetOpen() == true)
                uiManager.DisplayIntractUI("Close:E");
            else
                uiManager.DisplayIntractUI("Open:E");
        }

        // �{������Ă�����
        else
        {
            // �C���x���g�������Č�����������
            if (doorLock) { uiManager.DisplayIntractUI("Unlock the door:E"); }
            // ��������������
            else { uiManager.DisplayIntractUI("The door is locked..."); }
        }
    }

    /// <summary>
    /// ���ꂽ�Ƃ��ɕ\������Ă���UI������
    /// </summary>
    public void DisableInteractUI()
    {
        // UI���\��
        uiManager.DisableIntractUI();
    }

    /// <summary>
    /// �h�A�̊J����
    /// </summary>
    public void OpenDoor()
    {
        if (animator.GetBool("open") == true)
            animator.SetBool("open", false);
        else
            animator.SetBool("open", true);

    }

    public void UnlockDoor()
    {
        if(doorLock) { doorLock = false; }
    }

    /// <summary>
    /// Animator�̏�Ԃ�Ԃ�
    /// </summary>
    public bool GetOpen()
    {
        return animator.GetBool("open");
    }

    public void SetDoorLock(bool isLock) { doorLock = isLock; }
}