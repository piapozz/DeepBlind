/*
 * @file EventGoal.cs
 * @brief ゲームを終了させるイベントを実装する
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
    /// 実行されるイベント処理
    /// </summary>
    public void Event()
    {

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
}
