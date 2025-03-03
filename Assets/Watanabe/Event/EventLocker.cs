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
    private Vector3 enterPos;
    private Vector3 exitPos;
    private Quaternion enterRot;
    private Quaternion exitRot;

    [SerializeField] private GameObject lockerEnterAnker = null;
    [SerializeField] private GameObject lockerExitAnker = null;

    private UIManager uiManager = null;
    private Player player = null;
    private bool inPlayer; 

    void Start()
    {
        uiManager = UIManager.instance;
        player = Player.instance;

        enterPos = lockerEnterAnker.transform.position;
        exitPos = lockerExitAnker.transform.position;

        enterRot = lockerEnterAnker.transform.rotation;
        exitRot = lockerExitAnker.transform.rotation;

        inPlayer = false;
    }

    /// <summary>
    /// 実行されるイベント処理
    /// </summary>
    public void Event()
    {
        EnableInteractUI();
        ActionLocker();
    }

    /// <summary>
    /// ドアの状態を見て適切なUIを描画
    /// </summary>
    public void EnableInteractUI()
    {
        if (inPlayer == true)
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
        // ロッカーから出る
        if (inPlayer == true)
        {
            player.SetRotate(exitRot);
            player.SetPosition(exitPos);
            player.SetCharaController(true);
            inPlayer = false;
        }
        // ロッカーに入る
        else 
        {
            player.SetCharaController(false);
            player.SetRotate(enterRot);
            player.SetPosition(enterPos);
            inPlayer = true;
        }
    }
}