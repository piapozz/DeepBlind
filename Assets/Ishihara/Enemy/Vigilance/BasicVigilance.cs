using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicVigilance : IVigilance
{
    /// 更新する情報
    private EnemyInfo _enemyInfo;

    // 追跡中から警戒に移ったかで処理を変えるフラグ
    private bool _isViaSearch = false;

    // 状態管理フラグ
    private bool _search = false;
    private bool _tracking = false;

    /// <summary>
    /// 行動
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public EnemyInfo Activity(EnemyInfo info, ISkill skill)
    {
        // 取得
        GetTarget(info);

        // 完全に見失ったかどうか
        CheckLookAround();

        // 特殊処理
        _enemyInfo = skill.Ability(_enemyInfo);

        // 更新
        StatusUpdate();

        // 移動
        Move();

        return _enemyInfo;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        _enemyInfo = new EnemyInfo();
        _isViaSearch = false;
    }

    public void CheckLookAround()
    {
        if(!_isViaSearch && !_enemyInfo.status.prediction) return;

        _enemyInfo.status.prediction = false;
        _isViaSearch = true;

        // 目標地点をリスト順に格納
        _enemyInfo.status.targetPos = _enemyInfo.status.viaData[viaNum].viaPosition;

        // プレイヤーとの間に障害物があるかどうか
        Vector3 origin = _enemyInfo.status.position;                                                   // 原点
        Vector3 direction = Vector3.Normalize(_enemyInfo.playerStatus.playerPos - _enemyInfo.status.position);     // X軸方向を表すベクトル
        Ray ray = new Ray(origin, direction);                                                    // Rayを生成;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, _enemyInfo.pram.viewLength + 1, 1))                                                       // もしRayを投射して何らかのコライダーに衝突したら
        {
            string tag = hit.collider.gameObject.tag;                                            // 衝突した相手オブジェクトの名前を取得

            // プレイヤーなら
            if (tag == "Player")
            {
                float toPlayerAngle = Mathf.Atan2(_enemyInfo.playerStatus.playerPos.z - _enemyInfo.status.position.z,
                                   _enemyInfo.playerStatus.playerPos.x - _enemyInfo.status.position.x) * Mathf.Rad2Deg;
                float myAngle = Mathf.Atan2(_enemyInfo.status.dir.z, _enemyInfo.status.dir.x) * Mathf.Rad2Deg;

                // 0 ~ 360にクランプ
                toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
                myAngle = Mathf.Repeat(myAngle, 360);

                // 視野範囲内なら
                if (myAngle + (_enemyInfo.pram.fieldOfView / 2) > toPlayerAngle &&
                    myAngle - (_enemyInfo.pram.fieldOfView / 2) < toPlayerAngle)
                {
                    // 見つけた
                    _tracking = true;
                }
            }
        }

        // 部屋に到着したら見渡す
        if (Vector3.Distance(_enemyInfo.status.position , _enemyInfo.status.targetPos) > 3.0f) return;

        // 見渡す
        if(!LookAround()) return;

        // 次の巡回地点を設定
        viaNum++;

        Debug.Log("経由地点" + viaNum + "/" + _enemyInfo.status.viaData.Count + "通過");

        // 警戒終了
        if(viaNum == _enemyInfo.status.viaData.Count) _search = true;
    }

    private bool LookAround()
    {
        if(!_enemyInfo.status.viaData[viaNum].room) return true;

        // プレイヤーとの間に障害物があるかどうか
        Vector3 origin = _enemyInfo.status.position;                                                   // 原点
        Vector3 direction = Vector3.Normalize(_enemyInfo.playerStatus.playerPos - _enemyInfo.status.position);     // X軸方向を表すベクトル
        Ray ray = new Ray(origin, direction);                                                    // Rayを生成;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))                                                       // もしRayを投射して何らかのコライダーに衝突したら
        {
            string tag = hit.collider.gameObject.tag;                                            // 衝突した相手オブジェクトの名前を取得

            // プレイヤーなら
            if (tag == "Player")
            {
                // 見つけた
                _tracking = true;
                return false;
            }
        }

        return true;
    }

    // 目標位置の取得
    public void GetTarget(EnemyInfo info)
    {
        // ターゲットの情報取得
        _enemyInfo = info;
    }

    // 情報の更新
    public void StatusUpdate()
    {
        // ステートの切り替え
        if (_search) _enemyInfo.status.state = State.SEARCH;
        if (_tracking) _enemyInfo.status.state = State.TRACKING;
    }

    // 移動
    public void Move()
    {

    }
}
