using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using static CommonModule;

public class EnemyManager : SystemObject
{
    public static EnemyManager instance { get; private set; } = null;

    [SerializeField]
    private Transform _useObjectRoot = null;

    [SerializeField]
    private Transform _unuseObjectRoot = null;

    // 使用中のキャラクターリスト
    private List<EnemyBase> _useList = null;

    // 使用中のキャラクターオブジェクトリスト
    private List<GameObject> _useObjectList = null;


    // 再生中のBGM
    BGM bgm;

    public override void Initialize()
    {
        MasterDataManager.LoadAllData();
        instance = this;
        EnemyBase.SetGetObjectCallback(GetCharacterObject);
        

        int enemyMax = MasterDataManager.enemyData[0].Count;

        // 必要なキャラクターとオブジェクトのインスタンスを生成して未使用状態にしておく
        _useList = new List<EnemyBase>(enemyMax);
        _useObjectList = new List<GameObject>(enemyMax);

        for (int i = 0; i < enemyMax; i++)
        {
            _useList.Add(new EnemyBase());
        }
        for (int i = 0; i < enemyMax; i++)
        {
            _useObjectList.Add(null);
        }
        AudioManager.instance.PlayBGM(BGM.MAIN_NORMAL);
        bgm = BGM.MAIN_NORMAL;

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

            CheckChangeBGM();

            // エネミーとプレイヤーが接触しているかを確認
            if(ExecuteAll(enemy =>
            {
                if (enemy.CheckCaughtPlayer())
                {
                    EnemyUtility.GetPlayer().EnemyCaught(enemy._camera, enemy);
                    enemy.SetNavTarget(enemy.transform.position);
                    enemy.SetAnimationSpeed(1);
                    return true;
                }
                return false;
            }))
            {
                break;
            }

            // 次のフレームまで待機
            await UniTask.DelayFrame(1);
        }
    }

    private void CheckChangeBGM()
    {
        EnemyBase.State state = EnemyBase.State.TRACKING;
        bool tracking = false;

        for(int i = 0, max = _useList.Count;i < max; i++)
        {
            if (_useList[i]._nowState == EnemyBase.State.TRACKING)
            {
                tracking = true;
            }
        }

        if (tracking && bgm == BGM.MAIN_NORMAL)
        {
            bgm = BGM.MAIN_TRACKING;
            AudioManager.instance.PlayBGM(bgm);
            return;
        }

        if (!tracking && bgm == BGM.MAIN_TRACKING)
        {
            bgm = BGM.MAIN_NORMAL;
            AudioManager.instance.PlayBGM(bgm);
        }
    }

    /// <summary>
    /// 探索状態のエネミーの目標位置を振り分ける
    /// </summary>
    public Vector3 DispatchTargetPosition()
    {
        // 目標位置を再設定
        return GenerateStage.instance.GetRandCorridorPos();
    }

    /// <summary>
    /// エネミーの生成
    /// </summary>
    private async UniTask CreateEnemy()
    {
        for (int i = 0; i < MasterDataManager.enemyData[0].Count; i++)
        {
            // 生成
            // 使用可能なIDを取得して使用リストに追加
            Entity_EnemyData.Param param = MasterDataManager.enemyData[0][i];
            Vector3 position = DispatchTargetPosition();
            UseEnemy(position, param.ID);

            await UniTask.DelayFrame(1);
        }
    }

    /// <summary>
	/// エネミーの生成
	/// </summary>
	/// <param name="squareData"></param>
	public void UseEnemy(Vector3 position, int masterID)
    {
        // インスタンスの取得
        // 使用可能なIDを取得して使用リストに追加
        int useID = UseCharacter(masterID);
        _useList[useID].Setup(useID, position, masterID);
    }

    private int UseCharacter(int masterID)
    {
        // 使用可能なIDを取得して使用リストに追加
        int useID = -1;
        for (int i = 0, max = _useList.Count; i < max; i++)
        {
            if (_useList[i] != null) continue;

            useID = i;
            break;
        }
        if (useID < 0)
        {
            useID = _useList.Count;
            _useList.Add(new EnemyBase());
        }
        // オブジェクトの取得
        GameObject useObject = null;
        Entity_EnemyData.Param param = CharacterMasterUtility.GetCharacterMaster(masterID);
        useObject = Instantiate((GameObject)Resources.Load("Prefab/" + param.Name));
        // オブジェクトの使用リストへの追加
        while (!IsEnableIndex(_useObjectList, useID)) _useObjectList.Add(null);

        _useList[useID] = useObject.AddComponent<EnemyBase>();
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

            action(_useList[i]);
        }
    }

    public bool ExecuteAll(System.Func<EnemyBase , bool> action)
    {
        if (action == null || IsEmpty(_useList)) return default;

        for (int i = 0, max = _useList.Count; i < max; i++)
        {
            if (_useList[i] == null) continue;

            if (action(_useList[i]))
            {
                return true;
            }
        }

        return default;
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
