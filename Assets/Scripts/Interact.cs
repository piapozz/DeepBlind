/*
 * @file Interact.cs
 * @brief インタラクトが押されたときに適切な処理を実行する機能を実装
 * @author sein
 * @date 2025/1/17
 */

using System.Collections;
using System.Collections.Generic;
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

    void OnTriggerStay(Collider other)
    {
        // アタッチされているイベントを取得
        iEvent = other.GetComponent<IEvent>();

        // イベントがあったら処理を実行
        if (iEvent != null)
        {
            // インタラクトの入力を受け付けるようにする
            inputStay = true;

            // 適したUIを描画する
            iEvent.EnableInteractUI();
            
            // プレイヤーがインタラクトを起こしたら
            if (interact == true)
            {
                // 対応するイベント実行
                iEvent.Event();
            }

            // インタラクトを無効にする
            interact = false;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // インタラクトの入力受付を無効化する
        inputStay = false;

        // イベントを取得
        iEvent = other.GetComponent<IEvent>();

        // イベントがあったらUIの非表示を実行
        if(iEvent != null) iEvent.DisableInteractUI();
    }
}
