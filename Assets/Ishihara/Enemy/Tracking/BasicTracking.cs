using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicTracking : ITracking
{
    private int _ID = -1;
    private EnemyBase _enemy;
    private Player _player;

    private bool _IsTargetlost = false;
    private float _lostTime = 0;

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
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init(int setID)
    {
        _ID = setID;
        _enemy = EnemyUtility.GetCharacter(_ID);
        _player = EnemyUtility.GetPlayer();
        _IsTargetlost = false;
        _lostTime = 0;
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

            // 見失っていたら
            if (tag != "Player")
            {
                if (!_IsTargetlost) _lostTime = 0.0f;
                _IsTargetlost = true;
                //_enemy.StateChange(State.SEARCH);
            }
            else
            {
                _IsTargetlost = false;
            }
        }

        if (!_IsTargetlost) return;

        // 距離が一定以下なら警戒
        // 秒数が一定以上になったら警戒
        _lostTime += Time.deltaTime;
        if(_lostTime > 10.0f)
        {
            _enemy.StateChange(State.VIGILANCE);
            return;
        }


        Vector3 start = _enemy.transform.position;
        Vector3 end = Player.instance.transform.position;
        start.y = 0;
        end.y = 0;

        float length = Vector3.Distance(start, end);
        if (length > _enemy.viewLength + 10)
        {
            _enemy.StateChange(State.VIGILANCE);
            return;
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
}
