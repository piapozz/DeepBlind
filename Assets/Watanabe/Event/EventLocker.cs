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
    private Vector3 enterPos;
    private Vector3 exitPos;
    private Quaternion enterRot;
    private Quaternion exitRot;

    [SerializeField] private GameObject lockerEnterAnker = null;
    [SerializeField] private GameObject lockerExitAnker = null;

    private UIManager uiManager = null;
    private Player player = null;
    private bool inPlayer; 

    void Start()
    {
        uiManager = UIManager.instance;
        player = Player.instance;

        enterPos = lockerEnterAnker.transform.position;
        exitPos = lockerExitAnker.transform.position;

        enterRot = lockerEnterAnker.transform.rotation;
        exitRot = lockerExitAnker.transform.rotation;

        inPlayer = false;
    }

    /// <summary>
    /// ���s�����C�x���g����
    /// </summary>
    public void Event()
    {
        EnableInteractUI();
        ActionLocker();
    }

    /// <summary>
    /// �h�A�̏�Ԃ����ēK�؂�UI��`��
    /// </summary>
    public void EnableInteractUI()
    {
        if (inPlayer == true)
            uiManager.DisplayIntractUI("Enter:E");
        else
            uiManager.DisplayIntractUI("Exit:E");

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
    public void ActionLocker()
    {
        // ���b�J�[����o��
        if (inPlayer == true)
        {
            player.SetRotate(exitRot);
            player.SetPosition(exitPos);
            player.SetCharaController(true);
            inPlayer = false;
        }
        // ���b�J�[�ɓ���
        else 
        {
            player.SetCharaController(false);
            player.SetRotate(enterRot);
            player.SetPosition(enterPos);
            inPlayer = true;
        }
    }
}