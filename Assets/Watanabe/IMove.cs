using UnityEngine;

public interface PlayerState
{
    // �v���C���[�̃X�e�[�^�X�l
    public struct PlayerStatus
    {
        public float stamina;                                       // �X�^�~�i��
        public float speed;                                         // ��������
        public float fear;                                          // �|�C�x
        public float soundRange;                                    // �v���C���[���o���Ă��܂����͈̔�
    }

    // ����
    public void Move();


}
