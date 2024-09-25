using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static EnemyBase;

public class BasicVigilance : IVigilance
{
    // 更新する情報
    EnemyInfo enemyInfo;

    // 状態管理フラグ
    bool search = false;
    bool tracking = false;

    // 行動
    public EnemyInfo Activity(EnemyInfo info)
    {
        // 取得
        GetTarget(info);

        // 完全に見失ったかどうか
        CheckLookAround();

        // 特殊処理
        //Ability();

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

    // 周りを見渡す
    public void CheckLookAround()
    {
        Debug.DrawLine(enemyInfo.status.position, enemyInfo.status.targetPos, Color.blue, 0.01f);

        // プレイヤーとの間に障害物があるかどうか
        Vector3 origin = enemyInfo.status.position;                                                   // 原点
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X軸方向を表すベクトル
        Ray ray = new Ray(origin, direction);                                                    // Rayを生成;

        RaycastHit hit;

        for (int i = 0; i < 100; i++)
        {
            float myAngle1 = Template(enemyInfo.status.dir);

            Debug.DrawLine(ray.origin,
                ray.origin + (
                Quaternion.Euler(
                    new Vector3(0, Mathf.Repeat(myAngle1, 360) - (enemyInfo.fieldOfView / 2) + ((enemyInfo.fieldOfView / 100) * i), 0)) * enemyInfo.status.dir * enemyInfo.viewLength),
                Color.gray,
                0.01f);
        }

        if (Physics.Raycast(ray, out hit, enemyInfo.viewLength + 1, 1))                                                       // もしRayを投射して何らかのコライダーに衝突したら
        {
            Debug.DrawLine(ray.origin, enemyInfo.status.targetPos, Color.red, 0.01f);
            // Debug.DrawLine(ray.origin, ray.origin + (enemyInfo.status.dir * enemyInfo.viewLength), Color.blue, 0.01f);

            string tag = hit.collider.gameObject.tag;                                            // 衝突した相手オブジェクトの名前を取得

            // プレイヤーなら
            if (tag == "Player")
            {

                float toPlayerAngle = Template(enemyInfo.status.position, enemyInfo.playerStatus.playerPos);   // プレイヤーへの角度
                float myAngle = Template(enemyInfo.status.dir);                                     // 向いてる角度

                // 0 ~ 360にクランプ
                toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
                myAngle = Mathf.Repeat(myAngle, 360);

                // 視野範囲内なら
                if (myAngle + (enemyInfo.fieldOfView / 2) > toPlayerAngle &&
                    myAngle - (enemyInfo.fieldOfView / 2) < toPlayerAngle)
                {
                    Debug.Log("発見");
                    // 見つけた
                    tracking = true;
                }
            }
        }

        // 部屋に到着した
        if (Vector3.Distance(enemyInfo.status.position , enemyInfo.status.targetPos) > 3.0f) return;

        //// プレイヤーとの間に障害物があるかどうか
        //Vector3 origin = enemyInfo.status.position;                                                              // 原点
        //Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X軸方向を表すベクトル
        //Ray ray = new Ray(origin, direction);                                                                    // Rayを生成;

        //RaycastHit hit;
        if (Physics.Raycast(ray, out hit, enemyInfo.viewLength))                                                 // もしRayを投射して何らかのコライダーに衝突したら
        {
            string tag = hit.collider.gameObject.tag;                                                            // 衝突した相手オブジェクトの名前を取得

            // 視野範囲内なら
            if (tag == "Player")
            {
                // 追跡継続
                Debug.Log("見つけた");

                // プレイヤーがいたらTRACKING状態に切り替える
                tracking = true;
            }
            // 初めて見失っていたら
            else if (tag != "Player")
            {
                Debug.Log("見つからない");

                // プレイヤーがいなかったらSEARCH状態に切り替え
                search = true;
            }
        }
        else
        {
            Debug.Log("見つからない");

            // プレイヤーがいなかったらSEARCH状態に切り替え
            search = true;
        }

    }

    // 角度計算
    private float Template(Vector3 point1)
    {
        float temp;

        temp = Mathf.Atan2(point1.z, point1.x);

        return temp;
    }

    // 角度計算
    private float Template(Vector3 point1, Vector3 point2)
    {
        float temp;

        temp = Mathf.Atan2(point1.z - point2.z, point1.x - point2.x);

        return temp;
    }

    // 特殊処理(見られていたら止まる)
    public void Ability()
    {
        Vector3[] targetPoints = new Vector3[8];

        targetPoints[0] = enemyInfo.bounds.min;
        targetPoints[1] = new Vector3(enemyInfo.bounds.max.x, enemyInfo.bounds.min.y, enemyInfo.bounds.min.z);
        targetPoints[2] = new Vector3(enemyInfo.bounds.min.x, enemyInfo.bounds.max.y, enemyInfo.bounds.min.z);
        targetPoints[3] = new Vector3(enemyInfo.bounds.min.x, enemyInfo.bounds.min.y, enemyInfo.bounds.max.z);
        targetPoints[4] = new Vector3(enemyInfo.bounds.max.x, enemyInfo.bounds.max.y, enemyInfo.bounds.min.z);
        targetPoints[5] = new Vector3(enemyInfo.bounds.max.x, enemyInfo.bounds.min.y, enemyInfo.bounds.max.z);
        targetPoints[6] = new Vector3(enemyInfo.bounds.min.x, enemyInfo.bounds.max.y, enemyInfo.bounds.max.z);
        targetPoints[7] = enemyInfo.bounds.max;

        // 各コーナーがカメラのビューポートに収まっているかをチェック

        //　カメラ内にオブジェクトがあるかどうか
        bool isInsideCamera = false;
        //　ターゲットポイントがカメラのビューポート内にあるかどうかを調べる
        foreach (var targetPoint in targetPoints)
        {

            Plane[] planes;

            // カメラの視錐台を求める
            planes = GeometryUtility.CalculateFrustumPlanes(enemyInfo.playerStatus.cam);

            // カメラに写っているか判定
            if (GeometryUtility.TestPlanesAABB(planes, enemyInfo.bounds))
            {
                // コーナーからカメラ位置へのレイキャスト
                Vector3 direction = -(targetPoint - enemyInfo.playerStatus.cam.transform.position);
                Ray ray = new Ray(targetPoint, direction.normalized);
                RaycastHit hit;
                // レイキャストがプレイヤーに直接当たるか確認
                if (Physics.SphereCast(ray, 0.1f, out hit, direction.magnitude + 1))
                {
                    // レイを描画する
                   // Debug.DrawLine(ray.origin, ray.origin + ray.direction * (direction.magnitude + 1), Color.green, 0.01f);

                    // 障害物がなく直接当たった場合に true を返す
                    if (hit.collider.CompareTag("Player"))
                    {
                        isInsideCamera = true;
                    }
                }
            }
        }

        if (isInsideCamera)
        {
            //映っていたら制止する
            enemyInfo.status.nowSpeed = 0.0f; // 目標位置を現在位置に
            enemyInfo.animator.speed = 0.0f;                        // アニメーションの再生を停止
            enemyInfo.status.nowAccelerate = 0.0f;
            enemyInfo.status.isAblity = true;
        }
        else
        {
            enemyInfo.status.nowSpeed = enemyInfo.speed;
            enemyInfo.status.nowAccelerate = enemyInfo.accelerate;
            enemyInfo.animator.speed = enemyInfo.animSpeed; ;   // 通常再生
            enemyInfo.status.isAblity = false;
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
        if (search) enemyInfo.status.state = State.SEARCH;
        if (tracking) enemyInfo.status.state = State.VIGILANCE;
    }

    // 移動
    public void Move()
    {

    }
}
