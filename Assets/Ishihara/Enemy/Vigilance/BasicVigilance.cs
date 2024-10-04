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

    // 時間計測
    float time = 0;

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
    }


    public void CheckLookAround()
    {
        Debug.DrawLine(enemyInfo.status.position, Vector3.up * 100 , Color.yellow);

        if(!isViaSearch && !enemyInfo.status.prediction) return;

        enemyInfo.status.prediction = false;
        isViaSearch = true;

        // 目標地点をリスト順に格納
        enemyInfo.status.targetPos = enemyInfo.status.viaData[viaNum].viaPosition;

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
                    // 見つけた
                    tracking = true;
                }
            }
        }

        // 部屋に到着したら見渡す
        if (Vector3.Distance(enemyInfo.status.position , enemyInfo.status.targetPos) > 3.0f) return;

        // 見渡す
        if(!LookAround() || !enemyInfo.status.viaData[viaNum].room) return;

        // 次の巡回地点を設定
        viaNum++;

        // 警戒終了
        if(viaNum == enemyInfo.status.viaData.Count) search = true;
    }

    private bool LookAround()
    {
        // 時間を計測
        time += Time.deltaTime;

        // 時間に従って部屋を探索


        // 回転する


        // 一周したら次の探索場所へ

        Debug.DrawLine(enemyInfo.status.position, enemyInfo.status.targetPos, Color.blue, 0.01f);

        // プレイヤーとの間に障害物があるかどうか
        Vector3 origin = enemyInfo.status.position;                                                   // 原点
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X軸方向を表すベクトル
        Ray ray = new Ray(origin, direction);                                                    // Rayを生成;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))                                                       // もしRayを投射して何らかのコライダーに衝突したら
        {
            Debug.DrawLine(ray.origin, enemyInfo.status.targetPos, Color.red, 0.01f);

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
