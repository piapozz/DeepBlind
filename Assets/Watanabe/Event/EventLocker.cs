/*
 * @file EventLocker.cs
 * @brief �v���C���[�����b�J�[�ɓ���@�\����������
 * @author sein
 * @date 2025/1/17
 */

using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CommonModule;

public class EventLocker : MonoBehaviour, IEvent
{
    [SerializeField] private GameObject lockerEnterAnker = null;
    [SerializeField] private GameObject lockerExitAnker = null;
    [SerializeField] private CinemachineVirtualCamera vcam = null;

    private readonly float LOCKER_ACTION_SPEED = 1.0f;
    private readonly float FADE_SPEED = 0.7f;
    private readonly float FADE_DELAY = 0.5f;

    private bool inPlayer; 

    void Start()
    {
        inPlayer = false;
    }

    /// <summary>
    /// ���s�����C�x���g����
    /// </summary>
    public void Event()
    {
        _ = ActionLocker();
    }

    /// <summary>
    /// �h�A�̏�Ԃ����ēK�؂�UI��`��
    /// </summary>
    public void EnableInteractUI()
    {
        if (inPlayer == false)
            UIManager.instance.DisplayIntractUI("Enter:E");
        else
            UIManager.instance.DisplayIntractUI("Exit:E");

    }

    /// <summary>
    /// ���ꂽ�Ƃ��ɕ\������Ă���UI������
    /// </summary>
    public void DisableInteractUI()
    {
        UIManager.instance.DisableIntractUI();
    }

    /// <summary>
    /// �h�A�̊J����
    /// </summary>
    public async UniTask ActionLocker()
    {
        Vector3 nextPos;

        // ���b�J�[����o��
        if (inPlayer == true)
        {
            nextPos = lockerExitAnker.transform.position;

            ChangeVirtualCameraPriority(0);

            Player.instance.SetCharaController(true);
            inPlayer = false;
            Player.instance.isLocker = false;

            await FadeScreen.instance.FadeOut(FADE_SPEED);
            await Task.Delay(TimeSpan.FromSeconds(FADE_DELAY));
            await FadeScreen.instance.FadeIn(FADE_SPEED);
        }
        // ���b�J�[�ɓ���
        else 
        {
            nextPos = lockerEnterAnker.transform.position;

            ChangeVirtualCameraPriority(50);

            Player.instance.SetCharaController(false);
            inPlayer = true;
            Player.instance.isLocker = true;

            await FadeScreen.instance.FadeOut(FADE_SPEED);
            await Task.Delay(TimeSpan.FromSeconds(FADE_DELAY));
            await FadeScreen.instance.FadeIn(FADE_SPEED);

        }
    }

    private void ChangeVirtualCameraPriority(int value)
    {
        vcam.Priority = value;
    }
}