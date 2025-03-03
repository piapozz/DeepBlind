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

public class EventGoal : MonoBehaviour, IEvent 
{
    [SerializeField] UIManager uiManager;

    void Start()
    {
        GameObject targetObject = GameObject.Find("UIManager");

        uiManager = targetObject.GetComponent<UIManager>();
    }

    /// <summary>
    /// ���s�����C�x���g����
    /// </summary>
    public void Event()
    {
        FadeScreen.instance.FadeOutRun();

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
        // 3�b�ԑ҂�
        await Task.Delay(TimeSpan.FromSeconds(2));

        SceneManager.LoadScene("Result");
    }
}
