using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDoor : MonoBehaviour, IEvent
{
    [SerializeField] UIManager uiManager;

    Animator animator;

    void Start()
    {
        GameObject targetObject = GameObject.Find("UIManager");

        uiManager = targetObject.GetComponent<UIManager>();
        animator = GetComponent<Animator>();
    }

    // ドアの開け閉めを実行
    public void Event()
    {
        EnableInteractUI();
        OpenDoor();
    }

    // ドアの状態を見て適切なUIを表示する
    public void EnableInteractUI()
    {
        if (GetOpen() == true)
            uiManager.DisplayIntractUI("Close:E");
        else
            uiManager.DisplayIntractUI("Open:E");

    }

    // UIを非表示
    public void DisableInteractUI()
    {
        uiManager.DisableIntractUI();
    }

    // ドアの開閉処理
    public void OpenDoor()
    {
        if (animator.GetBool("open") == true)
            animator.SetBool("open", false);
        else
            animator.SetBool("open", true);
    }

    // openのアニメーションの状態を見てBoolを返す
    public bool GetOpen()
    {
        return animator.GetBool("open");
    }
}