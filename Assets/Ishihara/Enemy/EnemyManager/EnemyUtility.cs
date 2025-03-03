using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUtility : MonoBehaviour
{
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

    public static async UniTask DeadCharacter(EnemyBase character)
    {
        // エネミー死亡の処理
        EnemyManager.instance.UnuseEnemy(character);
        
        await UniTask.CompletedTask;
    }
}
