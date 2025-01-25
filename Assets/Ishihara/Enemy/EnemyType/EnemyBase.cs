using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// スモーク
// ドア固め
// ロッカーワープ

// エネミーの元となる親クラス

public abstract class EnemyBase : MonoBehaviour
{
    // 情報構造体 (参照用)
    public struct EnemyInfo
    {
        public EnemyPram pram;              // 基本データ
        public EnemyStatus status;          // ステータス
        public PlayerStatus playerStatus;   // プレイヤーのステータス
    }

    // ステータス構造体 (操作用)
    public struct EnemyStatus
    {
        public float nowSpeed;              // エネミーの現在の速さ
        public Vector3 position;            // 現在位置
        public Vector3 targetPos;           // 目標位置
        public Vector3 dir;                 // 進行方向
        public State state;                 // 現在のステート
        public bool isAblity;               // アビリティ中
    }

    /// 部屋の経由探索用の情報構造体
    public struct ViaSeachData
    {
        public Vector3 viaPosition;        // 経由地点の座標
        public bool room;                  // 経由地点が部屋かどうか
    }

    // プレイヤーからもらう情報
    public struct PlayerStatus
    {
        public Camera cam;          // カメラ
        public Vector3 playerPos;   // プレイヤーの位置
    }

    // ステート
    public enum State
    {
        SEARCH,        // 探索
        VIGILANCE,     // 警戒
        TRACKING,      // 追跡

        MAX
    }

    protected EnemyInfo myInfo;             // ステータス
    protected IEnemyState enemyState;       // ステートクラス
    protected ISkill skill;                 // スキル
    private State _oldState;                 // 一つ前のステート
    private NavMeshAgent _enemyAgent;        // ナビメッシュ
    private Animator _animator;             // アニメーター

    // それぞれのステートクラス
    protected ISeach seach;
    protected IVigilance vigilance;
    protected ITracking tracking;

    // それぞれのスキルクラス
    protected ISkill seachSkill;
    protected ISkill vigilanceSkill;
    protected ISkill trackingSkill;

    bool caught = false;

    void Start()
    {
        // 初期化
        Init();

        // 初期化
        BaseInit();
    }

    void Update()
    {
        // ナビメッシュの移動
        MoveNavAgent();

        // 行動
        Active();

        // ステートの切り替え処理
        StateSwitching();

        // アニメーションの更新

    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void BaseInit()
    {
        // ステート初期化
        _oldState = myInfo.status.state = State.SEARCH;

        // ナビメッシュ取得
        _enemyAgent = GetComponent<NavMeshAgent>();

        // アニメーターの取得
        _animator = GetComponent<Animator>();

        // 参照データの初期化
        myInfo.status.position = this.transform.position;
        myInfo.status.nowSpeed = myInfo.pram.speed;
    }

    /// <summary>
    /// ステートの切り替え処理
    /// </summary>
    private void StateSwitching()
    {
        // 切り替わっていたら
        if (_oldState == myInfo.status.state) return;

        // 更新
        _oldState = myInfo.status.state;

        // 切り替え処理
        StateChange(myInfo.status.state);
    }

    /// <summary>
    /// 行動
    /// </summary>
    public void Active()
    {
        // 行動(情報を渡して更新)
        myInfo = enemyState.Activity(myInfo, skill);
    }

    /// <summary>
    /// ナビメッシュで移動する
    /// </summary>
    private void MoveNavAgent()
    {
        // スキル使用中なら移動を停止
        if (myInfo.status.isAblity)
        {
            _enemyAgent.velocity = Vector3.zero;
            return;
        }
        
        // エネミーが目標に対して接近すると少し減速するようにする
        if (Vector3.Distance(_enemyAgent.steeringTarget, myInfo.status.position) < 1.0f)
        {
            _enemyAgent.speed = myInfo.status.nowSpeed / 2;
        }
        else
        {
            _enemyAgent.speed = myInfo.status.nowSpeed;
        }

        _enemyAgent.velocity = (_enemyAgent.steeringTarget - myInfo.status.position).normalized * _enemyAgent.speed;

        // 目標位置を設定
        _enemyAgent.SetDestination(myInfo.status.targetPos);

        // 速度の変更
        _enemyAgent.acceleration = _enemyAgent.speed * 8;

        // 向いている方向を取得
        myInfo.status.dir = Vector3.Normalize(_enemyAgent.nextPosition - myInfo.status.position);

        // 現在の座標を取得
        myInfo.status.position = this.transform.position;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// ステートとスキルの切り替え処理
    /// </summary>
    /// <param name="state"></param>
    public void StateChange(State state)
    {
        // ステート、スキルを切り替える
        switch (myInfo.status.state)
        {
            case State.SEARCH:

                enemyState = seach;
                skill = seachSkill;

                myInfo.status.nowSpeed = myInfo.pram.speed;

                break;

            case State.VIGILANCE:

                enemyState = vigilance;
                skill = vigilanceSkill;

                myInfo.status.nowSpeed = myInfo.pram.speed * myInfo.pram.speedDiameter / 2;

                break;

            case State.TRACKING:

                enemyState = tracking;
                skill = trackingSkill;

                myInfo.status.nowSpeed = myInfo.pram.speed * myInfo.pram.speedDiameter;

                break;
        }

        // ステート、スキルの初期化
        skill.Init(_animator);
        enemyState.Init();
    }

    /// <summary>
    /// コリジョンがプレイヤーに接触していたら接触フラグを倒す
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            caught = true;
        }
    }

    /// <summary>
    /// プレイヤー情報の更新
    /// </summary>
    /// <param name="status"></param>
    public void SetPlayerStatus(PlayerStatus status) { myInfo.playerStatus = status; }

    /// <summary>
    /// プレイヤーを捕まえたかどうか
    /// </summary>
    /// <returns></returns>
    public bool CheckCaught() { return caught; }
}