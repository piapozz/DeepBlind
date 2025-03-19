using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicTracking : ITracking
{
    private int _ID = -1;               // ID
    private EnemyBase _enemy;           // “G
    private Player _player;             // ƒvƒŒƒCƒ„[

    private bool _IsTargetlost = false; // Œ©¸‚Á‚½‚©‚Ç‚¤‚©
    private float _lostTime = 0;        // Œ©¸‚Á‚Ä‚¢‚éŠÔ

    /// <summary>
    /// s“®
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public void Activity()
    {
        if (_enemy == null) return;
        // æ“¾
        GetTarget();
        // Œ©¸‚Á‚½‚©‚Ç‚¤‚©
        CheckTargetLost();

        Debug.DrawLine(_enemy.transform.position, _enemy.target, Color.red);
    }

    /// <summary>
    /// ‰Šú‰»
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
    /// Œ©¸‚Á‚½‚©‚Ç‚¤‚©
    /// </summary>
    public void CheckTargetLost()
    {
        // ‹ŠE‚É“ü‚Á‚Ä‚¢‚é‚©‚Ç‚¤‚©
        if (EnemyUtility.CheckViewPlayer(_ID, !_IsTargetlost))
        {
            _IsTargetlost = false;
        }
        else
        {
            // Œ©¸‚Á‚Ä‚¢‚½‚ç
            if (!_IsTargetlost) _lostTime = 0.0f;
            _IsTargetlost = true;
        }

        if (!_IsTargetlost) return;

        // ‹——£‚ªˆê’èˆÈ‰º‚È‚çŒx‰ú
        // •b”‚ªˆê’èˆÈã‚É‚È‚Á‚½‚çŒx‰ú
        if(!_enemy.isAbility) _lostTime += Time.deltaTime;
        if(_lostTime > 3.0f)
        {
            _enemy.StateChange(State.VIGILANCE);
            return;
        }
        // ‹——£‚ªˆê’èˆÈ‰º‚È‚çŒx‰ú
        float length = EnemyUtility.EnemyToPlayerLength(_ID);
        if (length > _enemy.viewLength + 10)
        {
            _enemy.StateChange(State.VIGILANCE);
            return;
        }
    }

    /// <summary>
    /// –Ú•WˆÊ’u‚Ìæ“¾
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget()
    {
        _enemy.SetNavTarget(_player.transform.position);
    }
}
