/*
 * @file EventLocker.cs
 * @brief プレイヤーがロッカーに入る機能を実装する
 * @author sein
 * @date 2025/1/17
 */

using Cinemachine;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using static CommonModule;

public class EventLocker : MonoBehaviour, IEvent
{
    [SerializeField] private GameObject lockerEnterAnker = null;
    [SerializeField] private GameObject lockerExitAnker = null;
    [SerializeField] private CinemachineVirtualCamera vcam = null;

    private readonly float LOCKER_ACTION_SPEED = 1.0f;
    private readonly float FADE_SPEED = 0.7f;
    private readonly float FADE_DELAY = 0.5f;

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
        _ = ActionLocker();
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
    public async UniTask ActionLocker()
    {
        Vector3 nextPos;

        // ロッカーから出る
        if (inPlayer == true)
        {
            nextPos = lockerExitAnker.transform.position;

            ChangeVirtualCameraPriority(0);

            player.SetPosition(nextPos);
            player.SetCharaController(true);
            inPlayer = false;
            player.isLocker = false;

            await FadeScreen.instance.FadeOut(FADE_SPEED);
            await Task.Delay(TimeSpan.FromSeconds(FADE_DELAY));
            await FadeScreen.instance.FadeIn(FADE_SPEED);
        }
        // ロッカーに入る
        else 
        {
            nextPos = lockerEnterAnker.transform.position;

            ChangeVirtualCameraPriority(50);

            player.SetCharaController(false);
            // player.SetPosition(nextPos);
            inPlayer = true;
            player.isLocker = true;

            await FadeScreen.instance.FadeOut(FADE_SPEED);
            await Task.Delay(TimeSpan.FromSeconds(FADE_DELAY));
            await FadeScreen.instance.FadeIn(FADE_SPEED);

        }
    }

    private void ChangeVirtualCameraPriority(int value)
    {
        vcam.Priority = value;
    }
}