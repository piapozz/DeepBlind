using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicVigilance : IVigilance
{
    // 更新する情報
    EnemyInfo enemyInfo;

    // 行動
    public EnemyInfo Activity(EnemyInfo info)
    {
        // 取得
        GetTarget();

        // 完全に見失ったかどうか
        CheckTargetLost();

        // 見つけたかどうか
        ChekTracking();

        // 特殊処理
        Ability();

        // 更新
        StatusUpdate();

        // 移動
        Move();

        return enemyInfo;
    }

    // 初期化
    public void Init()
    {
        enemyInfo = new EnemyInfo();
    }

    // 完全に見失ったかどうか
    public void CheckTargetLost()
    {

    }

    // 見つけたかどうか
    public void ChekTracking()
    {

    }

    // 特殊処理
    public void Ability()
    {

    }

    // 目標位置の取得
    public void GetTarget()
    {

    }

    // 情報の更新
    public void StatusUpdate()
    {

    }

    // 移動
    public void Move()
    {

    }
}
