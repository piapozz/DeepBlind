using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;
using static UnityEngine.Rendering.HableCurve;
using static UnityEngine.UI.GridLayoutGroup;

public class BasicSeach : ISeach
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

        // ���������ǂ���
        CheckTracking();

        // �x�������𖞂��������ǂ���
        CheckVigilance();
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
    /// ���������ǂ���
    /// </summary>
    public void CheckTracking()
    {
        RaycastHit hit;
        Vector3 playerPos = _player.transform.position;
        Vector3 enemyPos = _enemy.transform.position;

        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = _enemy.transform.position;                                              // ���_
        Vector3 direction = Vector3.Normalize(playerPos - enemyPos);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                    // Ray�𐶐�;

        LayerMask layer = LayerMask.GetMask("Player") | LayerMask.GetMask("Stage");
        if (Physics.Raycast(ray, out hit, _enemy.viewLength + 1, layer))
        {
            string tag = hit.collider.gameObject.tag;                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾

            if (tag != "Player") return;                                                          // �v���C���[�ȊO�Ȃ�I���
                                                                
            float toPlayerAngle = Mathf.Atan2(playerPos.z - enemyPos.z,
                                   playerPos.x - enemyPos.x) * Mathf.Rad2Deg;
            Vector3 dir = _enemy.transform.forward;
            float myAngle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

            // 0 ~ 360�ɃN�����v
            toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
            myAngle = Mathf.Repeat(myAngle, 360);

            // ����͈͓��Ȃ�
            if (myAngle + (_enemy.fieldOfView / 2) > toPlayerAngle &&
                myAngle - (_enemy.fieldOfView / 2) < toPlayerAngle)
            {
                // ������
                _enemy.StateChange(State.TRACKING);
            }
        }
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
        Debug.DrawLine(_enemy.transform.position, _enemy.target);
        if (Vector3.Distance(_enemy.target, _enemy.transform.position) < 2.0f)
        {
            // �T���ӏ��������_���ɐݒ肷��
            _enemy.SetNavTarget(EnemyManager.instance.DispatchTargetPosition());
        }
        else
        {
            _enemy.SetNavTarget(_enemy.target);
        }
    }
}
