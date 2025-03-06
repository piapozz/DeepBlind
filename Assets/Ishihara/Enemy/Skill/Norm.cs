using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyBase;

public class Norm : ISkill
{

    private int ID;

    /// <summary>
    /// ‰Šú‰»
    /// </summary>
    /// <param name="animator"></param>
    public void Init(int setID)
    {
        ID = setID;
    }

    public void Ability()
    {
       
    }
}
