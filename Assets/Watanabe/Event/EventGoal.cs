/*
 * @file EventGoal.cs
 * @brief ゲームを終了させるイベントを実装する
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
using static SceneChanger;

public class EventGoal : MonoBehaviour, IEvent 
{
    private UIManager uiManager;

    void Start()
    {
        uiManager = UIManager.instance;
    }

    /// <summary>
    /// 実行されるイベント処理
    /// </summary>
    public void Event()
    {
        _ = SceneChange();
    }

    /// <summary>
    /// 近づいたときに表示されるUIを描画
    /// </summary>
    public void EnableInteractUI()
    {
        uiManager.DisplayIntractUI("Finally:E");
    }

    /// <summary>
    /// 離れたときに表示されているUIを消す
    /// </summary>
    public void DisableInteractUI()
    {
        uiManager.DisableIntractUI();
    }

    public async UniTask SceneChange()
    {
        FadeScreen.instance.FadeOutRun();

        // 3秒間待つ
        await Task.Delay(TimeSpan.FromSeconds(2));

        SceneManager.LoadScene("GameResult");

        FadeScreen.instance.FadeInRun();
    }
}
