using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITracking : IEnemyState
{
    /// <summary>
    /// �ڕW�ʒu�̎擾
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget();

    /// <summary>
    /// �����������ǂ���
    /// </summary>
    public void CheckTargetLost();
}
