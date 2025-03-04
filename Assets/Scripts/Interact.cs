/*
 * @file Interact.cs
 * @brief インタラクトが押されたときに適切な処理を実行する機能を実装
 * @author sein
 * @date 2025/1/17
 */

using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    private IEvent iEvent;

    public void ExecuteEvent(InputAction.CallbackContext context)
    {
        // 一度飲み入力を受付
        if (!context.performed) return;
        if (iEvent == null) return;
        iEvent.Event();
    }

    void OnTriggerEnter(Collider other)
    {
        // アタッチされているイベントを取得
        IEvent triggerEvent = other.GetComponent<IEvent>();
        if (triggerEvent == null) return;
        iEvent = triggerEvent;
        iEvent.EnableInteractUI();
    }

    void OnTriggerExit(Collider other)
    {
        IEvent triggerEvent = other.GetComponent<IEvent>();
        if (triggerEvent == null) return;
        // イベントがあったらUIの非表示を実行
        iEvent.DisableInteractUI();
        iEvent = null;
    }
}
