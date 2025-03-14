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
    [SerializeField] private GameObject lockerEnterAnker = null;
    [SerializeField] private GameObject lockerExitAnker = null;

    private UIManager uiManager = null;
    private Player player = null;
    private bool inPlayer; 

    void Start()
    {
        uiManager = UIManager.instance;
        player = Player.instance;

        inPlayer = false;
    }

    /// <summary>
    /// ���s�����C�x���g����
    /// </summary>
    public void Event()
    {
        ActionLocker();
    }

    /// <summary>
    /// �h�A�̏�Ԃ����ēK�؂�UI��`��
    /// </summary>
    public void EnableInteractUI()
    {
        if (inPlayer == false)
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
        Vector3 nextPos;
        Quaternion nextRot;

        // ���b�J�[����o��
        if (inPlayer == true)
        {
            nextPos = lockerExitAnker.transform.position;
            nextRot = lockerExitAnker.transform.rotation;

            player.SetRotate(nextRot);
            player.SetPosition(nextPos);
            player.SetCharaController(true);
            inPlayer = false;
            player.isLocker = false;
        }
        // ���b�J�[�ɓ���
        else 
        {
            nextPos = lockerEnterAnker.transform.position;
            nextRot = lockerEnterAnker.transform.rotation;

            player.SetCharaController(false);
            player.SetRotate(nextRot);
            player.SetPosition(nextPos);
            inPlayer = true;
            player.isLocker = true;
        }
    }
}