using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public interface IEnemyState
{
    /// <summary>
    /// s“®
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public void Activity();

    /// <summary>
    /// ‰Šú‰»
    /// </summary>
    public void Init(int setID);
}
