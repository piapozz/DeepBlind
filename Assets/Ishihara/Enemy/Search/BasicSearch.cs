using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;
using static UnityEngine.Rendering.HableCurve;
using static UnityEngine.UI.GridLayoutGroup;

public class BasicSearch : ISearch
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
        if(_enemy == null) return;
        // 取得
        GetTarget();
        // 見つけたかどうか
        CheckTracking();
        // 警戒条件を満たしたかどうか
        CheckVigilance();

        Debug.DrawLine(_enemy.transform.position, _enemy.target, Color.green);
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
        Vector3 playerPos = _player.transform.position;
        Vector3 enemyPos = _enemy.transform.position;

        // プレイヤーが知覚範囲に入っているか
        if (EnemyUtility.CheckViewPlayer(_ID, false))
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

    // 警戒条件を満たしたかどうか
    public void CheckVigilance()
    {
        Vector3 position = SoundObjectManager.GetBigSoundPosition(_enemy.transform.position, 0.1f);
        if (position == Vector3.zero) return;

        // 警戒状態
        _enemy.StateChange(State.VIGILANCE);
        _enemy.SetNavTarget(position);
    }

    /// <summary>
    /// 目標位置の取得
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget()
    {
        if (Vector3.Distance(_enemy.target, _enemy.transform.position) < 2.0f)
        {
            // 探索箇所を設定する
            if (!EnemyUtility.CheckSearchAnchor(_ID))
            {
                // 次のアンカーがないなら
                EnemyUtility.SetSearchAnchor(_ID);
            }
        }
    }
}
