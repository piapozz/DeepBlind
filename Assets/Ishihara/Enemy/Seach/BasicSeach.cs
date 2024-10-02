using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;
using static UnityEngine.Rendering.HableCurve;
using static UnityEngine.UI.GridLayoutGroup;

public class BasicSeach : ISeach
{
    EnemyInfo enemyInfo = new EnemyInfo();
    
    bool tracking;              // ������

    // �s��
    public EnemyInfo Activity(EnemyInfo info , ISkill skill)
    {
        // �擾
        GetTarget(info);

        // ���������ǂ���
        CheckTracking();

        // �x�������𖞂��������ǂ���
        CheckVigilance();

        // ���ꏈ��
        enemyInfo = skill.Ability(info);

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
    public void CheckTracking()
    {
        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = enemyInfo.status.position;                                                   // ���_
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                    // Ray�𐶐�;

        RaycastHit hit;

        for (int i = 0; i < 100; i++)
        {
            float angleOffset = (-enemyInfo.fieldOfView / 2) + (enemyInfo.fieldOfView / 100) * i; // ������E�܂ŋϓ��ɐ�������
            Vector3 rotatedDirection = Quaternion.Euler(0, angleOffset, 0) * enemyInfo.status.dir; // ��]���ĐV���������x�N�g�����v�Z
            Debug.DrawLine(ray.origin, ray.origin + rotatedDirection * enemyInfo.viewLength, Color.gray, 0.01f); // ����`��
        }

        if (Physics.Raycast(ray, out hit, enemyInfo.viewLength + 1 , 1))                                                       // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            Debug.DrawLine(ray.origin, enemyInfo.status.targetPos, Color.red,0.01f);
            // Debug.DrawLine(ray.origin, ray.origin + (enemyInfo.status.dir * enemyInfo.viewLength), Color.blue, 0.01f);

            string tag = hit.collider.gameObject.tag;                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾
            
            if(tag != "Player") return;                                                          // �v���C���[�ȊO�Ȃ�I���

            float toPlayerAngle = Template(enemyInfo.status.position, enemyInfo.playerStatus.playerPos);   // �v���C���[�ւ̊p�x
            float myAngle = Template(enemyInfo.status.dir);                                     // �����Ă�p�x

            // 0 ~ 360�ɃN�����v
            toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
            myAngle = Mathf.Repeat(myAngle, 360);

            // ����͈͓��Ȃ�
            if (myAngle + (enemyInfo.fieldOfView / 2) > toPlayerAngle &&
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

        // point1.Normalize();

        temp = Mathf.Atan2(point1.z , point1.x);

        return temp;
    }

    private float Template(Vector3 point2 , Vector3 point1)
    {
        float temp;

        // point1.Normalize();
        // point2.Normalize();

        temp = Mathf.Atan2(point1.z - point2.z , point1.x - point2.x);

        return temp; 
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
