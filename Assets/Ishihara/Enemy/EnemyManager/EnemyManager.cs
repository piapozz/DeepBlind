using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; } = null;

    [SerializeField]
    private GameObject[] _enemies;              // プレハブ一覧

    [SerializeField]
    private int[] _createEnemies;               // 生成するエネミーの数と種類

    [SerializeField]
    private Player _player;                     // プレイヤー

    [SerializeField]
    private GenerateStage _generateStage;       // ステージの情報

    List<EnemyBase> enemyList = new List<EnemyBase>();  // 管理する敵リスト

    void Start()
    {
        Instance = this;

        // エネミーの生成
        CreateEnemy();

        // 非同期ループを開始
        StartEnemyBehaviorLoop().Forget();
    }

    /// <summary>
    /// 非同期のエネミー管理ループ
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid StartEnemyBehaviorLoop()
    {
        while (true)
        {
            // エネミーの情報を更新
            UpdateEnemyData();

            // エネミーとプレイヤーが接触しているかを確認
            if (HittingDecision())
            {
                Debug.Log("終了");
                break;
            }

            // 次のフレームまで待機
            await UniTask.Yield();
        }
    }

    /// <summary>
    /// プレイヤーの予測ルートを取得する
    /// </summary>
    /// <returns></returns>
    public List<EnemyBase.ViaSeachData> SpecifiedExpectedPosition()
    {
        // 目標位置を再設定
        return _generateStage.GetPredictionPlayerPos(_player.GetPosition(), _player.GetMoveVec());
    }

    /// <summary>
    /// 探索状態のエネミーの目標位置を振り分ける
    /// </summary>
    public Vector3 DispatchTargetPosition()
    {
        // 目標位置を再設定
        return _generateStage.GetRandRoomPos();
    }

    /// <summary>
    /// エネミーの情報を更新
    /// </summary>
    private void UpdateEnemyData()
    {
        EnemyBase.PlayerStatus playerStatus;

        // プレイヤーから情報を取得
        playerStatus.cam = _player.GetCamera();
        playerStatus.playerPos = _player.GetPosition();

        // 各エネミーにプレイヤーの情報を渡す
        foreach (var enemy in enemyList)
        {
            enemy.SetPlayerStatus(playerStatus);
        }
    }

    /// <summary>
    /// エネミーの生成
    /// </summary>
    private void CreateEnemy()
    {
        for (int i = 0; i < _enemies.Length; i++)
        {
            for (int j = 0; j < _createEnemies[i]; j++)
            {
                // 生成
                var enemy = Instantiate(_enemies[i], _generateStage.GetRandCorridorPos(), Quaternion.identity, this.transform);

                // リストに追加
                enemyList.Add(enemy.GetComponent<EnemyBase>());
            }
        }
    }

    /// <summary>
    /// エネミーとプレイヤーが接触しているか調べる
    /// </summary>
    /// <returns></returns>
    private bool HittingDecision()
    {
        foreach (var enemy in enemyList)
        {
            if (enemy.CheckCaught())
            {
                return true;
            }
        }
        return false;
    }
}
