using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicTracking : ITracking
{
    // ���
    private EnemyInfo _enemyInfo;

    // �x���t���O
    private bool _vigilance = false;

    /// <summary>
    /// �s��
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public EnemyInfo Activity(EnemyInfo info, ISkill skill)
    {
        // �擾
        GetTarget(info);

        // �����������ǂ���
        CheckTargetLost();

        // ���ꏈ��
        _enemyInfo = skill.Ability(_enemyInfo);

        // �ړ�
        Move();

        // �X�V
        StatusUpdate();

        return _enemyInfo;
    }

    /// <summary>
    /// ������
    /// </summary>
    public void Init()
    {
        _enemyInfo = new EnemyInfo();

        // �x���t���O
        _vigilance = false;
    }

    /// <summary>
    /// �����������ǂ���
    /// </summary>
    public void CheckTargetLost()
    {
        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = _enemyInfo.status.position;                                                              // ���_
        Vector3 direction = Vector3.Normalize(_enemyInfo.playerStatus.playerPos - _enemyInfo.status.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                                    // Ray�𐶐�;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))                                                 // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            string tag = hit.collider.gameObject.tag;                                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾

            // ���߂Č������Ă�����
            if (tag != "Player")
            {
                _vigilance = true;
            }
        }
    }

    /// <summary>
    /// �ڕW�ʒu�̎擾
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget(EnemyInfo info)
    {
        // �^�[�Q�b�g�̏��擾
        _enemyInfo = info;
    }

    /// <summary>
    /// ���̍X�V
    /// </summary>
    public void StatusUpdate()
    {
        // �X�e�[�g�̐؂�ւ�
        if (_vigilance) _enemyInfo.status.state = State.VIGILANCE;
    }

    /// <summary>
    /// �ړ�
    /// </summary>
    public void Move()
    {
        // �ڕW�ʒu��ݒ�
        _enemyInfo.status.targetPos = _enemyInfo.playerStatus.playerPos;                       // �ǐՒ�

    }
}
