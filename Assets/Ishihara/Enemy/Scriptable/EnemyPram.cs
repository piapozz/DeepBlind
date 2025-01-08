using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �G�̊�{�f�[�^

[CreateAssetMenu]
public class EnemyPram : ScriptableObject
{
    public int id;                      // �G�l�~�[�̎��ʔԍ�
    public float speed;                 // �G�l�~�[�̑���
    public float speedDiameter;         // ���������̑����̔{��
    public float animSpeed;             // �A�j���[�V�����̑���
    public float threatRange;           // ���Д͈�
    public float viewLength;            // ���E�̒���
    public float fieldOfView;           // ����p
}
