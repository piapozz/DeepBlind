using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

// エネミーのユーティリティ
public class EnemyUtility : MonoBehaviour
{
    /// <summary>
    /// プレイヤー取得
    /// </summary>
    /// <returns></returns>
    public static Player GetPlayer()
    {
        return Player.instance;
    }

    /// <summary>
    /// キャラクターデータ取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static EnemyBase GetCharacter(int ID)
    {
        return EnemyManager.instance.Get(ID);
    }

    /// <summary>
    /// 全てのキャラクターに処理実行
    /// </summary>
    /// <param name="action"></param>
    public static void ExecuteAllCharacter(System.Action<EnemyBase> action)
    {
        EnemyManager.instance.ExecuteAll(action);
    }

    /// <summary>
    /// 全てのキャラクターにタスク実行
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public static async UniTask ExecuteTaskAllCharacter(System.Func<EnemyBase, UniTask> task)
    {
        await EnemyManager.instance.ExecuteAllTask(task);
    }

    /// <summary>
    /// キャラクターの死亡処理
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public static async UniTask DeadCharacter(EnemyBase character)
    {
        // エネミー死亡の処理
        EnemyManager.instance.UnuseEnemy(character);
        
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// エネミーからプレイヤーまでの距離を取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static float EnemyToPlayerLength(int ID)
    {
        EnemyBase enemy = GetCharacter(ID);
        Player player = GetPlayer();

        Vector3 start = enemy.transform.position;
        start.y = 0;
        Vector3 end = player.transform.position;
        end.y = 0;

        float length = Vector3.Distance(start, end);
        return length;
    }

    /// <summary>
    /// エネミーからプレイヤーが確認できるかの関数
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="locker">ロッカーを無視するかどうか</param>
    /// <returns></returns>
    public static bool CheckViewPlayer(int ID, bool ignoreLocker = true)
    {
        bool isHit = false;

        EnemyBase enemy = GetCharacter(ID);
        Player player = GetPlayer();

        if (enemy == null || player == null) return false;
        if (player.isLocker && !ignoreLocker) return false;

        // プレイヤーとの間に障害物があるかどうか
        Vector3 origin = enemy.transform.position;                                                              // 原点
        Vector3 direction = Vector3.Normalize(player.transform.position - enemy.transform.position);     // X軸方向を表すベクトル
        Ray ray = new Ray(origin, direction);                                                                    // Rayを生成;

        RaycastHit hit;
        LayerMask layer = LayerMask.GetMask("Player") | LayerMask.GetMask("Stage");
        isHit = Physics.Raycast(ray, out hit, Vector3.Distance(enemy.transform.position, player.transform.position), layer);

        return isHit && hit.collider.tag == "Player";
    }

    /// <summary>
    /// アンカーが残っているか確認する
    /// 残っていたら次のアンカーを返す
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static bool CheckSearchAnchor(int ID)
    {
        EnemyBase enemy = GetCharacter(ID);
        if (enemy == null) return false;
        if (enemy.searchAnchorList.Count < 2) return false;
        enemy.searchAnchorList.RemoveAt(0);
        enemy.SetNavTarget(enemy.searchAnchorList[0].position);

        return true;
    }

    /// <summary>
    /// 次のアンカーを設定する
    /// </summary>
    /// <param name="ID"></param>
    public static void SetSearchAnchor(int ID)
    {
        EnemyBase enemy = GetCharacter(ID);
        if (enemy == null) return;
        enemy.SetSearchAnchor(StageManager.instance.GetRandomEnemyAnchor());
        enemy.SetNavTarget(enemy.searchAnchorList[0].position);
    }
}
