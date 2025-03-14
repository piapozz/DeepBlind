using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicTracking : ITracking
{
    private int _ID = -1;
    private EnemyBase _enemy;
    private Player _player;

    private bool _IsTargetlost = false;
    private float _lostTime = 0;

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
        if(EnemyUtility.CheckViewPlayer(_ID, !_IsTargetlost))                                              // もしRayを投射して何らかのコライダーに衝突したら
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
        if(_lostTime > 10.0f)
        {
            _enemy.StateChange(State.VIGILANCE);
            return;
        }

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
