using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicVigilance : IVigilance
{
    private int _ID = -1;
    private EnemyBase _enemy;
    private Player _player;

    /// <summary>
    /// s“®
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public void Activity()
    {
        if (_enemy == null) return;
        // Š®‘S‚ÉŒ©¸‚Á‚½‚©‚Ç‚¤‚©
        CheckLookAround();
    }

    /// <summary>
    /// ‰Šú‰»
    /// </summary>
    public void Init(int setID)
    {
        _ID = setID;
        _enemy = EnemyUtility.GetCharacter(_ID);
        _player = EnemyUtility.GetPlayer();
    }

    public void CheckLookAround()
    {
        // ˆê’è”ÍˆÍ“à‚ğ„‰ñ‚µ‚½‚çI‚í‚é

        _enemy.StateChange(State.SEARCH);
    }

    public void GetTarget()
    {
        
    }
}
