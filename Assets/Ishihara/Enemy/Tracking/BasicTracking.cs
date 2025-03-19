using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicTracking : ITracking
{
    private int _ID = -1;               // ID
    private EnemyBase _enemy;           // �G
    private Player _player;             // �v���C���[

    private bool _IsTargetlost = false; // �����������ǂ���
    private float _lostTime = 0;        // �������Ă��鎞��

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

        Debug.DrawLine(_enemy.transform.position, _enemy.target, Color.red);
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
        // ���E�ɓ����Ă��邩�ǂ���
        if (EnemyUtility.CheckViewPlayer(_ID, !_IsTargetlost))
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
        if(_lostTime > 3.0f)
        {
            _enemy.StateChange(State.VIGILANCE);
            return;
        }
        // ���������ȉ��Ȃ�x��
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
