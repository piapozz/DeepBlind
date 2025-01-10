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

    [SerializeReference, SubclassSelector(true)] ISkill _seachSkill;
    [SerializeReference, SubclassSelector(true)] ISkill _vigilanceSkill;
    [SerializeReference, SubclassSelector(true)] ISkill _trackingSkill;

    // 基礎データ
    [SerializeField] EnemyPram _pram;

    //初期化
    public override void Init()
    {
        // 参照ステータスの初期化
        myInfo.pram = _pram;
        myInfo.status.prediction = false;

        //ステート初期化
        seach = _seach;
        vigilance = _vigilance;
        tracking = _tracking;

        // スキル初期化
        seachSkill = _seachSkill;
        vigilanceSkill = _vigilanceSkill;
        trackingSkill = _trackingSkill;

        // 現在のステート、スキル初期化
        enemyState = _seach;
        skill = _seachSkill;
    }
}
