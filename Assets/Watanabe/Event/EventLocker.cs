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
    /// 実行されるイベント処理
    /// </summary>
    public void Event()
    {
        ActionLocker();
    }

    /// <summary>
    /// ドアの状態を見て適切なUIを描画
    /// </summary>
    public void EnableInteractUI()
    {
        if (inPlayer == false)
            uiManager.DisplayIntractUI("Enter:E");
        else
            uiManager.DisplayIntractUI("Exit:E");

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
    public void ActionLocker()
    {
        Vector3 nextPos;
        Quaternion nextRot;

        // ロッカーから出る
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
        // ロッカーに入る
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