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

    bool inputStay = false;                 // 入力を受け付けるかを管理する

    public bool interact = false;

    // ActionsのFireに登録されているキーが押されたときに入力値を取得
    public void OnFire(InputValue inputValue)
    {
        if (inputStay == true) interact = true;
    }

    private void Update()
    {
        Debug.Log(iEvent);
    }

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
