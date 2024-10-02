using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public interface IVigilance : IEnemyState
{
    // 目標位置の取得
    public void GetTarget(EnemyInfo info);

    // 完全に見失ったかどうか
    public void CheckLookAround();

    // 移動
    public void Move();

    // 情報の更新
    public void StatusUpdate();
}
