using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicTracking : ITracking
{
    private int _ID = -1;
    private EnemyBase _enemy;
    private Player _player;

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

        // 見失ったかどうか
        CheckTargetLost();

        // 更新
        StatusUpdate();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(int setID)
    {
        _ID = setID;
        _enemy = EnemyUtility.GetCharacter(_ID);
        _player = EnemyUtility.GetPlayer();
    }

    /// <summary>
    /// 見失ったかどうか
    /// </summary>
    public void CheckTargetLost()
    {
        // プレイヤーとの間に障害物があるかどうか
        Vector3 origin = _enemy.transform.position;                                                              // 原点
        Vector3 direction = Vector3.Normalize(_player.transform.position - _enemy.transform.position);     // X軸方向を表すベクトル
        Ray ray = new Ray(origin, direction);                                                                    // Rayを生成;

        RaycastHit hit;
        LayerMask layer = LayerMask.GetMask("Player") | LayerMask.GetMask("Stage");
        if (Physics.Raycast(ray, out hit, Vector3.Distance(_enemy.transform.position, _player.transform.position), layer))                                                 // もしRayを投射して何らかのコライダーに衝突したら
        {
            string tag = hit.collider.gameObject.tag;                                                            // 衝突した相手オブジェクトの名前を取得

            // 初めて見失っていたら
            if (tag != "Player")
            {
                _enemy.StateChange(State.SEARCH);
            }
        }
    }

    /// <summary>
    /// 目標位置の取得
    /// </summary>
    /// <param name="info"></param>
    public void GetTarget()
    {
        _enemy.SetNavTarget(_player.transform.position);
    }

    /// <summary>
    /// 情報の更新
    /// </summary>
    public void StatusUpdate()
    {

    }
}
