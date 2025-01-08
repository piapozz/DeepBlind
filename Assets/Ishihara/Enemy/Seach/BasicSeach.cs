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
        enemyInfo = skill.Ability(enemyInfo);

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
        RaycastHit hit;

        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = enemyInfo.status.position;                                              // ���_
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                    // Ray�𐶐�;

        if (Physics.Raycast(ray, out hit, enemyInfo.pram.viewLength + 1 , 1)) 
        {
            string tag = hit.collider.gameObject.tag;                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾
            
            if(tag != "Player") return;                                                          // �v���C���[�ȊO�Ȃ�I���

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
        // �X�e�[�g�̐؂�ւ�
        if (tracking) enemyInfo.status.state = State.TRACKING;
    }

}
