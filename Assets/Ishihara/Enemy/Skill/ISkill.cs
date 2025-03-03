using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    /// <summary>
    /// ������
    /// </summary>
    /// <param name="animator"></param>
    void Init(Animator animator);

    /// <summary>
    /// ���ꏈ��
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    EnemyBase.EnemyInfo Ability(EnemyBase.EnemyInfo info);
}
