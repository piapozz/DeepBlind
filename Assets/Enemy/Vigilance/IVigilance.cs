using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVigilance : IEnemyState
{
    // �ڕW�ʒu�̎擾
    public void GetTarget();

    // ���S�Ɍ����������ǂ���
    public void CheckTargetLost();

    // ���������ǂ���
    public void ChekTracking();

    // ���ꏈ��
    public void Ability();

    // �ړ�
    public void Move();

    // ���̍X�V
    public void StatusUpdate();
}
