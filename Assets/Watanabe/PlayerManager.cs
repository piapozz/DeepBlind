using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    bool canMove = true;

    // �v���C���[�̃X�e�[�^�X�l
    public struct PlayerStatus
    {
        public float stamina;               // �X�^�~�i��
        public float speed;                 // ��������
        public float fear;                  // �|�C�x
        public float soundRange;            // �v���C���[���o���Ă��܂����͈̔�
    }

    [SerializeField] IMove iMove;

    // ������
    void Start()
    {
        iMove = new PlayerWalk();
    }

    // ����
    private void FixedUpdate()
    {
        if (canMove)
        {
            // ����
            iMove.Move();

            // �����`�L�[��������Ă����ԂȂ�_�b�V���ɐ؂�ւ���
            // iMove = new PlayerDash();
        }

        // �|�C�����炷





    }
}
