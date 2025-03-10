using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public interface ISeach : IEnemyState
{
    // –Ú•WˆÊ’u‚Ìæ“¾
    public void GetTarget();

    // Œ©‚Â‚¯‚½‚©‚Ç‚¤‚©
    public void CheckTracking();

    // Œx‰úğŒ‚ğ–‚½‚µ‚½‚©‚Ç‚¤‚©
    public void CheckVigilance();
}
