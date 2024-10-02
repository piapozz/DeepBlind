using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public interface ISeach : IEnemyState
{
    // 目標位置の取得
    public void GetTarget(EnemyInfo info);

    // 見つけたかどうか
    public void CheckTracking();

    // 警戒条件を満たしたかどうか
    public void CheckVigilance();

    // 情報の更新
    public void StatusUpdate(EnemyInfo info);
}
