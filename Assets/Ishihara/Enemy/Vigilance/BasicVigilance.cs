using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicVigilance : IVigilance
{
    private int _ID = -1;           // ID
    private EnemyBase _enemy;       // 敵
    private Player _player;         // プレイヤー

    /// <summary>
    /// 行動
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public void Activity()
    {
        if (_enemy == null) return;
        // 完全に見失ったかどうか
        CheckLookAround();
        // ターゲット取得
        GetTarget();

        Debug.DrawLine(_enemy.transform.position, _enemy.target, Color.yellow);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(int setID)
    {
        _ID = setID;
        _enemy = EnemyUtility.GetCharacter(_ID);
        _player = EnemyUtility.GetPlayer();
        _enemy.SetSearchAnchor(StageManager.instance.GetEnemyAnchor(_enemy.target));
    }

    public void CheckLookAround()
    {
        // 一定範囲内を巡回したら終わる
        Vector3 playerPos = _player.transform.position;
        Vector3 enemyPos = _enemy.transform.position;
        // プレイヤーが知覚範囲に入っているか
        if (EnemyUtility.CheckViewPlayer(_ID, _enemy.viewLength, false))
        {
            // 視野角判定
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

    /// <summary>
    /// ターゲット取得
    /// </summary>
    public void GetTarget()
    {
        // 探索ルートの設定
        if (Vector3.Distance(_enemy.target, _enemy.transform.position) < 2.0f)
        {
            if (!EnemyUtility.CheckSearchAnchor(_ID))
            {
                // 次のアンカーがないなら
                _enemy.StateChange(State.SEARCH);
                _enemy.SetSearchAnchor(StageManager.instance.GetRandomEnemyAnchor());

            }
        }
    }
}
