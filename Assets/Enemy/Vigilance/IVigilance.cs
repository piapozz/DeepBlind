using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVigilance : IEnemyState
{
    // 目標位置の取得
    public void GetTarget();

    // 完全に見失ったかどうか
    public void CheckTargetLost();

    // 見つけたかどうか
    public void ChekTracking();

    // 特殊処理
    public void Ability();

    // 移動
    public void Move();

    // 情報の更新
    public void StatusUpdate();
}
