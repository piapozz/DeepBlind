using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;
using static UnityEngine.Rendering.HableCurve;
using static UnityEngine.UI.GridLayoutGroup;

public class BasicSeach : ISeach
{
    //private EnemyInfo _enemyInfo = new EnemyInfo();
    
    private bool _tracking;              // ������

    /// <summary>
    /// �s��
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public void Activity()
    {
        // �擾
        GetTarget();

        // ���������ǂ���
        CheckTracking();

        // �x�������𖞂��������ǂ���
        CheckVigilance();

        // ���ꏈ��
        //_enemyInfo = skill.Ability(_enemyInfo);

        // ���ڕW�n�_�ɂ��ǂ蒅�������ǂ���
        CheckReaching();

        // �X�V
        StatusUpdate();

        //return _enemyInfo;
    }

    /// <summary>
    /// ������
    /// </summary>
    public void Init()
    {

    }

    /// <summary>
    /// ���������ǂ���
    /// </summary>
    public void CheckTracking()
    {
        //RaycastHit hit;

        //// �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        //Vector3 origin = _enemyInfo.status.position;                                              // ���_
        //Vector3 direction = Vector3.Normalize(_enemyInfo.playerStatus.playerPos - _enemyInfo.status.position);     // X��������\���x�N�g��
        //Ray ray = new Ray(origin, direction);                                                    // Ray�𐶐�;

        //if (Physics.Raycast(ray, out hit, _enemyInfo.pram.viewLength + 1 , 1)) 
        //{
        //    string tag = hit.collider.gameObject.tag;                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾
            
        //    if(tag != "Player") return;                                                          // �v���C���[�ȊO�Ȃ�I���

        //    float toPlayerAngle = Mathf.Atan2(_enemyInfo.playerStatus.playerPos.z - _enemyInfo.status.position.z,
        //                           _enemyInfo.playerStatus.playerPos.x - _enemyInfo.status.position.x) * Mathf.Rad2Deg;
        //    float myAngle = Mathf.Atan2(_enemyInfo.status.dir.z, _enemyInfo.status.dir.x) * Mathf.Rad2Deg;

        //    // 0 ~ 360�ɃN�����v
        //    toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
        //    myAngle = Mathf.Repeat(myAngle, 360);

        //    // ����͈͓��Ȃ�
        //    if (myAngle + (_enemyInfo.pram.fieldOfView / 2) > toPlayerAngle &&
        //        myAngle - (_enemyInfo.pram.fieldOfView / 2) < toPlayerAngle)
        //    {
        //        // ������
        //        _tracking = true;
        //    }
        //}
    }

    // ���ڕW�n�_�ɂ��ǂ蒅�������ǂ���
    private void CheckReaching()
    { 
        //if((Vector3.Distance(_enemyInfo.status.targetPos, _enemyInfo.status.position) < 2.0f) && (!_enemyInfo.status.isAblity))
        //{
        //    // �T���ӏ��������_���ɐݒ肷��
        //    _enemyInfo.status.targetPos = EnemyManager.Instance.DispatchTargetPosition();
        //}
    }

    // �x�������𖞂��������ǂ���
    public void CheckVigilance()
    {

    }

    /// <summary>
    /// �ڕW�ʒu�̎擾
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget()
    {
        // �^�[�Q�b�g�̏��擾
        //_enemyInfo = info; 
    }

    /// <summary>
    /// ���̍X�V
    /// </summary>
    /// <param name="info"></param>
    public void StatusUpdate()
    {
        // �X�e�[�g�̐؂�ւ�
        //if (_tracking) _enemyInfo.status.state = State.TRACKING;
    }

}
