using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicTracking : ITracking
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

        // �����������ǂ���
        CheckTargetLost();

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

    /// <summary>
    /// �����������ǂ���
    /// </summary>
    public void CheckTargetLost()
    {
        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = _enemy.transform.position;                                                              // ���_
        Vector3 direction = Vector3.Normalize(_player.transform.position - _enemy.transform.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                                    // Ray�𐶐�;

        RaycastHit hit;
        LayerMask layer = LayerMask.GetMask("Player") | LayerMask.GetMask("Stage");
        if (Physics.Raycast(ray, out hit, Vector3.Distance(_enemy.transform.position, _player.transform.position), layer))                                                 // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            string tag = hit.collider.gameObject.tag;                                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾

            // ���߂Č������Ă�����
            if (tag != "Player")
            {
                _enemy.StateChange(State.SEARCH);
            }
        }
    }

    /// <summary>
    /// �ڕW�ʒu�̎擾
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget()
    {
        _enemy.SetNavTarget(_player.transform.position);
    }

    /// <summary>
    /// ���̍X�V
    /// </summary>
    public void StatusUpdate()
    {

    }
}
