using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static CommonModule;

// �G�l�~�[�̊Ǘ��A����

public class EnemyManager : SystemObject
{
    public static EnemyManager instance { get; private set; } = null;

    [SerializeField]
    private Transform _useObjectRoot = null;                            // �L���I�u�W�F�N�g

    [SerializeField]
    private Transform _unuseObjectRoot = null;                          // �����I�u�W�F�N�g

    [SerializeField]
    EnemyPrefabs enemyOrigin;                                           // �v���n�u�ꗗ

    private List<EnemyBase> _useList = new List<EnemyBase>();           // �g�p�����X�g
    private List<GameObject> _useObjectList = new List<GameObject>();   // �g�p���I�u�W�F�N�g���X�g

    private BGM bgm;                                                    // �Đ�����BGM

    public override async void Initialize()
    {
        if (instance != null)
        {
            return;
        }
        instance = this;

        // �}�X�^�[�f�[�^�̓ǂݍ���
        MasterDataManager.LoadAllData();
        // �R�[���o�b�N�̐ݒ�
        EnemyBase.SetGetObjectCallback(GetCharacterObject);
        // ���X�g�̏�����
        int enemyMax = MasterDataManager.enemyData[0].Count;
        _useList.Capacity = enemyMax;
        _useObjectList.Capacity = enemyMax;
        // BGM�̍Đ�
        AudioManager.instance.PlayBGM(BGM.MAIN_NORMAL);
        bgm = BGM.MAIN_NORMAL;
        // �G�l�~�[�̐���
        await CreateEnemy();
        StartEnemyBehaviorLoop().Forget();
    }

    /// <summary>
    /// �G�l�~�[�̍s�����[�v
    /// </summary>
    /// <returns></returns>
    private async UniTask StartEnemyBehaviorLoop()
    {
        while (true)
        {
            // �G�l�~�[�̍s��
            ExecuteAll(enemy => enemy.Active());
            CheckChangeBGM();

            // �v���C���[�̕ߊl����
            if (ExecuteAll(enemy => {
                if (enemy.CheckCaughtPlayer())
                {
                    // �v���C���[�ߊl
                    Player.instance.EnemyCaught(enemy._camera);
                    enemy.CaughtPlayer();
                    return true;
                }

                return false;
            }))
            {
                // ���[�v�I��
                break;
            }
            await UniTask.DelayFrame(1);
        }
    }

    /// <summary>
    /// BGM�̕ύX
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
    /// �����_���ȃ}�b�v�̍��W���擾
    /// </summary>
    /// <returns></returns>
    public Vector3 DispatchTargetPosition()
    {
        List<Transform> anchorList = StageManager.instance.GetRandomEnemyAnchor();
        return anchorList[0].position;
    }

    /// <summary>
    /// �G�l�~�[�̐���
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
    /// �G�l�~�[�̎g�p
    /// </summary>
    /// <param name="position"></param>
    /// <param name="masterID"></param>
    public void UseEnemy(Vector3 position, int masterID)
    {
        int useID = UseCharacter(masterID);
        if (useID >= 0)
        {
            // �Z�b�g�A�b�v
            _useList[useID]?.Setup(useID, position, masterID);
        }
    }

    /// <summary>
    /// �L�����N�^�[�̎g�p
    /// </summary>
    /// <param name="masterID"></param>
    /// <returns></returns>
    private int UseCharacter(int masterID)
    {
        // �g�p�\��ID���擾
        int useID = _useList.FindIndex(enemy => enemy == null);
        if (useID == -1)
        {
            useID = _useList.Count;
            _useList.Add(null);
        }

        // �L�����N�^�[�̐���
        Entity_EnemyData.Param param = CharacterMasterUtility.GetCharacterMaster(masterID);
        GameObject useObject = Instantiate(enemyOrigin.enemies[masterID]);
        useObject.transform.SetParent(_useObjectRoot);
        useObject.transform.position = Vector3.zero;
        // �L�����N�^�[�̏�����
        _useList[useID] = useObject.AddComponent<EnemyBase>();
        _useObjectList.Insert(useID, useObject);

        return useID;
    }

    /// <summary>
    /// �G�l�~�[�̖��g�p
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
    /// �I�u�W�F�N�g�̖��g�p
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
    /// �L�����N�^�[�I�u�W�F�N�g�̎擾
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    private GameObject GetCharacterObject(int ID)
    {
        return IsEnableIndex(_useObjectList, ID) ? _useObjectList[ID] : null;
    }

    /// <summary>
    /// �L�����N�^�[�f�[�^�擾
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public EnemyBase Get(int ID)
    {
        return IsEnableIndex(_useList, ID) ? _useList[ID] : null;
    }

    /// <summary>
    /// �S�ẴL�����N�^�[�ɏ������s
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
    /// �S�ẴL�����N�^�[�ɏ������s
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
    /// �S�ẴL�����N�^�[�Ƀ^�X�N���s
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
