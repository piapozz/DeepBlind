using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEditor.Rendering;
using UnityEngine;
using static CommonModule;

public class EnemyManager : SystemObject
{
    public static EnemyManager instance { get; private set; } = null;

    [SerializeField]
    private GameObject[] _enemies;              // プレハブ一覧

    [SerializeField]
    private int[] _createEnemies;               // 生成するエネミーの数と種類

    [SerializeField]
    private Transform _useObjectRoot = null;

    [SerializeField]
    private Transform _unuseObjectRoot = null;

    // 使用中のキャラクターリスト
    private List<EnemyBase> _useList = null;
    // 未使用状態のエネミーリスト
    private List<EnemyBase> _unuseList = null;

    // 使用中のキャラクターオブジェクトリスト
    private List<GameObject> _useObjectList = null;
    // 未使用状態のキャラクターオブジェクトリスト
    private List<GameObject> _unuseObjectList = null;

    public override void Initialize()
    {
        instance = this;
        EnemyBase.SetGetObjectCallback(GetCharacterObject);

        int enemyMax = 0;
        for(int i = 0, max = _enemies.Length; i < max; i++)
        {
            enemyMax += _createEnemies[i];
        }

        // 必要なキャラクターとオブジェクトのインスタンスを生成して未使用状態にしておく
        _useList = new List<EnemyBase>(enemyMax);

        // エネミーの生成
        CreateEnemy();
        // 非同期ループを開始
        StartEnemyBehaviorLoop();
    }

    /// <summary>
    /// 非同期のエネミー管理ループ
    /// </summary>
    /// <returns></returns>
    private async UniTask StartEnemyBehaviorLoop()
    {
        while (true)
        {
            ExecuteAll(enemy => enemy.Active());
            ExecuteAll(enemy => enemy.MoveNavAgent());

            // エネミーとプレイヤーが接触しているかを確認
            
            {
               
            }

            // 次のフレームまで待機
            await UniTask.DelayFrame(1);
        }
    }

    /// <summary>
    /// 探索状態のエネミーの目標位置を振り分ける
    /// </summary>
    public Vector3 DispatchTargetPosition()
    {
        // 目標位置を再設定
        return GenerateStage.instance.GetRandRoomPos();
    }

    /// <summary>
    /// エネミーの生成
    /// </summary>
    private async UniTask CreateEnemy()
    {
        for (int i = 0; i < _enemies.Length; i++)
        {
            for (int j = 0; j < _createEnemies[i]; j++)
            {
                // 生成
                var enemy = Instantiate(_enemies[i], GenerateStage.instance.GetRandCorridorPos(), Quaternion.identity, this.transform);

                // IDの取得
                // インスタンスの取得
                EnemyBase useEnemy = enemy.GetComponent<EnemyBase>();
                
                // 使用可能なIDを取得して使用リストに追加
                int useID = UseCharacter(useEnemy);
                _useObjectList[useID] = enemy;
                enemy.transform.SetParent(_useObjectRoot);
                useEnemy.Setup(useID, squareData, masterID);

                Vector3 position = GenerateStage.instance.GetRandRoomPos();
                UseEnemy(position, 1);

                await UniTask.DelayFrame(1);
            }
        }
    }

    /// <summary>
	/// エネミーの生成
	/// </summary>
	/// <param name="squareData"></param>
	public void UseEnemy(Vector3 squareData, int masterID)
    {
        // インスタンスの取得
        EnemyBase useEnemy = null;
        useEnemy = _unuseList[0];
        _unuseList.RemoveAt(0);
        

        // 使用可能なIDを取得して使用リストに追加
        int useID = UseCharacter(useEnemy, masterID);
        useEnemy.Setup(useID, squareData, masterID);
    }

    private int UseCharacter(EnemyBase useCharacter, int masterID)
    {
        // 使用可能なIDを取得して使用リストに追加
        int useID = -1;
        for (int i = 0, max = _useList.Count; i < max; i++)
        {
            if (_useList[i] != null) continue;

            useID = i;
            _useList[i] = useCharacter;
            break;
        }
        if (useID < 0)
        {
            useID = _useList.Count;
            _useList.Add(useCharacter);
        }
        // オブジェクトの取得
        GameObject useObject = null;
        if (IsEmpty(_unuseObjectList))
        {
            Entity_EnemyData.Pram pram = CharacterMasterUtility.GetCharacterMaster(masterID);
            useObject = Instantiate(Resources.Load("Prefab/" + pram.Name));
        }
        else
        {
            useObject = _unuseObjectList[0];
            _unuseObjectList.RemoveAt(0);
        }
        // オブジェクトの使用リストへの追加
        while (!IsEnableIndex(_useObjectList, useID)) _useObjectList.Add(null);

        _useObjectList[useID] = useObject;
        useObject.transform.SetParent(_useObjectRoot);
        return useID;
    }

    public void UnuseEnemy(EnemyBase unuseEnemy)
    {
        if (unuseEnemy == null) return;

        int unuseID = unuseEnemy.ID;
        // 使用リストから取り除く
        if (IsEnableIndex(_useList, unuseID)) _useList[unuseID] = null;
        // 片付け処理を読んで未使用リストに加える
        unuseEnemy.Teardown();
        _unuseList.Add(unuseEnemy);
        // オブジェクトを未使用にする
        UnuseObject(unuseID);
    }

    private void UnuseObject(int unuseID)
    {
        GameObject unuseCharacterObject = GetCharacterObject(unuseID);
        // 使用リストから取り除く
        if (IsEnableIndex(_useObjectList, unuseID)) _useObjectList[unuseID] = null;
        // 見えない場所に置く
        unuseCharacterObject.transform.SetParent(_unuseObjectRoot);
        // 未使用リストに追加
        _unuseObjectList.Add(unuseCharacterObject);
    }

    private GameObject GetCharacterObject(int ID)
    {
        if (!IsEnableIndex(_useObjectList, ID)) return null;

        return _useObjectList[ID];
    }

    public EnemyBase Get(int ID)
    {
        if (!IsEnableIndex(_useList, ID)) return null;

        return _useList[ID];
    }

    public void ExecuteAll(System.Action<EnemyBase> action)
    {
        if (action == null || IsEmpty(_useList)) return;

        for (int i = 0, max = _useList.Count; i < max; i++)
        {
            if (_useList[i] == null) continue;
            
        }
    }

    public async UniTask ExecuteAllTask(System.Func<EnemyBase, UniTask> task)
    {
        if (task == null || IsEmpty(_useList)) return;

        for (int i = 0, max = _useList.Count; i < max; i++)
        {
            if (_useList[i] == null) continue;

            await task(_useList[i]);
        }
    }
}
