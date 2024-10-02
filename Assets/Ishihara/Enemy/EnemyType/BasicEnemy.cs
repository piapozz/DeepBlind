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

    [SerializeField] int _id = 0;     // 識別番号
    [SerializeField] float _speed = 1f;     // 速さ
    [SerializeField] float _speedDiameter = 5f;     // 速さの倍率
    [SerializeField] float _animSpeed = 1f;     // アニメーションの速さ
    [SerializeField] float _threatRange = 1f;     // 音の感知範囲
    [SerializeField] float _fieldOfView = 120f;     // 視野角
    [SerializeField] float _viewLength = 10f;     // 視野の長さ

    //初期化
    public override void Init()
    {
        // 参照ステータスの初期化
        myInfo.id = _id;
        myInfo.speed = _speed;
        myInfo.speedDiameter = _speedDiameter; 
        myInfo.animSpeed = _animSpeed;
        myInfo.threatRange = _threatRange;
        myInfo.fieldOfView = _fieldOfView;
        myInfo.viewLength = _viewLength;
        myInfo.status.prediction = false;

        //ステート初期化
        seach = _seach;
        vigilance = _vigilance;
        tracking = _tracking;

        seachSkill = _seachSkill;
        vigilanceSkill = _vigilanceSkill;
        trackingSkill = _trackingSkill;

        enemyState = _seach;
        skill = _seachSkill;
    }

}
