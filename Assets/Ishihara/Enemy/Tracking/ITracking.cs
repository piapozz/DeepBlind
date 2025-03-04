using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITracking : IEnemyState
{
    /// <summary>
    /// 目標位置の取得
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget();

    /// <summary>
    /// 見失ったかどうか
    /// </summary>
    public void CheckTargetLost();

    /// <summary>
    /// 情報の更新
    /// </summary>
    public void StatusUpdate();
}
