using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public interface ISeach : IEnemyState
{
    // �ڕW�ʒu�̎擾
    public void GetTarget();

    // ���������ǂ���
    public void CheckTracking();

    // �x�������𖞂��������ǂ���
    public void CheckVigilance();
}
