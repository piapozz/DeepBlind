using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicVigilance : IVigilance
{
    // �X�V������
    EnemyInfo enemyInfo;

    // �s��
    public EnemyInfo Activity(EnemyInfo info)
    {
        // �擾
        GetTarget();

        // ���S�Ɍ����������ǂ���
        CheckTargetLost();

        // ���������ǂ���
        ChekTracking();

        // ���ꏈ��
        Ability();

        // �X�V
        StatusUpdate();

        // �ړ�
        Move();

        return enemyInfo;
    }

    // ������
    public void Init()
    {
        enemyInfo = new EnemyInfo();
    }

    // ���S�Ɍ����������ǂ���
    public void CheckTargetLost()
    {

    }

    // ���������ǂ���
    public void ChekTracking()
    {

    }

    // ���ꏈ��
    public void Ability()
    {

    }

    // �ڕW�ʒu�̎擾
    public void GetTarget()
    {

    }

    // ���̍X�V
    public void StatusUpdate()
    {

    }

    // �ړ�
    public void Move()
    {

    }
}
