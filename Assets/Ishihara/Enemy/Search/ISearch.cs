using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public interface ISearch : IEnemyState
{
    // 目標位置の取得
    public void GetTarget();

    // 見つけたかどうか
    public void CheckTracking();

    // 警戒条件を満たしたかどうか
    public void CheckVigilance();
}
