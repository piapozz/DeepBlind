using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicTracking : ITracking
{
    // 情報
    EnemyInfo enemyInfo;

    // 警戒フラグ
    bool vigilance = false;

    // 行動
    public EnemyInfo Activity(EnemyInfo info, ISkill skill)
    {
        // 取得
        GetTarget(info);

        // 見失ったかどうか
        CheckTargetLost();

        // 特殊処理
        enemyInfo = skill.Ability(enemyInfo);

        // 移動
        Move();

        // 更新
        StatusUpdate();

        return enemyInfo;
    }

    // 初期化
    public void Init()
    {
        enemyInfo = new EnemyInfo();

        // 警戒フラグ
        vigilance = false;
    }

    // 見失ったかどうか
    public void CheckTargetLost()
    {
        // プレイヤーとの間に障害物があるかどうか
        Vector3 origin = enemyInfo.status.position;                                                              // 原点
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X軸方向を表すベクトル
        Ray ray = new Ray(origin, direction);                                                                    // Rayを生成;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))                                                 // もしRayを投射して何らかのコライダーに衝突したら
        {
            string tag = hit.collider.gameObject.tag;                                                            // 衝突した相手オブジェクトの名前を取得

            // 初めて見失っていたら
            if (tag != "Player")
            {
                // ロストポジションを設定
                enemyInfo.status.lostPos = enemyInfo.playerStatus.playerPos;

                // プレイヤーの移動量を保存
                enemyInfo.status.lostMoveVec = enemyInfo.playerStatus.moveValue;

                vigilance = true;

                // 推測する
                enemyInfo.status.prediction = true;
                enemyInfo.status.isTargetLost = false;
            }
        }
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
        if (vigilance) enemyInfo.status.state = State.VIGILANCE;
    }

    // 移動
    public void Move()
    {
        // 目標位置を設定
        if(enemyInfo.status.isTargetLost)　enemyInfo.status.targetPos = enemyInfo.status.lostPos; // 見失っていたら
        else enemyInfo.status.targetPos = enemyInfo.playerStatus.playerPos;                       // 追跡中

    }
}
