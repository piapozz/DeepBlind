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
    /// �s��
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public void Activity()
    {
        if (_enemy == null) return;
        // �擾
        GetTarget();

        // �����������ǂ���
        CheckTargetLost();
    }

    /// <summary>
    /// ������
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
    /// �����������ǂ���
    /// </summary>
    public void CheckTargetLost()
    {
        if(EnemyUtility.CheckViewPlayer(_ID, !_IsTargetlost))                                              // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            _IsTargetlost = false;
        }
        else
        {
            // �������Ă�����
            if (!_IsTargetlost) _lostTime = 0.0f;
            _IsTargetlost = true;
        }

        if (!_IsTargetlost) return;

        // ���������ȉ��Ȃ�x��
        // �b�������ȏ�ɂȂ�����x��
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
    /// �ڕW�ʒu�̎擾
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget()
    {
        _enemy.SetNavTarget(_player.transform.position);
    }
}
