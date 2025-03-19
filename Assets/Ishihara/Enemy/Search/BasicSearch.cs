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
    /// �s��
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public void Activity()
    {
        if(_enemy == null) return;
        // �擾
        GetTarget();
        // ���������ǂ���
        CheckTracking();
        // �x�������𖞂��������ǂ���
        CheckVigilance();

        Debug.DrawLine(_enemy.transform.position, _enemy.target, Color.green);
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

    /// <summary>
    /// ���������ǂ���
    /// </summary>
    public void CheckTracking()
    {
        Vector3 playerPos = _player.transform.position;
        Vector3 enemyPos = _enemy.transform.position;

        // �v���C���[���m�o�͈͂ɓ����Ă��邩
        if (EnemyUtility.CheckViewPlayer(_ID, false))
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

    // �x�������𖞂��������ǂ���
    public void CheckVigilance()
    {
        Vector3 position = SoundObjectManager.GetBigSoundPosition(_enemy.transform.position, 0.1f);
        if (position == Vector3.zero) return;

        // �x�����
        _enemy.StateChange(State.VIGILANCE);
        _enemy.SetNavTarget(position);
    }

    /// <summary>
    /// �ڕW�ʒu�̎擾
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget()
    {
        if (Vector3.Distance(_enemy.target, _enemy.transform.position) < 2.0f)
        {
            // �T���ӏ���ݒ肷��
            if (!EnemyUtility.CheckSearchAnchor(_ID))
            {
                // ���̃A���J�[���Ȃ��Ȃ�
                EnemyUtility.SetSearchAnchor(_ID);
            }
        }
    }
}
