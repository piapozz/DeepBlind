using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;
using static UnityEngine.Rendering.HableCurve;
using static UnityEngine.UI.GridLayoutGroup;

public class BasicSeach : ISeach
{
    EnemyInfo enemyInfo = new EnemyInfo();
    
    bool tracking;              // 見つけた

    // 行動
    public EnemyInfo Activity(EnemyInfo info , ISkill skill)
    {
        // 取得
        GetTarget(info);

        // 見つけたかどうか
        CheckTracking();

        // 警戒条件を満たしたかどうか
        CheckVigilance();

        // 特殊処理
        enemyInfo = skill.Ability(enemyInfo);

        // 更新
        StatusUpdate(info);

        return enemyInfo;
    }

    // 初期化
    public void Init()
    {
        enemyInfo = new EnemyInfo();

        tracking = false;              // 見つけた
    }

    // 見つけたかどうか
    public void CheckTracking()
    {
        RaycastHit hit;

        // プレイヤーとの間に障害物があるかどうか
        Vector3 origin = enemyInfo.status.position;                                              // 原点
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X軸方向を表すベクトル
        Ray ray = new Ray(origin, direction);                                                    // Rayを生成;

        if (Physics.Raycast(ray, out hit, enemyInfo.pram.viewLength + 1 , 1)) 
        {
            string tag = hit.collider.gameObject.tag;                                            // 衝突した相手オブジェクトの名前を取得
            
            if(tag != "Player") return;                                                          // プレイヤー以外なら終わる

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

    // 警戒条件を満たしたかどうか
    public void CheckVigilance()
    {

    }

    // 目標位置の取得
    public void GetTarget(EnemyInfo info)
    {
        // ターゲットの情報取得
        enemyInfo = info; 
    }

    // 情報の更新
    public void StatusUpdate(EnemyInfo info)
    {
        // ステートの切り替え
        if (tracking) enemyInfo.status.state = State.TRACKING;
    }

}
