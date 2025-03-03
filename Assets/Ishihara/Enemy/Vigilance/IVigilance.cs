using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public interface IVigilance : IEnemyState
{
    /// <summary>
    /// �ڕW�ʒu�̎擾
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget(EnemyInfo info);

    /// <summary>
    /// ���S�Ɍ����������ǂ���
    /// </summary>
    public void CheckLookAround();

    /// <summary>
    /// �ړ�
    /// </summary>
    public void Move();

    /// <summary>
    /// ���̍X�V
    /// </summary>
    public void StatusUpdate();
}
