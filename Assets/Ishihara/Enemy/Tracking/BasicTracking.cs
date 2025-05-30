using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicTracking : ITracking
{
    private int _ID = -1;               // ID
    private EnemyBase _enemy;           // 敵
    private Player _player;             // プレイヤー

    private bool _IsTargetlost = false; // 見失ったかどうか
    private float _lostTime = 0;        // 見失っている時間

    /// <summary>
    /// 行動
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public void Activity()
    {
        if (_enemy == null) return;
        // 取得
        GetTarget();
        // 見失ったかどうか
        CheckTargetLost();

        Debug.DrawLine(_enemy.transform.position, _enemy.target, Color.red);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(int setID)
    {
        _ID = setID;
        _enemy = EnemyUtility.GetCharacter(_ID);
        _player = EnemyUtility.GetPlayer();
        _IsTargetlost = false;
        _lostTime = 0;
}

    /// <summary>
    /// 見失ったかどうか
    /// </summary>
    public void CheckTargetLost()
    {
        // 視界に入っているかどうか
        if (EnemyUtility.CheckViewPlayer(_ID, _enemy.viewLength, !_IsTargetlost))
        {
            _IsTargetlost = false;
        }
        else
        {
            // 見失っていたら
            if (!_IsTargetlost) _lostTime = 0.0f;
            _IsTargetlost = true;
        }

        if (!_IsTargetlost) return;

        // 距離が一定以下なら警戒
        // 秒数が一定以上になったら警戒
        if(!_enemy.isAbility) _lostTime += Time.deltaTime;
        if(_lostTime > 3.0f)
        {
            _enemy.StateChange(State.VIGILANCE);
            return;
        }
        // 距離が一定以下なら警戒
        float length = EnemyUtility.EnemyToPlayerLength(_ID);
        if (length > _enemy.viewLength + 10)
        {
            _enemy.StateChange(State.VIGILANCE);
            return;
        }
    }

    /// <summary>
    /// 目標位置の取得
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget()
    {
        _enemy.SetNavTarget(_player.transform.position);
    }
}
