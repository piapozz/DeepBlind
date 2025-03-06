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
    [SerializeField] private UIManager uiManager;

    [SerializeField] public bool doorLock = false;

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
        // 施錠されていなかったらドアの開け閉めを実行
        if (doorLock == false) OpenDoor();
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
            {
                uiManager.DisplayIntractUI("Close:E");
            }

            else
            {
                uiManager.DisplayIntractUI("Open:E");
            }
        }

        // 施錠されていたら
        else
        {
            // インベントリを見て鍵があったら
            if (doorLock) { uiManager.DisplayIntractUI("Unlock the door:RightClick"); }
            // 鍵が無かったら
            else { uiManager.DisplayIntractUI("The door is locked..."); }
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
        if (animator.GetBool("open") == true)
        {
            animator.SetBool("open", false);
            AudioManager.instance.PlaySE(SE.DOOR_OPEN);
        }
        else
        {
            animator.SetBool("open", true);
            AudioManager.instance.PlaySE(SE.DOOR_OPEN);
        }

    }

    // ドアの鍵を開く
    public void UnlockDoor()
    {
        if(doorLock) { doorLock = false; }
    }

    /// <summary>
    /// Animatorの状態を返す
    /// </summary>
    public bool GetOpen()
    {
        return animator.GetBool("open");
    }

    public void SetDoorLock(bool isLock) { doorLock = isLock; }
}