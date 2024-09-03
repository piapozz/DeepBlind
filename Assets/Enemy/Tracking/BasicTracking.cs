using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicTracking : ITracking
{
    // ���
    EnemyInfo enemyInfo;

    // �T���t���O
    bool seach = false;

    // �s��
    public EnemyInfo Activity(EnemyInfo info)
    {
        // �擾
        GetTarget(info);

        // �����������ǂ���
        CheckTargetLost();

        // ���ꏈ��
        Ability();

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

        seach = false;              // ������
    }

    // �����������ǂ���
    public void CheckTargetLost()
    {
        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = enemyInfo.status.position;                                                              // ���_
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                                    // Ray�𐶐�;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, enemyInfo.viewLength))                                                 // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            string tag = hit.collider.gameObject.tag;                                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾

            if (tag != "Player") return;                                                                         // �v���C���[�ȊO�Ȃ�I���

            float toPlayerAngle = Template(enemyInfo.status.position, enemyInfo.playerStatus.playerPos) + 180;   // �v���C���[�ւ̊p�x
            float myAngle = Template(enemyInfo.status.dir) + 180;                                                // �����Ă�p�x

            // ����͈͓��Ȃ�
            if (myAngle + (enemyInfo.fieldOfView / 2) > toPlayerAngle &&
                myAngle - (enemyInfo.fieldOfView / 2) < toPlayerAngle)
            {
                // �ǐՌp��

                // ���O�܂Ō������Ă����Ȃ�
                if (enemyInfo.status.isTargetLost) enemyInfo.status.isTargetLost = false; // �Ĕ���
            }
            // ���߂Č������Ă�����
            else if (enemyInfo.status.isTargetLost == false)
            {
                // ���X�g�|�W�V������ݒ�
                enemyInfo.status.lostPos = enemyInfo.playerStatus.playerPos;
                enemyInfo.status.isTargetLost = true;
            }
            // �Ō�̌��������n�_�ɓ��B���ĂȂ��������Ȃ�������
            else if(enemyInfo.status.position == enemyInfo.status.lostPos)
            {
                seach = true;
            }
        }
    }
    private float Template(Vector3 point1)
    {
        float temp;

        point1.Normalize();

        temp = Mathf.Atan2(point1.z, point1.x);

        return temp;
    }

    private float Template(Vector3 point1, Vector3 point2)
    {
        float temp;

        point1.Normalize();
        point2.Normalize();

        temp = Mathf.Atan2(point1.z - point2.z, point1.x - point2.x);

        return temp;
    }

    // �ڕW�ʒu�̎擾
    public void GetTarget(EnemyInfo info)
    {
        // �^�[�Q�b�g�̏��擾
        enemyInfo = info;
    }

    // ���ꏈ��
    public void Ability()
    {
        Plane[] planes;

        // �J�����̎���������߂�
        planes = GeometryUtility.CalculateFrustumPlanes(enemyInfo.playerStatus.cam);

        // �J�����Ɏʂ��Ă��邩����
        if (GeometryUtility.TestPlanesAABB(planes, enemyInfo.bounds))
        {
            // �f���Ă����琧�~����
            enemyInfo.status.targetPos = enemyInfo.status.position; // �ڕW�ʒu�����݈ʒu��
            enemyInfo.animator.speed = 0.0f;                        // �A�j���[�V�����̍Đ����~
        }
        else enemyInfo.animator.speed = 1.0f;   // �ʏ�Đ�
    }

    // ���̍X�V
    public void StatusUpdate()
    {
        // �X�e�[�g�̐؂�ւ�
        if (seach) enemyInfo.status.state = State.SEACH;
    }

    // �ړ�
    public void Move()
    {
        // �ڕW�ʒu��ݒ�
        if(enemyInfo.status.isTargetLost)�@enemyInfo.status.targetPos = enemyInfo.status.lostPos; // �������Ă�����
        else enemyInfo.status.targetPos = enemyInfo.playerStatus.playerPos;                       // �ǐՒ�

    }
}
