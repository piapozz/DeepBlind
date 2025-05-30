using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="animator"></param>
    void Init(int setID);

    /// <summary>
    /// 特殊処理
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    void Ability();
}
