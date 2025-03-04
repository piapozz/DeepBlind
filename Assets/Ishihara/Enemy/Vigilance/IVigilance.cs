using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public interface IVigilance : IEnemyState
{
    /// <summary>
    /// 目標位置の取得
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget();

    /// <summary>
    /// 完全に見失ったかどうか
    /// </summary>
    public void CheckLookAround();

    /// <summary>
    /// 情報の更新
    /// </summary>
    public void StatusUpdate();
}
