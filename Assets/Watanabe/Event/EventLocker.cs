/*
 * @file EventLocker.cs
 * @brief プレイヤーがロッカーに入る機能を実装する
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
    /// 実行されるイベント処理
    /// </summary>
    public void Event()
    {
        EnableInteractUI();
        OpenDoor();
    }

    /// <summary>
    /// ドアの状態を見て適切なUIを描画
    /// </summary>
    public void EnableInteractUI()
    {
        if (GetOpen() == true)
            uiManager.DisplayIntractUI("Close:E");
        else
            uiManager.DisplayIntractUI("Open:E");

    }

    /// <summary>
    /// 離れたときに表示されているUIを消す
    /// </summary>
    public void DisableInteractUI()
    {
        uiManager.DisableIntractUI();
    }

    /// <summary>
    /// ドアの開閉処理
    /// </summary>
    public void OpenDoor()
    {
        if (animator.GetBool("open") == true)
            animator.SetBool("open", false);
        else
            animator.SetBool("open", true);
    }

    /// <summary>
    /// Animatorの状態を返す
    /// </summary>
    public bool GetOpen()
    {
        return animator.GetBool("open");
    }
}