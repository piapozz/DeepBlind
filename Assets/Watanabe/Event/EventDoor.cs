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
    [SerializeField] private BoxCollider[] _doorCollision = null;

    [SerializeField] public bool doorLock = false;

    Animator animator;

    void Start()
    {
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
            {
                UIManager.instance.DisplayIntractUI("Close:E");
            }

            else
            {
                UIManager.instance.DisplayIntractUI("Open:E");
            }
        }

        // �{������Ă�����
        else
        {
            // �C���x���g�������Č�����������
            if (doorLock) { UIManager.instance.DisplayIntractUI("Unlock the door:RightClick"); }
            // ��������������
            else { UIManager.instance.DisplayIntractUI("The door is locked..."); }
        }
    }

    /// <summary>
    /// ���ꂽ�Ƃ��ɕ\������Ă���UI������
    /// </summary>
    public void DisableInteractUI()
    {
        // UI���\��
        UIManager.instance.DisableIntractUI();
    }

    /// <summary>
    /// �h�A�̊J����
    /// </summary>
    public void OpenDoor()
    {
        if (animator.GetBool("open") == true)
        {
            animator.SetBool("open", false);
            SwitchDoorCollision(true);
        }
        else
        {
            animator.SetBool("open", true);
            SwitchDoorCollision(false);
        }

    }

    // �h�A�̌����J��
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

    private void SwitchDoorCollision(bool enable)
    {
        for (int i = 0, max = _doorCollision.Length; i < max; i++)
        {
            _doorCollision[i].enabled = enable;
        }
    }

    public void SetDoorLock(bool isLock) { doorLock = isLock; }

    public void OpenSound() { AudioManager.instance.PlaySE(SE.DOOR_OPEN); }
    public void CloseSound() { AudioManager.instance.PlaySE(SE.DOOR_CLOSE); }
}