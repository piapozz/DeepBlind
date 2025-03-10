using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public interface IVigilance : IEnemyState
{
    /// <summary>
    /// –Ú•WˆÊ’u‚ÌŽæ“¾
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget();

    /// <summary>
    /// Š®‘S‚ÉŒ©Ž¸‚Á‚½‚©‚Ç‚¤‚©
    /// </summary>
    public void CheckLookAround();
}
