/*
 * @file EventGoal.cs
 * @brief �Q�[�����I��������C�x���g����������
 * @author sein
 * @date 2025/1/17
 */

using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventGoal : MonoBehaviour, IEvent 
{
    public bool canGoal;

    private readonly float FADE_SPEED = 0.35f;
    private readonly float FADE_DELAY = 0.5f;

    /// <summary>
    /// ���s�����C�x���g����
    /// </summary>
    public void Event()
    {
        if(canGoal) _ = SceneChange();
    }

    /// <summary>
    /// �߂Â����Ƃ��ɕ\�������UI��`��
    /// </summary>
    public void EnableInteractUI()
    {
        if (canGoal) UIManager.instance.DisplayIntractUI("Finally:E");
        else UIManager.instance.DisplayIntractUI("");
    }

    /// <summary>
    /// ���ꂽ�Ƃ��ɕ\������Ă���UI������
    /// </summary>
    public void DisableInteractUI()
    {
        UIManager.instance.DisableIntractUI();
    }

    public async UniTask SceneChange()
    {
        await FadeScreen.instance.FadeOut(FADE_SPEED);
        await Task.Delay(TimeSpan.FromSeconds(FADE_DELAY));
        SceneManager.LoadScene("GameResult");
        await FadeScreen.instance.FadeIn(FADE_SPEED);
    }

    public void UnlockDoor()
    {
        canGoal = true;
    }
}
