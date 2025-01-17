/*
 * @file EventGoal.cs
 * @brief �Q�[�����I��������C�x���g����������
 * @author sein
 * @date 2025/1/17
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
