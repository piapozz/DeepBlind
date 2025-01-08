using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicTracking : ITracking
{
    // ���
    EnemyInfo enemyInfo;

    // �x���t���O
    bool vigilance = false;

    // �s��
    public EnemyInfo Activity(EnemyInfo info, ISkill skill)
    {
        // �擾
        GetTarget(info);

        // �����������ǂ���
        CheckTargetLost();

        // ���ꏈ��
        enemyInfo = skill.Ability(enemyInfo);

        // �ړ�
        Move();

        // �X�V
        StatusUpdate();

        return enemyInfo;
    }

    // ������
    public void Init()
    {
        enemyInfo = new EnemyInfo();

        // �x���t���O
        vigilance = false;
    }

    // �����������ǂ���
    public void CheckTargetLost()
    {
        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = enemyInfo.status.position;                                                              // ���_
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                                    // Ray�𐶐�;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))                                                 // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            string tag = hit.collider.gameObject.tag;                                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾

            // ���߂Č������Ă�����
            if (tag != "Player")
            {
                // ���X�g�|�W�V������ݒ�
                enemyInfo.status.lostPos = enemyInfo.playerStatus.playerPos;

                // �v���C���[�̈ړ��ʂ�ۑ�
                enemyInfo.status.lostMoveVec = enemyInfo.playerStatus.moveValue;

                vigilance = true;

                // ��������
                enemyInfo.status.prediction = true;
                enemyInfo.status.isTargetLost = false;
            }
        }
    }

    // �ڕW�ʒu�̎擾
    public void GetTarget(EnemyInfo info)
    {
        // �^�[�Q�b�g�̏��擾
        enemyInfo = info;
    }

    // ���̍X�V
    public void StatusUpdate()
    {
        // �X�e�[�g�̐؂�ւ�
        if (vigilance) enemyInfo.status.state = State.VIGILANCE;
    }

    // �ړ�
    public void Move()
    {
        // �ڕW�ʒu��ݒ�
        if(enemyInfo.status.isTargetLost)�@enemyInfo.status.targetPos = enemyInfo.status.lostPos; // �������Ă�����
        else enemyInfo.status.targetPos = enemyInfo.playerStatus.playerPos;                       // �ǐՒ�

    }
}
