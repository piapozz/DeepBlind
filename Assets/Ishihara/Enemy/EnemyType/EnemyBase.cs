using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting.APIUpdating;
using System;
using System.Windows.Input;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;


// スモーク
// ドア固め
// ロッカーワープ

// エネミーの元となる親クラス

public abstract class EnemyBase : MonoBehaviour
{
    // 情報構造体 (参照用)
    public struct EnemyInfo
    {
        public int id;                      // エネミーの識別番号
        public float speed;                 // エネミーの速さ
        public float speedDiameter;         // 見つけた時の速さの倍率
        public float animSpeed;             // アニメーションの速さ
        public float threatRange;           // 脅威範囲
        public float viewLength;            // 視界の長さ
        public float fieldOfView;           // 視野角
        public Animator animator;           // アニメーター
        public EnemyStatus status;          // ステータス
        public PlayerStatus playerStatus;   // プレイヤーのステータス
    }

    // ステータス構造体 (操作用)
    public struct EnemyStatus
    {
        public float nowSpeed;              // エネミーの速さ
        public Vector3 position;            // 現在位置
        public Vector3 targetPos;           // 目標位置
        public Vector3 lostPos;             // 見失った位置
        public bool isTargetLost;           // 見失っているかどうか
        public Vector3 dir;                 // 進行方向
        public State state;                 // 現在のステート
        public bool isAblity;               // アビリティ中
        public Vector3 lostMoveVec;         // 見失った時のプレイヤーの移動量
        public bool prediction;            // 推測
        public List<ViaSeachData> viaData;  // 経由探索用データ
    }

    // 部屋の経由探索用の情報構造体
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
        public Vector3 moveValue;   // プレイヤーの移動量
    }

    // ステート
    public enum State
    {
        SEARCH,        // 探索
        VIGILANCE,     // 警戒
        TRACKING,      // 追跡

        MAX
    }

    // アニメーション
    public enum BoolAnimation
    {
        WALKING,        // 歩行
        RUNNING,        // 走行
        SKILL,          // スキル使用時

        MAX
    }

    // アニメーション
    public enum TriggerAnimation
    {
        SCREAM,         // 発見
        LOOKING,        // 見渡し

        MAX
    }

    [SerializeField] GameObject meshObject;     // メッシュ

    protected EnemyInfo myInfo;   // ステータス
    private State oldState;               // 一つ前のステート
    protected IEnemyState enemyState;       // ステートクラス
    protected ISkill skill;                 // スキル
    private NavMeshAgent enemyAgent;       // ナビメッシュ

    // それぞれのステートクラス
    protected ISeach seach;
    protected IVigilance vigilance;
    protected ITracking tracking;

    // それぞれのスキルクラス
    protected ISkill seachSkill;
    protected ISkill vigilanceSkill;
    protected ISkill trackingSkill;

    protected string[] boolAnimation;       // boolパラメーター
    protected string[] triggerAnimation;    // triggerパラメーター

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

    // 初期化
    private void BaseInit()
    {
        // ステート初期化
        oldState = myInfo.status.state = State.SEARCH;

        // ナビメッシュ取得
        enemyAgent = GetComponent<NavMeshAgent>();

        // アニメーター取得
        myInfo.animator = GetComponent<Animator>();


        myInfo.status.position = this.transform.position;
        myInfo.status.nowSpeed = myInfo.speed;
        myInfo.status.prediction = false;
        myInfo.status.viaData = new List<ViaSeachData>();


    }

    // ステートの切り替え処理
    private void StateSwitching()
    {
        // 切り替わっていたら
        if (oldState == myInfo.status.state) return;

        // 更新
        oldState = myInfo.status.state;

        // 切り替え処理
        StateChange(myInfo.status.state);
    }

    // 行動
    public void Active()
    {
        // 行動(情報を渡して更新)
        myInfo = enemyState.Activity(myInfo, skill);
    }

    // ナビメッシュで移動する
    private void MoveNavAgent()
    {
        if (myInfo.status.isAblity)
        {
            enemyAgent.velocity = Vector3.zero;
            enemyAgent.angularSpeed = 0;
            return;
        }

        enemyAgent.angularSpeed = 360;

        // 以下を追加
        if (Vector3.Distance(enemyAgent.steeringTarget, myInfo.status.position) < 1.0f)
        {
            enemyAgent.speed = myInfo.status.nowSpeed / 2;
        }
        else
        {
            enemyAgent.speed = myInfo.status.nowSpeed;
        }

        enemyAgent.velocity = (enemyAgent.steeringTarget - myInfo.status.position).normalized * enemyAgent.speed;

        // 目標位置を設定
        enemyAgent.SetDestination(myInfo.status.targetPos);

        // 速度の変更
        // enemyAgent.speed = myInfo.status.nowSpeed;
        enemyAgent.acceleration = enemyAgent.speed * 8;

        // 向いている方向を取得
        myInfo.status.dir = Vector3.Normalize(enemyAgent.nextPosition - myInfo.status.position);

        // 現在の座標を取得
        myInfo.status.position = this.transform.position;
    }

    // 初期化
    public abstract void Init();

    // ステートとスキルの切り替え処理
    public void StateChange(State state)
    {
        // ステート、スキルを切り替える
        switch (myInfo.status.state)
        {
            case State.SEARCH:

                enemyState = seach;
                skill = seachSkill;

                myInfo.status.nowSpeed = myInfo.speed;

                break;

            case State.VIGILANCE:

                enemyState = vigilance;
                skill = vigilanceSkill;

                myInfo.status.nowSpeed = myInfo.speed * myInfo.speedDiameter / 2;

                break;

            case State.TRACKING:

                enemyState = tracking;
                skill = trackingSkill;

                // 発見アニメーションを流す


                myInfo.status.nowSpeed = myInfo.speed * myInfo.speedDiameter;

                break;
        }

        // ステートの初期化
        enemyState.Init();
    }

    // プレイヤーが逃げた場所を推測するかどうか
    public bool CheckPrediction() {return myInfo.status.prediction;}

    // 情報の更新
    public void SetEnemyInfo(EnemyInfo info) { myInfo = info; }

    // プレイヤー情報の更新
    public void SetPlayerStatus(PlayerStatus status) { myInfo.playerStatus = status; }

    // 目標位置の設定
    public void SetTargetPos(Vector3 pos) { myInfo.status.targetPos = pos; }

    // 目標位置の取得
    public Vector3 GetTargetPos() { return myInfo.status.targetPos; }

    // 現在のステート
    public State GetNowState() { return myInfo.status.state; }

    // 目標位置にたどり着いたかどうか
    public bool CheckReachingPosition() { return (Vector3.Distance(myInfo.status.targetPos, myInfo.status.position) < 2.0f) && (!myInfo.status.isAblity); }

    // 見失った地点の取得
    public Vector3 GetLostPos() { return myInfo.status.lostPos; }

    // 見失った時の移動量を取得
    public Vector3 GetLostMoveVec() {  return myInfo.status.lostMoveVec;}

    // 経由探索用の情報を設定
    public void SetViaSeachData(List<ViaSeachData> vias) { myInfo.status.viaData = vias; }
}