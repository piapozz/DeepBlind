using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicVigilance : IVigilance
{
    // 更新する情報
    EnemyInfo enemyInfo;

    // 追跡中から警戒に移ったかで処理を変えるフラグ
    bool isViaSearch = false;

    int viaNum = 0;

    // 状態管理フラグ
    bool search = false;
    bool tracking = false;

    // 行動
    public EnemyInfo Activity(EnemyInfo info, ISkill skill)
    {
        // 取得
        GetTarget(info);

        // 完全に見失ったかどうか
        CheckLookAround();

        // 特殊処理
        enemyInfo = skill.Ability(enemyInfo);

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
        isViaSearch = false;
    }

    public void CheckLookAround()
    {
        if(!isViaSearch && !enemyInfo.status.prediction) return;

        enemyInfo.status.prediction = false;
        isViaSearch = true;

        // 目標地点をリスト順に格納
        enemyInfo.status.targetPos = enemyInfo.status.viaData[viaNum].viaPosition;

        // プレイヤーとの間に障害物があるかどうか
        Vector3 origin = enemyInfo.status.position;                                                   // 原点
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X軸方向を表すベクトル
        Ray ray = new Ray(origin, direction);                                                    // Rayを生成;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, enemyInfo.pram.viewLength + 1, 1))                                                       // もしRayを投射して何らかのコライダーに衝突したら
        {
            string tag = hit.collider.gameObject.tag;                                            // 衝突した相手オブジェクトの名前を取得

            // プレイヤーなら
            if (tag == "Player")
            {
                float toPlayerAngle = Mathf.Atan2(enemyInfo.playerStatus.playerPos.z - enemyInfo.status.position.z,
                                   enemyInfo.playerStatus.playerPos.x - enemyInfo.status.position.x) * Mathf.Rad2Deg;
                float myAngle = Mathf.Atan2(enemyInfo.status.dir.z, enemyInfo.status.dir.x) * Mathf.Rad2Deg;

                // 0 ~ 360にクランプ
                toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
                myAngle = Mathf.Repeat(myAngle, 360);

                // 視野範囲内なら
                if (myAngle + (enemyInfo.pram.fieldOfView / 2) > toPlayerAngle &&
                    myAngle - (enemyInfo.pram.fieldOfView / 2) < toPlayerAngle)
                {
                    // 見つけた
                    tracking = true;
                }
            }
        }

        // 部屋に到着したら見渡す
        if (Vector3.Distance(enemyInfo.status.position , enemyInfo.status.targetPos) > 3.0f) return;

        // 見渡す
        if(!LookAround()) return;

        // 次の巡回地点を設定
        viaNum++;

        Debug.Log("経由地点" + viaNum + "/" + enemyInfo.status.viaData.Count + "通過");

        // 警戒終了
        if(viaNum == enemyInfo.status.viaData.Count) search = true;
    }

    private bool LookAround()
    {
        if(!enemyInfo.status.viaData[viaNum].room) return true;

        // プレイヤーとの間に障害物があるかどうか
        Vector3 origin = enemyInfo.status.position;                                                   // 原点
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X軸方向を表すベクトル
        Ray ray = new Ray(origin, direction);                                                    // Rayを生成;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))                                                       // もしRayを投射して何らかのコライダーに衝突したら
        {
            string tag = hit.collider.gameObject.tag;                                            // 衝突した相手オブジェクトの名前を取得

            // プレイヤーなら
            if (tag == "Player")
            {
                // 見つけた
                tracking = true;
                return false;
            }
        }

        return true;
    }

    // 目標位置の取得
    public void GetTarget(EnemyInfo info)
    {
        // ターゲットの情報取得
        enemyInfo = info;
    }

    // 情報の更新
    public void StatusUpdate()
    {
        // ステートの切り替え
        if (search) enemyInfo.status.state = State.SEARCH;
        if (tracking) enemyInfo.status.state = State.TRACKING;
    }

    // 移動
    public void Move()
    {

    }
}
