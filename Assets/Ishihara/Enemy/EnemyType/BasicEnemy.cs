using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicEnemy : EnemyBase
{
    // ���ꂼ��̃X�e�[�g�N���X
    [SerializeReference, SubclassSelector(true)] ISeach _seach;
    [SerializeReference, SubclassSelector(true)] IVigilance _vigilance;
    [SerializeReference, SubclassSelector(true)] ITracking _tracking;

    [SerializeReference, SubclassSelector(true)] ISkill _seachSkill;
    [SerializeReference, SubclassSelector(true)] ISkill _vigilanceSkill;
    [SerializeReference, SubclassSelector(true)] ISkill _trackingSkill;

    [SerializeField] int _id = 0;     // ���ʔԍ�
    [SerializeField] float _speed = 1f;     // ����
    [SerializeField] float _speedDiameter = 5f;     // �����̔{��
    [SerializeField] float _animSpeed = 1f;     // �A�j���[�V�����̑���
    [SerializeField] float _threatRange = 1f;     // ���̊��m�͈�
    [SerializeField] float _fieldOfView = 120f;     // ����p
    [SerializeField] float _viewLength = 10f;     // ����̒���

    //������
    public override void Init()
    {
        // �Q�ƃX�e�[�^�X�̏�����
        myInfo.id = _id;
        myInfo.speed = _speed;
        myInfo.speedDiameter = _speedDiameter; 
        myInfo.animSpeed = _animSpeed;
        myInfo.threatRange = _threatRange;
        myInfo.fieldOfView = _fieldOfView;
        myInfo.viewLength = _viewLength;
        myInfo.status.prediction = false;

        //�X�e�[�g������
        seach = _seach;
        vigilance = _vigilance;
        tracking = _tracking;

        seachSkill = _seachSkill;
        vigilanceSkill = _vigilanceSkill;
        trackingSkill = _trackingSkill;

        enemyState = _seach;
        skill = _seachSkill;
    }

}
