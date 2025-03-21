using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static CommonModule;

// エネミーの管理、生成

public class EnemyManager : SystemObject
{
    public static EnemyManager instance { get; private set; } = null;

    [SerializeField]
    private Transform _useObjectRoot = null;                            // 有効オブジェクト

    [SerializeField]
    private Transform _unuseObjectRoot = null;                          // 無効オブジェクト

    [SerializeField]
    EnemyPrefabs enemyOrigin;                                           // プレハブ一覧

    private List<EnemyBase> _useList = new List<EnemyBase>();           // 使用中リスト
    private List<GameObject> _useObjectList = new List<GameObject>();   // 使用中オブジェクトリスト

    private BGM bgm;                                                    // 再生中のBGM

    public override async void Initialize()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;

        // マスターデータの読み込み
        MasterDataManager.LoadAllData();
        // コールバックの設定
        EnemyBase.SetGetObjectCallback(GetCharacterObject);
        // リストの初期化
        int enemyMax = MasterDataManager.enemyData[0].Count;
        _useList.Capacity = enemyMax;
        _useObjectList.Capacity = enemyMax;
        // BGMの再生
        AudioManager.instance.PlayBGM(BGM.MAIN_NORMAL);
        bgm = BGM.MAIN_NORMAL;
        // エネミーの生成
        await CreateEnemy();
        StartEnemyBehaviorLoop().Forget();
    }

    /// <summary>
    /// エネミーの行動ループ
    /// </summary>
    /// <returns></returns>
    private async UniTask StartEnemyBehaviorLoop()
    {
        while (true)
        {
            // エネミーの行動
            ExecuteAll(enemy => enemy.Active());
            CheckChangeBGM();

            // プレイヤーの捕獲判定
            if (ExecuteAll(enemy => {
                if (enemy.CheckCaughtPlayer())
                {
                    // プレイヤー捕獲
                    Player.instance.EnemyCaught(enemy._camera);
                    enemy.CaughtPlayer();
                    return true;
                }

                return false;
            }))
            {
                // ループ終了
                break;
            }
            await UniTask.DelayFrame(1);
        }
    }

    /// <summary>
    /// BGMの変更
    /// </summary>
    private void CheckChangeBGM()
    {
        bool tracking = _useList.Exists(enemy => enemy._nowState == EnemyBase.State.TRACKING);

        if (tracking && bgm == BGM.MAIN_NORMAL)
        {
            bgm = BGM.MAIN_TRACKING;
            AudioManager.instance.PlayBGM(bgm);
        }
        else if (!tracking && bgm == BGM.MAIN_TRACKING)
        {
            bgm = BGM.MAIN_NORMAL;
            AudioManager.instance.PlayBGM(bgm);
        }
    }

    /// <summary>
    /// ランダムなマップの座標を取得
    /// </summary>
    /// <returns></returns>
    public Vector3 DispatchTargetPosition()
    {
        List<Transform> anchorList = StageManager.instance.GetRandomEnemyAnchor();
        return anchorList[0].position;
    }

    /// <summary>
    /// エネミーの生成
    /// </summary>
    /// <returns></returns>
    private async UniTask CreateEnemy()
    {
        foreach (var param in MasterDataManager.enemyData[0])
        {
            Vector3 position = DispatchTargetPosition();
            UseEnemy(position, param.ID);
            await UniTask.DelayFrame(1);
        }
    }

    /// <summary>
    /// エネミーの使用
    /// </summary>
    /// <param name="position"></param>
    /// <param name="masterID"></param>
    public void UseEnemy(Vector3 position, int masterID)
    {
        int useID = UseCharacter(masterID);
        if (useID >= 0)
        {
            // セットアップ
            _useList[useID]?.Setup(useID, position, masterID);
        }
    }

    /// <summary>
    /// キャラクターの使用
    /// </summary>
    /// <param name="masterID"></param>
    /// <returns></returns>
    private int UseCharacter(int masterID)
    {
        // 使用可能なIDを取得
        int useID = _useList.FindIndex(enemy => enemy == null);
        if (useID == -1)
        {
            useID = _useList.Count;
            _useList.Add(null);
        }

        // キャラクターの生成
        Entity_EnemyData.Param param = CharacterMasterUtility.GetCharacterMaster(masterID);
        GameObject useObject = Instantiate(enemyOrigin.enemies[masterID]);
        useObject.transform.SetParent(_useObjectRoot);
        useObject.transform.position = Vector3.zero;
        // キャラクターの初期化
        _useList[useID] = useObject.AddComponent<EnemyBase>();
        _useObjectList.Insert(useID, useObject);

        return useID;
    }

    /// <summary>
    /// エネミーの未使用
    /// </summary>
    /// <param name="unuseEnemy"></param>
    public void UnuseEnemy(EnemyBase unuseEnemy)
    {
        if (unuseEnemy == null) return;
        int unuseID = unuseEnemy.ID;

        if (IsEnableIndex(_useList, unuseID))
        {
            _useList[unuseID] = null;
        }

        unuseEnemy.Teardown();
        UnuseObject(unuseID);
    }

    /// <summary>
    /// オブジェクトの未使用
    /// </summary>
    /// <param name="unuseID"></param>
    private void UnuseObject(int unuseID)
    {
        if (!IsEnableIndex(_useObjectList, unuseID)) return;
        GameObject unuseObject = _useObjectList[unuseID];

        if (unuseObject != null)
        {
            unuseObject.transform.SetParent(_unuseObjectRoot);
            _useObjectList[unuseID] = null;
        }
    }

    /// <summary>
    /// キャラクターオブジェクトの取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    private GameObject GetCharacterObject(int ID)
    {
        return IsEnableIndex(_useObjectList, ID) ? _useObjectList[ID] : null;
    }

    /// <summary>
    /// キャラクターデータ取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public EnemyBase Get(int ID)
    {
        return IsEnableIndex(_useList, ID) ? _useList[ID] : null;
    }

    /// <summary>
    /// 全てのキャラクターに処理実行
    /// </summary>
    /// <param name="action"></param>
    public void ExecuteAll(System.Action<EnemyBase> action)
    {
        if (action == null) return;
        foreach (var enemy in _useList)
        {
            action(enemy);
        }
    }

    /// <summary>
    /// 全てのキャラクターに処理実行
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public bool ExecuteAll(System.Func<EnemyBase, bool> action)
    {
        if (action == null) return false;
        foreach (var enemy in _useList)
        {
            if (enemy != null && action(enemy)) return true;
        }
        return false;
    }
    
    /// <summary>
    /// 全てのキャラクターにタスク実行
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public async UniTask ExecuteAllTask(System.Func<EnemyBase, UniTask> task)
    {
        if (task == null) return;
        foreach (var enemy in _useList)
        {
            if (enemy != null) await task(enemy);
        }
    }


    private void OnGUI()
    {
        Color oldColor = GUI.color;
        GUI.color = Color.yellow;
        using (new GUILayout.AreaScope(new Rect(0, 0, Screen.width, Screen.height)))
        {
            using (new GUILayout.VerticalScope())
            {
                using (new GUILayout.HorizontalScope())
                {
                    for(int i = 0; i < _useList.Count; i++)
                    {
                        using (new GUILayout.VerticalScope("box"))
                        {
                            _useList[i].ShowSelfData();
                        }

                        GUILayout.FlexibleSpace();
                    }
                }
                GUILayout.FlexibleSpace();
            }
        }
        GUI.color = oldColor;
    }
}
