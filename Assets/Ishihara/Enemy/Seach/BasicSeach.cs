using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static EnemyBase;

public class BasicSeach : ISeach
{
    EnemyInfo enemyInfo = new EnemyInfo();
    
    bool tracking;              // 見つけた

    // 行動
    public EnemyInfo Activity(EnemyInfo info)
    {
        // 取得
        GetTarget(info);

        // 見つけたかどうか
        ChekTracking();

        // 警戒条件を満たしたかどうか
        CheckVigilance();

        // 特殊処理
        Ability();

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
    public void ChekTracking()
    {
        // プレイヤーとの間に障害物があるかどうか
        Vector3 origin = enemyInfo.status.position;                                                   // 原点
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X軸方向を表すベクトル
        Ray ray = new Ray(origin, direction);                                                    // Rayを生成;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, enemyInfo.viewLength))                                                       // もしRayを投射して何らかのコライダーに衝突したら
        {
            string tag = hit.collider.gameObject.tag;                                            // 衝突した相手オブジェクトの名前を取得
            
            if(tag != "Player") return;                                                          // プレイヤー以外なら終わる

            float toPlayerAngle = Template(enemyInfo.status.position, enemyInfo.playerStatus.playerPos) + 180;   // プレイヤーへの角度
            float myAngle = Template(enemyInfo.status.dir) + 180;                                     // 向いてる角度

            // 視野範囲内なら
            if(myAngle + (enemyInfo.fieldOfView / 2) > toPlayerAngle &&
                myAngle - (enemyInfo.fieldOfView / 2) < toPlayerAngle)
            {
                // 見つけた
                tracking = true;
            }
        }
    }
    private float Template(Vector3 point1)
    {
        float temp;

        point1.Normalize();

        temp = Mathf.Atan2(point1.z , point1.x);

        return temp;
    }

    private float Template(Vector3 point1 , Vector3 point2)
    {
        float temp;

        point1.Normalize();
        point2.Normalize();

        temp = Mathf.Atan2(point1.z - point2.z , point1.x - point2.x);

        return temp; 
    }


    // 特殊処理(見られていたら止まる)
    public void Ability()
    {
        Plane[] planes;

        // カメラの視錐台を求める
        planes = GeometryUtility.CalculateFrustumPlanes(enemyInfo.playerStatus.cam);

        // カメラに写っているか判定
        if(GeometryUtility.TestPlanesAABB(planes , enemyInfo.bounds))
        {
            // 映っていたら制止する
            enemyInfo.status.targetPos = enemyInfo.status.position; // 目標位置を現在位置に
            enemyInfo.animator.speed = 0.0f;                        // アニメーションの再生を停止
        }
        else enemyInfo.animator.speed = 1.0f;   // 通常再生

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
        //// 同期
        //enemyInfo = info;

        //// ステータス更新
        //enemyInfo.status = enemyStatus;

        // ステートの切り替え
        if (tracking) enemyInfo.status.state = State.TRACKING;
    }

}
