using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting.APIUpdating;
using System;
using System.Windows.Input;

// エネミーの元となる親クラス

public abstract class EnemyBase : MonoBehaviour
{
    // 情報構造体 (参照用)
    public struct EnemyInfo
    {
        public int id;                      // エネミーの識別番号
        public float spped;                 // エネミーの速さ
        public float threatRange;           // 脅威範囲
        public float viewLength;            // 視界の長さ
        public float fieldOfView;           // 視野角
        public Animator animator;           // アニメーター
        public Bounds bounds;               // 自分の形
        public EnemyStatus status;          // ステータス
        public PlayerStatus playerStatus;   // プレイヤーのステータス
    }

    // ステータス構造体 (操作用)
    public struct EnemyStatus
    {
        public Vector3 position;    // 現在位置
        public Vector3 targetPos;   // 目標位置
        public Vector3 lostPos;     // 見失った位置
        public bool isTargetLost;   // 見失っているかどうか
        public Vector3 dir;         // 進行方向
        public State state;         // 現在のステート
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
        SEACH,         // 探索
        VIGILANCE,     // 警戒
        TRACKING,      // 追跡

        MAX
    }

    protected EnemyInfo myInfo;   // ステータス
    private State oldState;               // 一つ前のステート
    protected IEnemyState enemyState;       // ステートクラス
    private NavMeshAgent enemyAgent;       // ナビメッシュ

    // それぞれのステートクラス
    protected ISeach seach;
    protected IVigilance vigilance;
    protected ITracking tracking;

    void Start()
    {
        // 初期化
        BaseInit();

        // 初期化
        Init();
    }

    void Update()
    {
        // ステートの切り替え処理
        StateSwitching();

        // 行動
        Active();

        // ナビメッシュの移動
        MoveNavAgent();
    }

    // 初期化
    private void BaseInit()
    {
        // ステート初期化
        oldState = myInfo.status.state = State.SEACH;

        // ナビメッシュ取得
        enemyAgent = GetComponent<NavMeshAgent>();

        // アニメーター取得
        myInfo.animator = GetComponent<Animator>();

        // 自身の形を取得
        myInfo.bounds = GetBounds(this.gameObject, new Bounds());

    }

    // 自身の形を取得
    private Bounds GetBounds(GameObject obj, Bounds bounds) 
    {
        // 全ての子オブジェクトをチェックする
        foreach (Transform child in transform)
        {
            // メッシュフィルターの存在確認
            MeshFilter filter = child.gameObject.GetComponent<MeshFilter>();

            if (filter != null)
            {
                // オブジェクトのワールド座標とサイズを取得する
                Vector3 ObjWorldPosition = child.position;
                Vector3 ObjWorldScale = child.lossyScale;

                // フィルターのメッシュ情報からバウンドボックスを取得する
                Bounds meshBounds = filter.mesh.bounds;

                // バウンドのワールド座標とサイズを取得する
                Vector3 meshBoundsWorldCenter = meshBounds.center + ObjWorldPosition;
                Vector3 meshBoundsWorldSize = Vector3.Scale(meshBounds.size, ObjWorldScale);

                // バウンドの最小座標と最大座標を取得する
                Vector3 meshBoundsWorldMin = meshBoundsWorldCenter - (meshBoundsWorldSize / 2);
                Vector3 meshBoundsWorldMax = meshBoundsWorldCenter + (meshBoundsWorldSize / 2);

                // 取得した最小座標と最大座標を含むように拡大/縮小を行う
                if (bounds.size == Vector3.zero)
                {
                    // 元バウンドのサイズがゼロの場合はバウンドを作り直す
                    bounds = new Bounds(meshBoundsWorldCenter, Vector3.zero);
                }
                bounds.Encapsulate(meshBoundsWorldMin);
                bounds.Encapsulate(meshBoundsWorldMax);
            }

            // 再帰処理
            bounds = GetBounds(child.gameObject, bounds);
        }
        return bounds;
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
        myInfo = enemyState.Activity(myInfo);
    }

    // ナビメッシュで移動する
    private void MoveNavAgent()
    {
        // 目標位置を設定
        enemyAgent.SetDestination(myInfo.status.targetPos);

        // 速度の変更
        enemyAgent.speed = myInfo.spped;

        // 現在の座標を取得
        myInfo.status.position = enemyAgent.nextPosition;
    }

    // 初期化
    public abstract void Init();

    // ステートの切り替え処理
    public void StateChange(State state)
    {
        // ステートを切り替える
        switch (myInfo.status.state)
        {
            case State.SEACH:

                enemyState = (ISeach)seach;

                break;

            case State.VIGILANCE:

                enemyState = (IVigilance)vigilance;

                break;

            case State.TRACKING:

                enemyState = (ITracking)tracking;

                break;
        }

        // ステートの初期化
        enemyState.Init();
    }

    // 情報の更新
    public void SetEnemyInfo(EnemyInfo info) { myInfo = info; }

    // プレイヤー情報の更新
    public void SetPlayerStatus(PlayerStatus status) { myInfo.playerStatus = status; }

    // 目標位置の設定
    public void SetTargetPos(Vector3 pos) { myInfo.status.targetPos = pos; }
}
