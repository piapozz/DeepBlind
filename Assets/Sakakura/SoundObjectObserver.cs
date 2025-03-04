/*
* @file SoundObjectMoveObserver.cs
* @brief 環境音のオブザーバー
* @author sakakura
* @date 2025/2/8
*/

using UnityEngine;

public interface SoundObjectObserver
{
    // 座標を更新する
    public void UpdatePosition(int ID, Vector3 position);

    public void SetRing(int ID, bool ring);
}
