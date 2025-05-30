using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public interface IEnemyState
{
    /// <summary>
    /// 行動
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public void Activity();

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(int setID);
}
