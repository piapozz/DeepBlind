using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicEnemy : EnemyBase
{
    // それぞれのステートクラス
    [SerializeReference, SubclassSelector(true)] ISeach _seach;
    [SerializeReference, SubclassSelector(true)] IVigilance _vigilance;
    [SerializeReference, SubclassSelector(true)] ITracking _tracking;

    //初期化
    public override void Init()
    {
        // 参照ステータスの初期化
        myInfo.id = 0;
        myInfo.speed = 0.5f;
        myInfo.accelerate = 8.0f;
        myInfo.animSpeed = 1.0f;
        myInfo.threatRange = 1.0f;
        myInfo.fieldOfView = 120.0f;
        myInfo.viewLength = 10.0f;

        //ステート初期化
        seach = _seach;
        vigilance = _vigilance;
        tracking = _tracking;

       // myInfo.status.targetPos = new Vector3(20.0f, 0.0f, 0.0f);
        myInfo.status.position = this.transform.position;

        enemyState = _seach;
    }

}
