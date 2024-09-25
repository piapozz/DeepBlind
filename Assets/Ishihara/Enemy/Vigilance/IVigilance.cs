using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public interface IVigilance : IEnemyState
{
    // �ڕW�ʒu�̎擾
    public void GetTarget(EnemyInfo info);

    // ���S�Ɍ����������ǂ���
    public void CheckLookAround();

    // ���ꏈ��
    public void Ability();

    // �ړ�
    public void Move();

    // ���̍X�V
    public void StatusUpdate();
}
