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

    // ��b�f�[�^
    [SerializeField] EnemyPram _pram;

    //������
    public override void Init()
    {
        // �Q�ƃX�e�[�^�X�̏�����
        myInfo.pram = _pram;
        myInfo.status.prediction = false;

        //�X�e�[�g������
        seach = _seach;
        vigilance = _vigilance;
        tracking = _tracking;

        // �X�L��������
        seachSkill = _seachSkill;
        vigilanceSkill = _vigilanceSkill;
        trackingSkill = _trackingSkill;

        // ���݂̃X�e�[�g�A�X�L��������
        enemyState = _seach;
        skill = _seachSkill;
    }
}
