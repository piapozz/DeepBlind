using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicVigilance : IVigilance
{
    // �X�V������
    EnemyInfo enemyInfo;

    // �ǐՒ�����x���Ɉڂ������ŏ�����ς���t���O
    bool isViaSearch = false;

    int viaNum = 0;

    // ��ԊǗ��t���O
    bool search = false;
    bool tracking = false;

    // �s��
    public EnemyInfo Activity(EnemyInfo info, ISkill skill)
    {
        // �擾
        GetTarget(info);

        // ���S�Ɍ����������ǂ���
        CheckLookAround();

        // ���ꏈ��
        enemyInfo = skill.Ability(enemyInfo);

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
        isViaSearch = false;
    }

    public void CheckLookAround()
    {
        if(!isViaSearch && !enemyInfo.status.prediction) return;

        enemyInfo.status.prediction = false;
        isViaSearch = true;

        // �ڕW�n�_�����X�g���Ɋi�[
        enemyInfo.status.targetPos = enemyInfo.status.viaData[viaNum].viaPosition;

        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = enemyInfo.status.position;                                                   // ���_
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                    // Ray�𐶐�;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, enemyInfo.pram.viewLength + 1, 1))                                                       // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            string tag = hit.collider.gameObject.tag;                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾

            // �v���C���[�Ȃ�
            if (tag == "Player")
            {
                float toPlayerAngle = Mathf.Atan2(enemyInfo.playerStatus.playerPos.z - enemyInfo.status.position.z,
                                   enemyInfo.playerStatus.playerPos.x - enemyInfo.status.position.x) * Mathf.Rad2Deg;
                float myAngle = Mathf.Atan2(enemyInfo.status.dir.z, enemyInfo.status.dir.x) * Mathf.Rad2Deg;

                // 0 ~ 360�ɃN�����v
                toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
                myAngle = Mathf.Repeat(myAngle, 360);

                // ����͈͓��Ȃ�
                if (myAngle + (enemyInfo.pram.fieldOfView / 2) > toPlayerAngle &&
                    myAngle - (enemyInfo.pram.fieldOfView / 2) < toPlayerAngle)
                {
                    // ������
                    tracking = true;
                }
            }
        }

        // �����ɓ��������猩�n��
        if (Vector3.Distance(enemyInfo.status.position , enemyInfo.status.targetPos) > 3.0f) return;

        // ���n��
        if(!LookAround()) return;

        // ���̏���n�_��ݒ�
        viaNum++;

        Debug.Log("�o�R�n�_" + viaNum + "/" + enemyInfo.status.viaData.Count + "�ʉ�");

        // �x���I��
        if(viaNum == enemyInfo.status.viaData.Count) search = true;
    }

    private bool LookAround()
    {
        if(!enemyInfo.status.viaData[viaNum].room) return true;

        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = enemyInfo.status.position;                                                   // ���_
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                    // Ray�𐶐�;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))                                                       // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            string tag = hit.collider.gameObject.tag;                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾

            // �v���C���[�Ȃ�
            if (tag == "Player")
            {
                // ������
                tracking = true;
                return false;
            }
        }

        return true;
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
