/*
 * @file EventDoor.cs
 * @brief 開閉可能なドアを実装する
 * @author sein
 * @date 2025/1/17
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDoor : MonoBehaviour, IEvent
{
    [SerializeField] UIManager uiManager;

    [SerializeField] bool doorLock = false;

    Animator animator;

    void Start()
    {
        // UIManagerを取得
        GameObject targetObject = GameObject.Find("UIManager");

        // 変数の初期化
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
        // 施錠されていなかったら処理を行えるようにする
        if (doorLock == false)
        {
            if (GetOpen() == true)
                uiManager.DisplayIntractUI("Close:E");
            else
                uiManager.DisplayIntractUI("Open:E");
        }

        // 施錠されていたら
        else
        {
            uiManager.DisplayIntractUI("The door is locked...");
        }
    }

    /// <summary>
    /// 離れたときに表示されているUIを消す
    /// </summary>
    public void DisableInteractUI()
    {
        // UIを非表示
        uiManager.DisableIntractUI();
    }

    /// <summary>
    /// ドアの開閉処理
    /// </summary>
    public void OpenDoor()
    {
        // 施錠されていなかったら
        if (doorLock == false)
        {
            if (animator.GetBool("open") == true)
                animator.SetBool("open", false);
            else
                animator.SetBool("open", true);
        }
    }

    /// <summary>
    /// Animatorの状態を返す
    /// </summary>
    public bool GetOpen()
    {
        return animator.GetBool("open");
    }
}