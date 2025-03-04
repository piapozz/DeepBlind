using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;
using static UnityEngine.Rendering.HableCurve;
using static UnityEngine.UI.GridLayoutGroup;

public class BasicSeach : ISeach
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

        // 見つけたかどうか
        CheckTracking();

        // 警戒条件を満たしたかどうか
        CheckVigilance();

        // 仮目標地点にたどり着いたかどうか
        CheckReaching();

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

    /// <summary>
    /// 見つけたかどうか
    /// </summary>
    public void CheckTracking()
    {
        RaycastHit hit;
        Vector3 playerPos = _player.transform.position;
        Vector3 enemyPos = _enemy.transform.position;

        // プレイヤーとの間に障害物があるかどうか
        Vector3 origin = _enemy.transform.position;                                              // 原点
        Vector3 direction = Vector3.Normalize(playerPos - enemyPos);     // X軸方向を表すベクトル
        Ray ray = new Ray(origin, direction);                                                    // Rayを生成;

        if (Physics.Raycast(ray, out hit, _enemy.viewLength + 1, 1))
        {
            string tag = hit.collider.gameObject.tag;                                            // 衝突した相手オブジェクトの名前を取得

            if (tag != "Player") return;                                                          // プレイヤー以外なら終わる
                                                                
            float toPlayerAngle = Mathf.Atan2(playerPos.z - enemyPos.z,
                                   playerPos.x - enemyPos.x) * Mathf.Rad2Deg;
            Vector3 dir = _enemy.transform.forward;
            float myAngle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

            // 0 ~ 360にクランプ
            toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
            myAngle = Mathf.Repeat(myAngle, 360);

            // 視野範囲内なら
            if (myAngle + (_enemy.fieldOfView / 2) > toPlayerAngle &&
                myAngle - (_enemy.fieldOfView / 2) < toPlayerAngle)
            {
                // 見つけた
                _enemy.StateChange(State.TRACKING);
            }
        }
    }

    // 仮目標地点にたどり着いたかどうか
    private void CheckReaching()
    {
        //if ((Vector3.Distance(_enemyInfo.status.targetPos, _enemy.transform.position) < 2.0f) && (!_enemyInfo.status.isAblity))
        //{
        //    // 探索箇所をランダムに設定する
        //    _enemyInfo.status.targetPos = EnemyManager.Instance.DispatchTargetPosition();
        //}
    }

    // 警戒条件を満たしたかどうか
    public void CheckVigilance()
    {

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
    /// <param name="info"></param>
    public void StatusUpdate()
    {

    }
}
