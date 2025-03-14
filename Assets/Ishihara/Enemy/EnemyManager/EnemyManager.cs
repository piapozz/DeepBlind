using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static CommonModule;

public class EnemyManager : SystemObject
{
    public static EnemyManager instance { get; private set; } = null;

    [SerializeField]
    private Transform _useObjectRoot = null;

    [SerializeField]
    private Transform _unuseObjectRoot = null;

    [SerializeField]
    EnemyPrefabs enemyOrigin;

    private List<EnemyBase> _useList = new List<EnemyBase>();
    private List<GameObject> _useObjectList = new List<GameObject>();

    private BGM bgm;

    public override void Initialize()
    {
        if (instance != null)
        {
            Debug.LogError("EnemyManager instance already exists!");
            return;
        }
        instance = this;
        MasterDataManager.LoadAllData();
        EnemyBase.SetGetObjectCallback(GetCharacterObject);

        int enemyMax = MasterDataManager.enemyData[0].Count;
        _useList.Capacity = enemyMax;
        _useObjectList.Capacity = enemyMax;

        AudioManager.instance.PlayBGM(BGM.MAIN_NORMAL);
        bgm = BGM.MAIN_NORMAL;

        CreateEnemy().Forget();
        StartEnemyBehaviorLoop().Forget();
    }

    private async UniTask StartEnemyBehaviorLoop()
    {
        while (true)
        {
            ExecuteAll(enemy => enemy.Active());
            CheckChangeBGM();

            if(ExecuteAll(enemy => {
                if (enemy.CheckCaughtPlayer())
                {
                    Player.instance.EnemyCaught(enemy._camera);
                    enemy.SetAnimationSpeed(1);
                    enemy.SetScreamTrigger();
                    return true;
                }

                return false;
            }))
            {
                break;
            }
            await UniTask.DelayFrame(1);
        }
    }

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

    public Vector3 DispatchTargetPosition()
    {
        return GenerateStage.instance.GetRandCorridorPos();
    }

    private async UniTask CreateEnemy()
    {
        foreach (var param in MasterDataManager.enemyData[0])
        {
            Vector3 position = DispatchTargetPosition();
            UseEnemy(position, param.ID);
            await UniTask.DelayFrame(1);
        }
    }

    public void UseEnemy(Vector3 position, int masterID)
    {
        int useID = UseCharacter(masterID);
        if (useID >= 0)
        {
            _useList[useID]?.Setup(useID, position, masterID);
        }
    }

    private int UseCharacter(int masterID)
    {
        int useID = _useList.FindIndex(enemy => enemy == null);
        if (useID == -1)
        {
            useID = _useList.Count;
            _useList.Add(null);
        }

        Entity_EnemyData.Param param = CharacterMasterUtility.GetCharacterMaster(masterID);
        GameObject useObject = Instantiate(enemyOrigin.enemies[masterID]);
        useObject.transform.SetParent(_useObjectRoot);

        _useList[useID] = useObject.AddComponent<EnemyBase>();
        _useObjectList.Insert(useID, useObject);

        return useID;
    }

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

    private GameObject GetCharacterObject(int ID)
    {
        return IsEnableIndex(_useObjectList, ID) ? _useObjectList[ID] : null;
    }

    public EnemyBase Get(int ID)
    {
        return IsEnableIndex(_useList, ID) ? _useList[ID] : null;
    }

    public void ExecuteAll(System.Action<EnemyBase> action)
    {
        if (action == null) return;
        foreach (var enemy in _useList)
        {
            action(enemy);
        }
    }

    public bool ExecuteAll(System.Func<EnemyBase, bool> action)
    {
        if (action == null) return false;
        foreach (var enemy in _useList)
        {
            if (enemy != null && action(enemy)) return true;
        }
        return false;
    }

    public async UniTask ExecuteAllTask(System.Func<EnemyBase, UniTask> task)
    {
        if (task == null) return;
        foreach (var enemy in _useList)
        {
            if (enemy != null) await task(enemy);
        }
    }
}
