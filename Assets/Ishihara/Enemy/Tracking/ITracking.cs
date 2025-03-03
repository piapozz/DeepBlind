using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITracking : IEnemyState
{
    /// <summary>
    /// �ڕW�ʒu�̎擾
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget(EnemyBase.EnemyInfo info);

    /// <summary>
    /// �����������ǂ���
    /// </summary>
    public void CheckTargetLost();

    /// <summary>
    /// �ړ�
    /// </summary>
    public void Move();

    /// <summary>
    /// ���̍X�V
    /// </summary>
    public void StatusUpdate();
}
