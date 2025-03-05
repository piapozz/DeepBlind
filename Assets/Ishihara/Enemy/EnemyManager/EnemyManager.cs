using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using static CommonModule;

public class EnemyManager : SystemObject
{
    public static EnemyManager instance { get; private set; } = null;

    [SerializeField]
    private GameObject[] _enemies;              // �v���n�u�ꗗ

    [SerializeField]
    private int[] _createEnemies;               // ��������G�l�~�[�̐��Ǝ��

    [SerializeField]
    private Transform _useObjectRoot = null;

    [SerializeField]
    private Transform _unuseObjectRoot = null;

    // �g�p���̃L�����N�^�[���X�g
    private List<EnemyBase> _useList = null;
    // ���g�p��Ԃ̃G�l�~�[���X�g
    private List<EnemyBase> _unuseList = null;

    // �g�p���̃L�����N�^�[�I�u�W�F�N�g���X�g
    private List<GameObject> _useObjectList = null;
    // ���g�p��Ԃ̃L�����N�^�[�I�u�W�F�N�g���X�g
    private List<GameObject> _unuseObjectList = null;

    // �Đ�����BGM
    BGM bgm;

    public override void Initialize()
    {
        MasterDataManager.LoadAllData();
        instance = this;
        EnemyBase.SetGetObjectCallback(GetCharacterObject);

        int enemyMax = 0;
        for(int i = 0, max = _enemies.Length; i < max; i++)
        {
            enemyMax += _createEnemies[i];
        }

        // �K�v�ȃL�����N�^�[�ƃI�u�W�F�N�g�̃C���X�^���X�𐶐����Ė��g�p��Ԃɂ��Ă���
        _useList = new List<EnemyBase>(enemyMax);
        _unuseList = new List<EnemyBase>(enemyMax);
        _useObjectList = new List<GameObject>(enemyMax);
        _unuseObjectList = new List<GameObject>(enemyMax);

        for (int i = 0; i < enemyMax; i++)
        {
            _useList.Add(new EnemyBase());
        }
        for (int i = 0; i < enemyMax; i++)
        {
            _unuseList.Add(new EnemyBase());
        }
        for (int i = 0; i < enemyMax; i++)
        {
            _useObjectList.Add(new GameObject());
        }
        for (int i = 0; i < enemyMax; i++)
        {
            _unuseObjectList.Add(new GameObject());
        }
        AudioManager.instance.PlayBGM(BGM.MAIN_NORMAL);
        bgm = BGM.MAIN_NORMAL;

        // �G�l�~�[�̐���
        CreateEnemy();
        // �񓯊����[�v���J�n
        StartEnemyBehaviorLoop();
    }

    /// <summary>
    /// �񓯊��̃G�l�~�[�Ǘ����[�v
    /// </summary>
    /// <returns></returns>
    private async UniTask StartEnemyBehaviorLoop()
    {
        while (true)
        {
            ExecuteAll(enemy => enemy.Active());

            CheckChangeBGM();

            // �G�l�~�[�ƃv���C���[���ڐG���Ă��邩���m�F
            if(ExecuteAll<bool>(enemy =>
            {
                if (enemy.ChackCaughtPlayer())
                {
                    EnemyUtility.GetPlayer().EnemyCaught(enemy.gameObject);
                    return true;
                }
                return false;
            }))
            {
                return;
            }

            // ���̃t���[���܂őҋ@
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
    /// �T����Ԃ̃G�l�~�[�̖ڕW�ʒu��U�蕪����
    /// </summary>
    public Vector3 DispatchTargetPosition()
    {
        // �ڕW�ʒu���Đݒ�
        return GenerateStage.instance.GetRandCorridorPos();
    }

    /// <summary>
    /// �G�l�~�[�̐���
    /// </summary>
    private async UniTask CreateEnemy()
    {
        for (int i = 0; i < MasterDataManager.enemyData[0].Count; i++)
        {
            // ����
            // �g�p�\��ID���擾���Ďg�p���X�g�ɒǉ�
            Entity_EnemyData.Param param = MasterDataManager.enemyData[0][i];
            Vector3 position = GenerateStage.instance.GetRandRoomPos();
            UseEnemy(position, param.ID);

            await UniTask.DelayFrame(1);
        }
    }

    /// <summary>
	/// �G�l�~�[�̐���
	/// </summary>
	/// <param name="squareData"></param>
	public void UseEnemy(Vector3 position, int masterID)
    {
        // �C���X�^���X�̎擾
        _unuseList.RemoveAt(0);

        // �g�p�\��ID���擾���Ďg�p���X�g�ɒǉ�
        int useID = UseCharacter(masterID);
        _useList[useID].Setup(useID, position, masterID);
    }

    private int UseCharacter(int masterID)
    {
        // �g�p�\��ID���擾���Ďg�p���X�g�ɒǉ�
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
        // �I�u�W�F�N�g�̎擾
        GameObject useObject = null;
        if (IsEmpty(_unuseObjectList))
        {
            _useObjectList.Add(new GameObject());
            _unuseObjectList.Add(new GameObject());
        }
        else
        {
            useObject = _unuseObjectList[0];
            _unuseObjectList.RemoveAt(0);
        }
        Entity_EnemyData.Param param = CharacterMasterUtility.GetCharacterMaster(masterID);
        useObject = Instantiate((GameObject)Resources.Load("Prefab/" + param.Name));
        // �I�u�W�F�N�g�̎g�p���X�g�ւ̒ǉ�
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
        // �g�p���X�g�����菜��
        if (IsEnableIndex(_useList, unuseID)) _useList[unuseID] = null;
        // �Еt��������ǂ�Ŗ��g�p���X�g�ɉ�����
        unuseEnemy.Teardown();
        _unuseList.Add(unuseEnemy);
        // �I�u�W�F�N�g�𖢎g�p�ɂ���
        UnuseObject(unuseID);
    }

    private void UnuseObject(int unuseID)
    {
        GameObject unuseCharacterObject = GetCharacterObject(unuseID);
        // �g�p���X�g�����菜��
        if (IsEnableIndex(_useObjectList, unuseID)) _useObjectList[unuseID] = null;
        // �����Ȃ��ꏊ�ɒu��
        unuseCharacterObject.transform.SetParent(_unuseObjectRoot);
        // ���g�p���X�g�ɒǉ�
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

            action(_useList[i]);
        }
    }

    public T ExecuteAll<T>(System.Func<EnemyBase , T> action)
    {
        if (action == null || IsEmpty(_useList)) return default;

        for (int i = 0, max = _useList.Count; i < max; i++)
        {
            if (_useList[i] == null) continue;

            return action(_useList[i]);
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
