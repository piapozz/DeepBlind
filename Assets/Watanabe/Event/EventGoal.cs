/*
 * @file EventGoal.cs
 * @brief �Q�[�����I��������C�x���g����������
 * @author sein
 * @date 2025/1/17
 */

using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneChanger;

public class EventGoal : MonoBehaviour, IEvent 
{
    private UIManager uiManager;

    void Start()
    {
        uiManager = UIManager.instance;
    }

    /// <summary>
    /// ���s�����C�x���g����
    /// </summary>
    public void Event()
    {
        _ = SceneChange();
    }

    /// <summary>
    /// �߂Â����Ƃ��ɕ\�������UI��`��
    /// </summary>
    public void EnableInteractUI()
    {
        uiManager.DisplayIntractUI("Finally:E");
    }

    /// <summary>
    /// ���ꂽ�Ƃ��ɕ\������Ă���UI������
    /// </summary>
    public void DisableInteractUI()
    {
        uiManager.DisableIntractUI();
    }

    public async UniTask SceneChange()
    {
        FadeScreen.instance.FadeOutRun();

        // 3�b�ԑ҂�
        await Task.Delay(TimeSpan.FromSeconds(2));

        SceneManager.LoadScene("GameResult");

        FadeScreen.instance.FadeInRun();
    }
}
