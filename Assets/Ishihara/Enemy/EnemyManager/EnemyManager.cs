using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] int managementEnemy;   // 一度に管理するエネミーの数

    [SerializeField] GameObject[] enemies;  // プレハブ一覧

    [SerializeField] int[] createEnemies;   // 生成するエネミーの数と種類

    [SerializeField] Player player;     // プレイヤー

    [SerializeField] GenerateStage generateStage;
    
    // マップの情報

    EnemyBase.PlayerStatus playerStatus;     // 渡すプレイヤーのデータ

    List<EnemyBase> enemyList = new List<EnemyBase>();

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

        // 音の管理

        // 接触判定管理

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
                var enemy = Instantiate(enemies[i], generateStage.GetRandRoomPos(), Quaternion.identity);

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

        // エネミーの数だけ繰り返す
        for(int i = 0;i < enemyList.Count; i++)
        {
            // プレイヤーの情報更新
            enemyList[i].SetPlayerStatus(playerStatus);
        }
    }


}
