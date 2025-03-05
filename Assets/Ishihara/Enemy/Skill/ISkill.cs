using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    /// <summary>
    /// ‰Šú‰»
    /// </summary>
    /// <param name="animator"></param>
    void Init(int setID);

    /// <summary>
    /// “Áêˆ—
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    void Ability();
}
