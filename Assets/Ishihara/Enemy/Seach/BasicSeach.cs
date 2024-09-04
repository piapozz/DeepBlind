using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static EnemyBase;

public class BasicSeach : ISeach
{
    EnemyInfo enemyInfo = new EnemyInfo();
    
    bool tracking;              // ������

    // �s��
    public EnemyInfo Activity(EnemyInfo info)
    {
        // �擾
        GetTarget(info);

        // ���������ǂ���
        ChekTracking();

        // �x�������𖞂��������ǂ���
        CheckVigilance();

        // ���ꏈ��
        Ability();

        // �X�V
        StatusUpdate(info);

        return enemyInfo;
    }

    // ������
    public void Init()
    {
        enemyInfo = new EnemyInfo();

        tracking = false;              // ������
    }

    // ���������ǂ���
    public void ChekTracking()
    {
        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = enemyInfo.status.position;                                                   // ���_
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                    // Ray�𐶐�;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, enemyInfo.viewLength))                                                       // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            string tag = hit.collider.gameObject.tag;                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾
            
            if(tag != "Player") return;                                                          // �v���C���[�ȊO�Ȃ�I���

            float toPlayerAngle = Template(enemyInfo.status.position, enemyInfo.playerStatus.playerPos) + 180;   // �v���C���[�ւ̊p�x
            float myAngle = Template(enemyInfo.status.dir) + 180;                                     // �����Ă�p�x

            // ����͈͓��Ȃ�
            if(myAngle + (enemyInfo.fieldOfView / 2) > toPlayerAngle &&
                myAngle - (enemyInfo.fieldOfView / 2) < toPlayerAngle)
            {
                // ������
                tracking = true;
            }
        }
    }
    private float Template(Vector3 point1)
    {
        float temp;

        point1.Normalize();

        temp = Mathf.Atan2(point1.z , point1.x);

        return temp;
    }

    private float Template(Vector3 point1 , Vector3 point2)
    {
        float temp;

        point1.Normalize();
        point2.Normalize();

        temp = Mathf.Atan2(point1.z - point2.z , point1.x - point2.x);

        return temp; 
    }


    // ���ꏈ��(�����Ă�����~�܂�)
    public void Ability()
    {
        Plane[] planes;

        // �J�����̎���������߂�
        planes = GeometryUtility.CalculateFrustumPlanes(enemyInfo.playerStatus.cam);

        // �J�����Ɏʂ��Ă��邩����
        if(GeometryUtility.TestPlanesAABB(planes , enemyInfo.bounds))
        {
            // �f���Ă����琧�~����
            enemyInfo.status.targetPos = enemyInfo.status.position; // �ڕW�ʒu�����݈ʒu��
            enemyInfo.animator.speed = 0.0f;                        // �A�j���[�V�����̍Đ����~
        }
        else enemyInfo.animator.speed = 1.0f;   // �ʏ�Đ�

    }

    // �x�������𖞂��������ǂ���
    public void CheckVigilance()
    {

    }

    // �ڕW�ʒu�̎擾
    public void GetTarget(EnemyInfo info)
    {
        // �^�[�Q�b�g�̏��擾
        enemyInfo = info; 
    }

    // ���̍X�V
    public void StatusUpdate(EnemyInfo info)
    {
        //// ����
        //enemyInfo = info;

        //// �X�e�[�^�X�X�V
        //enemyInfo.status = enemyStatus;

        // �X�e�[�g�̐؂�ւ�
        if (tracking) enemyInfo.status.state = State.TRACKING;
    }

}
