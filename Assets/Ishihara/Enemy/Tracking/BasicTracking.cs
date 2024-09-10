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

            float toPlayerAngle = Template(enemyInfo.status.position, enemyInfo.playerStatus.playerPos) * 180 / Mathf.PI;   // �v���C���[�ւ̊p�x
            float myAngle = Template(enemyInfo.status.dir);                                                // �����Ă�p�x

            Debug.Log(toPlayerAngle);

            // ����͈͓��Ȃ�
            if ((myAngle + (enemyInfo.fieldOfView / 2) > toPlayerAngle &&
                myAngle - (enemyInfo.fieldOfView / 2) < toPlayerAngle) &&
                tag == "Player")
            {
                // �ǐՌp��
                Debug.Log("����");

                // ���O�܂Ō������Ă����Ȃ�
                if (enemyInfo.status.isTargetLost) enemyInfo.status.isTargetLost = false; // �Ĕ���
            }
            // ���߂Č������Ă�����
            else if (enemyInfo.status.isTargetLost == false && tag != "Player")
            {
                // ���X�g�|�W�V������ݒ�
                enemyInfo.status.lostPos = enemyInfo.playerStatus.playerPos;
                enemyInfo.status.isTargetLost = true;

                Debug.Log("��������");
            }
            // �Ō�̌��������n�_�ɓ��B���ĂȂ��������Ȃ�������
            else if (Vector3.Distance(enemyInfo.status.lostPos, enemyInfo.status.position) < 1.0f && tag != "Player")
            {
                seach = true;

                Debug.Log("�T���ɖ߂�");
            }
        }
    }
    private float Template(Vector3 point1)
    {
        float temp;

        temp = Mathf.Atan2(point1.z, point1.x);

        return temp;
    }

    private float Template(Vector3 point1, Vector3 point2)
    {
        float temp;

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
        //Plane[] planes;

        //// �J�����̎���������߂�
        //planes = GeometryUtility.CalculateFrustumPlanes(enemyInfo.playerStatus.cam);

        //// �J�����Ɏʂ��Ă��邩����
        //if (GeometryUtility.TestPlanesAABB(planes, enemyInfo.bounds))
        //{
        //    // �f���Ă����琧�~����
        //    enemyInfo.status.targetPos = enemyInfo.status.position; // �ڕW�ʒu�����݈ʒu��
        //    enemyInfo.animator.speed = 0.0f;                        // �A�j���[�V�����̍Đ����~
        //}
        //else enemyInfo.animator.speed = 1.0f;   // �ʏ�Đ�

        enemyInfo.animator.speed = 2.0f;
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
