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
    /// �s��
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public void Activity()
    {
        if (_enemy == null) return;
        // ���S�Ɍ����������ǂ���
        CheckLookAround();

        GetTarget();
    }

    /// <summary>
    /// ������
    /// </summary>
    public void Init(int setID)
    {
        _ID = setID;
        _enemy = EnemyUtility.GetCharacter(_ID);
        _player = EnemyUtility.GetPlayer();
    }

    public void CheckLookAround()
    {
        // ���͈͓������񂵂���I���

        Vector3 playerPos = _player.transform.position;
        Vector3 enemyPos = _enemy.transform.position;

        if (EnemyUtility.CheckViewPlayer(_ID))
        {
            float toPlayerAngle = Mathf.Atan2(playerPos.z - enemyPos.z,
                                   playerPos.x - enemyPos.x) * Mathf.Rad2Deg;
            Vector3 dir = _enemy.transform.forward;
            float myAngle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

            // 0 ~ 360�ɃN�����v
            toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
            myAngle = Mathf.Repeat(myAngle, 360);

            // ����͈͓��Ȃ�
            if (myAngle + (_enemy.fieldOfView / 2) > toPlayerAngle &&
                myAngle - (_enemy.fieldOfView / 2) < toPlayerAngle)
            {
                // ������
                _enemy.StateChange(State.TRACKING);
            }
        }
    }

    public void GetTarget()
    {
        if (Vector3.Distance(_enemy.target, _enemy.transform.position) < 2.0f)
        {
            _enemy.StateChange(State.SEARCH);
        }
        else
        {
            _enemy.SetNavTarget(_enemy.target);
        }
    }
}
