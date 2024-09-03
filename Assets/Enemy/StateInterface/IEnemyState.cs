using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public interface IEnemyState
{
    // s“®
    public EnemyInfo Activity(EnemyInfo info);

    // ‰Šú‰»
    public void Init();
}
