using UnityEngine;

public interface IEvent
{
    // イベント
    public void Event();

    // UIを表示する
    public void EnableInteractUI();

    // UIを非表示にする
    public void DisableInteractUI();
}
