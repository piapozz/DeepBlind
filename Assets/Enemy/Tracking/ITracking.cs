using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITracking : IEnemyState
{
    // 目標位置の取得
    public void GetTarget(EnemyBase.EnemyInfo info);

    // 見失ったかどうか
    public void CheckTargetLost();

    // 特殊処理
    public void Ability();

    // 移動
    public void Move();

    // 情報の更新
    public void StatusUpdate();
}
