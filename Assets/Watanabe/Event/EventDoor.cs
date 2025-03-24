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
    [SerializeField] private BoxCollider[] _doorCollision = null;

    [SerializeField] public bool doorLock = false;

    Animator animator;

    void Start()
    {
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
                UIManager.instance.DisplayIntractUI("Close:E");
            }

            else
            {
                UIManager.instance.DisplayIntractUI("Open:E");
            }
        }

        // 施錠されていたら
        else
        {
            // インベントリを見て鍵があったら
            if (doorLock) { UIManager.instance.DisplayIntractUI("Unlock the door:RightClick"); }
            // 鍵が無かったら
            else { UIManager.instance.DisplayIntractUI("The door is locked..."); }
        }
    }

    /// <summary>
    /// 離れたときに表示されているUIを消す
    /// </summary>
    public void DisableInteractUI()
    {
        // UIを非表示
        UIManager.instance.DisableIntractUI();
    }

    /// <summary>
    /// ドアの開閉処理
    /// </summary>
    public void OpenDoor()
    {
        if (animator.GetBool("open") == true)
        {
            animator.SetBool("open", false);
            SwitchDoorCollision(true);
        }
        else
        {
            animator.SetBool("open", true);
            SwitchDoorCollision(false);
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

    private void SwitchDoorCollision(bool enable)
    {
        for (int i = 0, max = _doorCollision.Length; i < max; i++)
        {
            _doorCollision[i].enabled = enable;
        }
    }

    public void SetDoorLock(bool isLock) { doorLock = isLock; }

    public void OpenSound() { AudioManager.instance.PlaySE(SE.DOOR_OPEN); }
    public void CloseSound() { AudioManager.instance.PlaySE(SE.DOOR_CLOSE); }
}