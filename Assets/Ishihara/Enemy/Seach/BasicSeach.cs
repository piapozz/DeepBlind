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
    /// s“®
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public void Activity()
    {
        if(_enemy == null) return;
        // æ“¾
        GetTarget();

        // Œ©‚Â‚¯‚½‚©‚Ç‚¤‚©
        CheckTracking();

        // Œx‰úğŒ‚ğ–‚½‚µ‚½‚©‚Ç‚¤‚©
        CheckVigilance();
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

    /// <summary>
    /// Œ©‚Â‚¯‚½‚©‚Ç‚¤‚©
    /// </summary>
    public void CheckTracking()
    {
        Vector3 playerPos = _player.transform.position;
        Vector3 enemyPos = _enemy.transform.position;

        if (EnemyUtility.CheckViewPlayer(_ID))
        {                          
            float toPlayerAngle = Mathf.Atan2(playerPos.z - enemyPos.z,
                                   playerPos.x - enemyPos.x) * Mathf.Rad2Deg;
            Vector3 dir = _enemy.transform.forward;
            float myAngle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

            // 0 ~ 360‚ÉƒNƒ‰ƒ“ƒv
            toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
            myAngle = Mathf.Repeat(myAngle, 360);

            // ‹–ì”ÍˆÍ“à‚È‚ç
            if (myAngle + (_enemy.fieldOfView / 2) > toPlayerAngle &&
                myAngle - (_enemy.fieldOfView / 2) < toPlayerAngle)
            {
                // Œ©‚Â‚¯‚½
                _enemy.StateChange(State.TRACKING);
            }
        }
    }

    // Œx‰úğŒ‚ğ–‚½‚µ‚½‚©‚Ç‚¤‚©
    public void CheckVigilance()
    {
        Vector3 position = SoundObjectManager.instance.GetBigSoundPosition(_enemy.transform.position, 1);
        if (position == Vector3.zero) return;

        // Œx‰úó‘Ô
        _enemy.StateChange(State.VIGILANCE);
        _enemy.SetNavTarget(position);
    }

    /// <summary>
    /// –Ú•WˆÊ’u‚Ìæ“¾
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget()
    {
        if (Vector3.Distance(_enemy.target, _enemy.transform.position) < 2.0f)
        {
            // ’Tõ‰ÓŠ‚ğƒ‰ƒ“ƒ_ƒ€‚Éİ’è‚·‚é
            _enemy.SetNavTarget(EnemyManager.instance.DispatchTargetPosition());
        }
        else
        {
            _enemy.SetNavTarget(_enemy.target);
        }
    }
}
