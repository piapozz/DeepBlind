using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    /// <summary>
    /// ������
    /// </summary>
    /// <param name="animator"></param>
    void Init(int setID);

    /// <summary>
    /// ���ꏈ��
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    void Ability();
}
