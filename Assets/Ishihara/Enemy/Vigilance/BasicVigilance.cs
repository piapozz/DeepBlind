using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BasicVigilance : IVigilance
{
    private int _ID = -1;
    private EnemyBase _enemy;
    private Player _player;

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

        // 完全に見失ったかどうか
        CheckLookAround();

        // 更新
        StatusUpdate();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(int setID)
    {
        _ID = setID;
        _enemy = EnemyUtility.GetCharacter(_ID);
        _player = EnemyUtility.GetPlayer();
    }

    public void CheckLookAround()
    {
        //// 目標地点をリスト順に格納

        //// プレイヤーとの間に障害物があるかどうか
        //Vector3 origin = _enemyInfo.status.position;                                                   // 原点
        //Vector3 direction = Vector3.Normalize(_enemyInfo.playerStatus.playerPos - _enemyInfo.status.position);     // X軸方向を表すベクトル
        //Ray ray = new Ray(origin, direction);                                                    // Rayを生成;

        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit, _enemyInfo.pram.viewLength + 1, 1))                                                       // もしRayを投射して何らかのコライダーに衝突したら
        //{
        //    string tag = hit.collider.gameObject.tag;                                            // 衝突した相手オブジェクトの名前を取得

        //    // プレイヤーなら
        //    if (tag == "Player")
        //    {
        //        float toPlayerAngle = Mathf.Atan2(_enemyInfo.playerStatus.playerPos.z - _enemyInfo.status.position.z,
        //                           _enemyInfo.playerStatus.playerPos.x - _enemyInfo.status.position.x) * Mathf.Rad2Deg;
        //        float myAngle = Mathf.Atan2(_enemyInfo.status.dir.z, _enemyInfo.status.dir.x) * Mathf.Rad2Deg;

        //        // 0 ~ 360にクランプ
        //        toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
        //        myAngle = Mathf.Repeat(myAngle, 360);

        //        // 視野範囲内なら
        //        if (myAngle + (_enemyInfo.pram.fieldOfView / 2) > toPlayerAngle &&
        //            myAngle - (_enemyInfo.pram.fieldOfView / 2) < toPlayerAngle)
        //        {
        //            // 見つけた
        //            _tracking = true;
        //        }
        //    }
        //}

        //// 部屋に到着したら見渡す
        //if (Vector3.Distance(_enemyInfo.status.position, _enemyInfo.status.targetPos) > 3.0f) return;

        //// 見渡す
        //if (!LookAround()) return;

        //// 次の巡回地点を設定

        //// 警戒終了
    }

    private bool LookAround()
    {
        //// プレイヤーとの間に障害物があるかどうか
        //Vector3 origin = _enemyInfo.status.position;                                                   // 原点
        //Vector3 direction = Vector3.Normalize(_enemyInfo.playerStatus.playerPos - _enemyInfo.status.position);     // X軸方向を表すベクトル
        //Ray ray = new Ray(origin, direction);                                                    // Rayを生成;

        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))                                                       // もしRayを投射して何らかのコライダーに衝突したら
        //{
        //    string tag = hit.collider.gameObject.tag;                                            // 衝突した相手オブジェクトの名前を取得

        //    // プレイヤーなら
        //    if (tag == "Player")
        //    {
        //        // 見つけた
        //        _tracking = true;
        //        return false;
        //    }
        //}

        return true;
    }

    // 目標位置の取得
    public void GetTarget()
    {
       
    }

    // 情報の更新
    public void StatusUpdate()
    {

    }
}
