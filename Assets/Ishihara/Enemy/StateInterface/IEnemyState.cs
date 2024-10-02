using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public interface IEnemyState
{
    // s“®
    public EnemyInfo Activity(EnemyInfo info , ISkill skill);

    // ‰Šú‰»
    public void Init();
}
