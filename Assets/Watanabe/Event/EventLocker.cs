/*
 * @file EventLocker.cs
 * @brief �v���C���[�����b�J�[�ɓ���@�\����������
 * @author sein
 * @date 2025/1/17
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventLocker : MonoBehaviour, IEvent
{
    [SerializeField] UIManager uiManager;

    Animator animator;

    void Start()
    {
        GameObject targetObject = GameObject.Find("UIManager");

        uiManager = targetObject.GetComponent<UIManager>();
        animator = GetComponent<Animator>();
    }

    /// <summary>
    /// ���s�����C�x���g����
    /// </summary>
    public void Event()
    {
        EnableInteractUI();
        OpenDoor();
    }

    /// <summary>
    /// �h�A�̏�Ԃ����ēK�؂�UI��`��
    /// </summary>
    public void EnableInteractUI()
    {
        if (GetOpen() == true)
            uiManager.DisplayIntractUI("Close:E");
        else
            uiManager.DisplayIntractUI("Open:E");

    }

    /// <summary>
    /// ���ꂽ�Ƃ��ɕ\������Ă���UI������
    /// </summary>
    public void DisableInteractUI()
    {
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

    /// <summary>
    /// Animator�̏�Ԃ�Ԃ�
    /// </summary>
    public bool GetOpen()
    {
        return animator.GetBool("open");
    }
}