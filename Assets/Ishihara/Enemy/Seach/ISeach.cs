using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public interface ISeach : IEnemyState
{
    // �ڕW�ʒu�̎擾
    public void GetTarget(EnemyInfo info);

    // ���������ǂ���
    public void CheckTracking();

    // �x�������𖞂��������ǂ���
    public void CheckVigilance();

    // ���̍X�V
    public void StatusUpdate(EnemyInfo info);
}
