using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static EnemyBase;

public class BasicVigilance : IVigilance
{
    // �X�V������
    EnemyInfo enemyInfo;

    // �ǐՒ�����x���Ɉڂ������ŏ�����ς���t���O
    bool isViaSearch = false;

    // ��ԊǗ��t���O
    bool search = false;
    bool tracking = false;

    // ���Ԍv��
    float time = 0;

    // �s��
    public EnemyInfo Activity(EnemyInfo info, ISkill skill)
    {
        // �擾
        GetTarget(info);

        // ���S�Ɍ����������ǂ���
        CheckLookAround();

        // ���ꏈ��
        enemyInfo = skill.Ability(info);

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

    // ��������n��
    public void CheckLookAround()
    {
        Debug.DrawLine(enemyInfo.status.position, enemyInfo.status.targetPos, Color.blue, 0.01f);

        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = enemyInfo.status.position;                                                   // ���_
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                    // Ray�𐶐�;

        RaycastHit hit;

        for (int i = 0; i < 100; i++)
        {
            float myAngle1 = Template(enemyInfo.status.dir);

            Debug.DrawLine(ray.origin,
                ray.origin + (
                Quaternion.Euler(
                    new Vector3(0, Mathf.Repeat(myAngle1, 360) - (enemyInfo.fieldOfView / 2) + ((enemyInfo.fieldOfView / 100) * i), 0)) * enemyInfo.status.dir * enemyInfo.viewLength),
                Color.gray,
                0.01f);
        }

        if (Physics.Raycast(ray, out hit, enemyInfo.viewLength + 1, 1))                                                       // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            Debug.DrawLine(ray.origin, enemyInfo.status.targetPos, Color.red, 0.01f);
            // Debug.DrawLine(ray.origin, ray.origin + (enemyInfo.status.dir * enemyInfo.viewLength), Color.blue, 0.01f);

            string tag = hit.collider.gameObject.tag;                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾

            // �v���C���[�Ȃ�
            if (tag == "Player")
            {

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

        // �����ɓ�������
        if (Vector3.Distance(enemyInfo.status.position , enemyInfo.status.targetPos) > 3.0f) return;

        //// �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        //Vector3 origin = enemyInfo.status.position;                                                              // ���_
        //Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X��������\���x�N�g��
        //Ray ray = new Ray(origin, direction);                                                                    // Ray�𐶐�;

        //RaycastHit hit;
        if (Physics.Raycast(ray, out hit, enemyInfo.viewLength))                                                 // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            string tag = hit.collider.gameObject.tag;                                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾

            // ����͈͓��Ȃ�
            if (tag == "Player")
            {
                // �ǐՌp��

                // �v���C���[��������TRACKING��Ԃɐ؂�ւ���
                tracking = true;
            }
            // ���߂Č������Ă�����
            else if (tag != "Player")
            {
                // �v���C���[�����Ȃ�������SEARCH��Ԃɐ؂�ւ�
                search = true;
            }
        }
        else
        {

            // �v���C���[�����Ȃ�������SEARCH��Ԃɐ؂�ւ�
            search = true;
        }

    }

    private void LookAround()
    {
        // ���Ԃ��v��
        time += Time.deltaTime;

        // ���Ԃɏ]���ĕ�����T��

        // ��]����

        // ��������玟�̒T���ꏊ��
        

    }

    // �p�x�v�Z
    private float Template(Vector3 point1)
    {
        float temp;

        temp = Mathf.Atan2(point1.z, point1.x);

        return temp;
    }

    // �p�x�v�Z
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

    // ���̍X�V
    public void StatusUpdate()
    {
        // �X�e�[�g�̐؂�ւ�
        if (search) enemyInfo.status.state = State.SEARCH;
        if (tracking) enemyInfo.status.state = State.TRACKING;
    }

    // �ړ�
    public void Move()
    {

    }
}
