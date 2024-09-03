using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITracking : IEnemyState
{
    // �ڕW�ʒu�̎擾
    public void GetTarget(EnemyBase.EnemyInfo info);

    // �����������ǂ���
    public void CheckTargetLost();

    // ���ꏈ��
    public void Ability();

    // �ړ�
    public void Move();

    // ���̍X�V
    public void StatusUpdate();
}
