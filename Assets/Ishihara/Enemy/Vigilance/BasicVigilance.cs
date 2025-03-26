using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicVigilance : IVigilance
{
    private int _ID = -1;           // ID
    private EnemyBase _enemy;       // �G
    private Player _player;         // �v���C���[

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
        // �^�[�Q�b�g�擾
        GetTarget();

        Debug.DrawLine(_enemy.transform.position, _enemy.target, Color.yellow);
    }

    /// <summary>
    /// ������
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
        // ���͈͓������񂵂���I���
        Vector3 playerPos = _player.transform.position;
        Vector3 enemyPos = _enemy.transform.position;
        // �v���C���[���m�o�͈͂ɓ����Ă��邩
        if (EnemyUtility.CheckViewPlayer(_ID, _enemy.viewLength, false))
        {
            // ����p����
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

    /// <summary>
    /// �^�[�Q�b�g�擾
    /// </summary>
    public void GetTarget()
    {
        // �T�����[�g�̐ݒ�
        if (Vector3.Distance(_enemy.target, _enemy.transform.position) < 2.0f)
        {
            if (!EnemyUtility.CheckSearchAnchor(_ID))
            {
                // ���̃A���J�[���Ȃ��Ȃ�
                _enemy.StateChange(State.SEARCH);
                _enemy.SetSearchAnchor(StageManager.instance.GetRandomEnemyAnchor());

            }
        }
    }
}
