using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;
using static UnityEditor.Experimental.GraphView.GraphView;

public class BasicVigilance : IVigilance
{
    private int _ID = -1;
    private EnemyBase _enemy;
    private Player _player;

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

        // ���S�Ɍ����������ǂ���
        CheckLookAround();

        // �X�V
        StatusUpdate();
    }

    /// <summary>
    /// ������
    /// </summary>
    public void Init(int setID)
    {
        _ID = setID;
        _enemy = EnemyUtility.GetCharacter(_ID);
        _player = EnemyUtility.GetPlayer();
    }

    public void CheckLookAround()
    {
        //// �ڕW�n�_�����X�g���Ɋi�[

        //// �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        //Vector3 origin = _enemyInfo.status.position;                                                   // ���_
        //Vector3 direction = Vector3.Normalize(_enemyInfo.playerStatus.playerPos - _enemyInfo.status.position);     // X��������\���x�N�g��
        //Ray ray = new Ray(origin, direction);                                                    // Ray�𐶐�;

        //RaycastHit hit;

        //if (Physics.Raycast(ray, out hit, _enemyInfo.pram.viewLength + 1, 1))                                                       // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        //{
        //    string tag = hit.collider.gameObject.tag;                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾

        //    // �v���C���[�Ȃ�
        //    if (tag == "Player")
        //    {
        //        float toPlayerAngle = Mathf.Atan2(_enemyInfo.playerStatus.playerPos.z - _enemyInfo.status.position.z,
        //                           _enemyInfo.playerStatus.playerPos.x - _enemyInfo.status.position.x) * Mathf.Rad2Deg;
        //        float myAngle = Mathf.Atan2(_enemyInfo.status.dir.z, _enemyInfo.status.dir.x) * Mathf.Rad2Deg;

        //        // 0 ~ 360�ɃN�����v
        //        toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
        //        myAngle = Mathf.Repeat(myAngle, 360);

        //        // ����͈͓��Ȃ�
        //        if (myAngle + (_enemyInfo.pram.fieldOfView / 2) > toPlayerAngle &&
        //            myAngle - (_enemyInfo.pram.fieldOfView / 2) < toPlayerAngle)
        //        {
        //            // ������
        //            _tracking = true;
        //        }
        //    }
        //}

        //// �����ɓ��������猩�n��
        //if (Vector3.Distance(_enemyInfo.status.position, _enemyInfo.status.targetPos) > 3.0f) return;

        //// ���n��
        //if (!LookAround()) return;

        //// ���̏���n�_��ݒ�

        //// �x���I��
    }

    private bool LookAround()
    {
        //// �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        //Vector3 origin = _enemyInfo.status.position;                                                   // ���_
        //Vector3 direction = Vector3.Normalize(_enemyInfo.playerStatus.playerPos - _enemyInfo.status.position);     // X��������\���x�N�g��
        //Ray ray = new Ray(origin, direction);                                                    // Ray�𐶐�;

        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit))                                                       // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        //{
        //    string tag = hit.collider.gameObject.tag;                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾

        //    // �v���C���[�Ȃ�
        //    if (tag == "Player")
        //    {
        //        // ������
        //        _tracking = true;
        //        return false;
        //    }
        //}

        return true;
    }

    // �ڕW�ʒu�̎擾
    public void GetTarget()
    {
       
    }

    // ���̍X�V
    public void StatusUpdate()
    {

    }
}
