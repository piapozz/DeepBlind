using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemyManager : MonoBehaviour
{


    [SerializeField] int managementEnemy;               // 一度に管理するエネミーの数
    [SerializeField] GameObject[] enemies;              // プレハブ一覧
    [SerializeField] int[] createEnemies;               // 生成するエネミーの数と種類
    [SerializeField] Player player;                     // プレイヤー
    [SerializeField] GenerateStage generateStage;       // ステージの情報

    EnemyBase.PlayerStatus playerStatus;                // 渡すプレイヤーのデータ構造体

    List<EnemyBase> enemyList = new List<EnemyBase>();  // 管理する敵リスト

    void Start()
    {
        // エネミーの生成
        CreateEnemy();

        // エネミーに情報を渡す
        SetEnemyStatus();
    }

    void Update()
    {
        // 探索状態のエネミーの目標位置を振り分ける
        DispatchTargetPosition();

        // 警戒状態のエネミーに探索部屋を割り当てる
        SpecifiedExpectedPosition();

        // エネミーの情報を更新
        UpdateEnemyData();

        // 音の管理


        // 接触判定管理
        HittingDecision();
    }

    // 警戒状態のエネミーに探索部屋を割り当てる
    void SpecifiedExpectedPosition()
    {
        // エネミーの数だけ繰り返す
        for (int i = 0; i < enemyList.Count; i++)
        {
            // 警戒状態かどうか
            if (enemyList[i].GetNowState() != EnemyBase.State.VIGILANCE) continue;

            // 推測するかどうか
            if (!enemyList[i].CheckPrediction()) continue;

            // 目標位置を再設定
            enemyList[i].SetViaSeachData(generateStage.GetPredictionPlayerPos(enemyList[i].GetLostPos(), enemyList[i].GetLostMoveVec()));
        }
    }

    // 探索状態のエネミーの目標位置を振り分ける
    private void DispatchTargetPosition()
    {
        // エネミーの数だけ繰り返す
        for (int i = 0; i < enemyList.Count; i++)
        {

            // 探索状態かどうか
            if (enemyList[i].GetNowState() != EnemyBase.State.SEARCH) continue;

            // 到達しているかどうか
            if (!enemyList[i].CheckReachingPosition()) continue;


            // 目標位置を再設定
            enemyList[i].SetTargetPos(generateStage.GetRandRoomPos());
        }
    }

    // エネミーの情報を更新
    private void UpdateEnemyData()
    {
        // プレイヤーから情報をもらう
        playerStatus.cam = player.GetCamera();
        playerStatus.playerPos = player.GetPosition();
        playerStatus.moveValue = player.GetMoveVec();

        // エネミーの数だけ繰り返す
        for (int i = 0; i < enemyList.Count; i++)
        {
            // プレイヤーの情報更新
            enemyList[i].SetPlayerStatus(playerStatus);
        }
    }

    // エネミーの生成
    private void CreateEnemy()
    {
        // 繰り返す
        for(int i = 0; i < enemies.Length; i++)
        {
            for (int j = 0; j < createEnemies[i]; j++)
            {
                // 生成
                var enemy = Instantiate(enemies[i], generateStage.GetRandCorridorPos(), Quaternion.identity);

                // リストに追加
                enemyList.Add(enemy.GetComponent<EnemyBase>());
            }
        }
    }

    // エネミーに情報を渡す
    private void SetEnemyStatus()
    {
        // プレイヤーから情報をもらう
        playerStatus.cam = player.GetCamera();
        playerStatus.playerPos = player.GetPosition();
        playerStatus.moveValue = player.GetMoveVec();

        // エネミーの数だけ繰り返す
        for(int i = 0;i < enemyList.Count; i++)
        {
            // プレイヤーの情報更新
            enemyList[i].SetPlayerStatus(playerStatus);

            enemyList[i].SetTargetPos(generateStage.GetRandRoomPos());
        }
    }

    // エネミーとプレイヤーが接触しているか調べる
    private void HittingDecision()
    {
        bool gameEnd = false;

        // エネミーの数だけ繰り返す
        for (int i = 0; i < enemyList.Count; i++)
        {
            // エネミーの接触判定を確認
            gameEnd = enemyList[i].CheckCaught();
        }

        // ゲーム終了処理
        if(gameEnd) { Debug.Log("終了"); }
    }
}
