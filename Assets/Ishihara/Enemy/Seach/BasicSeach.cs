using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;
using static UnityEngine.Rendering.HableCurve;
using static UnityEngine.UI.GridLayoutGroup;

public class BasicSeach : ISeach
{
    //private EnemyInfo _enemyInfo = new EnemyInfo();
    
    private bool _tracking;              // 見つけた

    /// <summary>
    /// 行動
    /// </summary>
    /// <param name="info"></param>
    /// <param name="skill"></param>
    /// <returns></returns>
    public void Activity()
    {
        // 取得
        GetTarget();

        // 見つけたかどうか
        CheckTracking();

        // 警戒条件を満たしたかどうか
        CheckVigilance();

        // 特殊処理
        //_enemyInfo = skill.Ability(_enemyInfo);

        // 仮目標地点にたどり着いたかどうか
        CheckReaching();

        // 更新
        StatusUpdate();

        //return _enemyInfo;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {

    }

    /// <summary>
    /// 見つけたかどうか
    /// </summary>
    public void CheckTracking()
    {
        //RaycastHit hit;

        //// プレイヤーとの間に障害物があるかどうか
        //Vector3 origin = _enemyInfo.status.position;                                              // 原点
        //Vector3 direction = Vector3.Normalize(_enemyInfo.playerStatus.playerPos - _enemyInfo.status.position);     // X軸方向を表すベクトル
        //Ray ray = new Ray(origin, direction);                                                    // Rayを生成;

        //if (Physics.Raycast(ray, out hit, _enemyInfo.pram.viewLength + 1 , 1)) 
        //{
        //    string tag = hit.collider.gameObject.tag;                                            // 衝突した相手オブジェクトの名前を取得
            
        //    if(tag != "Player") return;                                                          // プレイヤー以外なら終わる

        //    float toPlayerAngle = Mathf.Atan2(_enemyInfo.playerStatus.playerPos.z - _enemyInfo.status.position.z,
        //                           _enemyInfo.playerStatus.playerPos.x - _enemyInfo.status.position.x) * Mathf.Rad2Deg;
        //    float myAngle = Mathf.Atan2(_enemyInfo.status.dir.z, _enemyInfo.status.dir.x) * Mathf.Rad2Deg;

        //    // 0 ~ 360にクランプ
        //    toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
        //    myAngle = Mathf.Repeat(myAngle, 360);

        //    // 視野範囲内なら
        //    if (myAngle + (_enemyInfo.pram.fieldOfView / 2) > toPlayerAngle &&
        //        myAngle - (_enemyInfo.pram.fieldOfView / 2) < toPlayerAngle)
        //    {
        //        // 見つけた
        //        _tracking = true;
        //    }
        //}
    }

    // 仮目標地点にたどり着いたかどうか
    private void CheckReaching()
    { 
        //if((Vector3.Distance(_enemyInfo.status.targetPos, _enemyInfo.status.position) < 2.0f) && (!_enemyInfo.status.isAblity))
        //{
        //    // 探索箇所をランダムに設定する
        //    _enemyInfo.status.targetPos = EnemyManager.Instance.DispatchTargetPosition();
        //}
    }

    // 警戒条件を満たしたかどうか
    public void CheckVigilance()
    {

    }

    /// <summary>
    /// 目標位置の取得
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget()
    {
        // ターゲットの情報取得
        //_enemyInfo = info; 
    }

    /// <summary>
    /// 情報の更新
    /// </summary>
    /// <param name="info"></param>
    public void StatusUpdate()
    {
        // ステートの切り替え
        //if (_tracking) _enemyInfo.status.state = State.TRACKING;
    }

}
