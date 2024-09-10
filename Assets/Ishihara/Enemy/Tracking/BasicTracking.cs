using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicTracking : ITracking
{
    // 情報
    EnemyInfo enemyInfo;

    // 探索フラグ
    bool seach = false;

    // 行動
    public EnemyInfo Activity(EnemyInfo info)
    {
        // 取得
        GetTarget(info);

        // 見失ったかどうか
        CheckTargetLost();

        // 特殊処理
        Ability();

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

        seach = false;              // 見つけた
    }

    // 見失ったかどうか
    public void CheckTargetLost()
    {
        // プレイヤーとの間に障害物があるかどうか
        Vector3 origin = enemyInfo.status.position;                                                              // 原点
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X軸方向を表すベクトル
        Ray ray = new Ray(origin, direction);                                                                    // Rayを生成;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, enemyInfo.viewLength))                                                 // もしRayを投射して何らかのコライダーに衝突したら
        {
            string tag = hit.collider.gameObject.tag;                                                            // 衝突した相手オブジェクトの名前を取得

            float toPlayerAngle = Template(enemyInfo.status.position, enemyInfo.playerStatus.playerPos) * 180 / Mathf.PI;   // プレイヤーへの角度
            float myAngle = Template(enemyInfo.status.dir);                                                // 向いてる角度

            Debug.Log(toPlayerAngle);

            // 視野範囲内なら
            if ((myAngle + (enemyInfo.fieldOfView / 2) > toPlayerAngle &&
                myAngle - (enemyInfo.fieldOfView / 2) < toPlayerAngle) &&
                tag == "Player")
            {
                // 追跡継続
                Debug.Log("見つけ");

                // 直前まで見失っていたなら
                if (enemyInfo.status.isTargetLost) enemyInfo.status.isTargetLost = false; // 再発見
            }
            // 初めて見失っていたら
            else if (enemyInfo.status.isTargetLost == false && tag != "Player")
            {
                // ロストポジションを設定
                enemyInfo.status.lostPos = enemyInfo.playerStatus.playerPos;
                enemyInfo.status.isTargetLost = true;

                Debug.Log("見失った");
            }
            // 最後の見失った地点に到達してなお見つけられなかったら
            else if (Vector3.Distance(enemyInfo.status.lostPos, enemyInfo.status.position) < 1.0f && tag != "Player")
            {
                seach = true;

                Debug.Log("探索に戻る");
            }
        }
    }
    private float Template(Vector3 point1)
    {
        float temp;

        temp = Mathf.Atan2(point1.z, point1.x);

        return temp;
    }

    private float Template(Vector3 point1, Vector3 point2)
    {
        float temp;

        temp = Mathf.Atan2(point1.z - point2.z, point1.x - point2.x);

        return temp;
    }

    // 目標位置の取得
    public void GetTarget(EnemyInfo info)
    {
        // ターゲットの情報取得
        enemyInfo = info;
    }

    // 特殊処理
    public void Ability()
    {
        //Plane[] planes;

        //// カメラの視錐台を求める
        //planes = GeometryUtility.CalculateFrustumPlanes(enemyInfo.playerStatus.cam);

        //// カメラに写っているか判定
        //if (GeometryUtility.TestPlanesAABB(planes, enemyInfo.bounds))
        //{
        //    // 映っていたら制止する
        //    enemyInfo.status.targetPos = enemyInfo.status.position; // 目標位置を現在位置に
        //    enemyInfo.animator.speed = 0.0f;                        // アニメーションの再生を停止
        //}
        //else enemyInfo.animator.speed = 1.0f;   // 通常再生

        enemyInfo.animator.speed = 2.0f;
    }

    // 情報の更新
    public void StatusUpdate()
    {
        // ステートの切り替え
        if (seach) enemyInfo.status.state = State.SEACH;
    }

    // 移動
    public void Move()
    {
        // 目標位置を設定
        if(enemyInfo.status.isTargetLost)　enemyInfo.status.targetPos = enemyInfo.status.lostPos; // 見失っていたら
        else enemyInfo.status.targetPos = enemyInfo.playerStatus.playerPos;                       // 追跡中

    }
}
