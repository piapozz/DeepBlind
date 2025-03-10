using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITracking : IEnemyState
{
    /// <summary>
    /// –Ú•WˆÊ’u‚ÌŽæ“¾
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget();

    /// <summary>
    /// Œ©Ž¸‚Á‚½‚©‚Ç‚¤‚©
    /// </summary>
    public void CheckTargetLost();
}
