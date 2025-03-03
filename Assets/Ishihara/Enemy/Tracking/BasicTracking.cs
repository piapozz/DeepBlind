using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicTracking : ITracking
{
    // 情報
    //private EnemyInfo _enemyInfo;

    // 警戒フラグ
    private bool _vigilance = false;

    /// <summary>
    /// 行動
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public void Activity()
    {
        // 取得
        GetTarget();

        // 見失ったかどうか
        CheckTargetLost();

        // 特殊処理
        //_enemyInfo = skill.Ability(_enemyInfo);

        // 移動
        Move();

        // 更新
        StatusUpdate();

        //return _enemyInfo;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        //_enemyInfo = new EnemyInfo();

        // 警戒フラグ
        _vigilance = false;
    }

    /// <summary>
    /// 見失ったかどうか
    /// </summary>
    public void CheckTargetLost()
    {
        //// プレイヤーとの間に障害物があるかどうか
        //Vector3 origin = _enemyInfo.status.position;                                                              // 原点
        //Vector3 direction = Vector3.Normalize(_enemyInfo.playerStatus.playerPos - _enemyInfo.status.position);     // X軸方向を表すベクトル
        //Ray ray = new Ray(origin, direction);                                                                    // Rayを生成;

        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))                                                 // もしRayを投射して何らかのコライダーに衝突したら
        //{
        //    string tag = hit.collider.gameObject.tag;                                                            // 衝突した相手オブジェクトの名前を取得

        //    // 初めて見失っていたら
        //    if (tag != "Player")
        //    {
        //        _vigilance = true;
        //    }
        //}
    }

    /// <summary>
    /// 目標位置の取得
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget()
    {

    }

    /// <summary>
    /// 情報の更新
    /// </summary>
    public void StatusUpdate()
    {

    }

    /// <summary>
    /// 移動
    /// </summary>
    public void Move()
    {


    }
}
