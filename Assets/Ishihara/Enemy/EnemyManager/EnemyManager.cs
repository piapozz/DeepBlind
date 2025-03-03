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

    public override void Initialize()
    {
        instance = this;
        EnemyBase.SetGetObjectCallback(GetCharacterObject);

        int enemyMax = 0;
        for(int i = 0, max = _enemies.Length; i < max; i++)
        {
            enemyMax += _createEnemies[i];
        }

        // �K�v�ȃL�����N�^�[�ƃI�u�W�F�N�g�̃C���X�^���X�𐶐����Ė��g�p��Ԃɂ��Ă���
        _useList = new List<EnemyBase>(enemyMax);

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
            ExecuteAll(enemy => enemy.MoveNavAgent());

            // �G�l�~�[�ƃv���C���[���ڐG���Ă��邩���m�F
            
            {
               
            }

            // ���̃t���[���܂őҋ@
            await UniTask.DelayFrame(1);
        }
    }

    /// <summary>
    /// �T����Ԃ̃G�l�~�[�̖ڕW�ʒu��U�蕪����
    /// </summary>
    public Vector3 DispatchTargetPosition()
    {
        // �ڕW�ʒu���Đݒ�
        return GenerateStage.instance.GetRandRoomPos();
    }

    /// <summary>
    /// �G�l�~�[�̐���
    /// </summary>
    private async UniTask CreateEnemy()
    {
        for (int i = 0; i < _enemies.Length; i++)
        {
            for (int j = 0; j < _createEnemies[i]; j++)
            {
                // ����
                var enemy = Instantiate(_enemies[i], GenerateStage.instance.GetRandCorridorPos(), Quaternion.identity, this.transform);

                // ID�̎擾
                // �C���X�^���X�̎擾
                EnemyBase useEnemy = enemy.GetComponent<EnemyBase>();
                
                // �g�p�\��ID���擾���Ďg�p���X�g�ɒǉ�
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
	/// �G�l�~�[�̐���
	/// </summary>
	/// <param name="squareData"></param>
	public void UseEnemy(Vector3 squareData, int masterID)
    {
        // �C���X�^���X�̎擾
        EnemyBase useEnemy = null;
        useEnemy = _unuseList[0];
        _unuseList.RemoveAt(0);
        

        // �g�p�\��ID���擾���Ďg�p���X�g�ɒǉ�
        int useID = UseCharacter(useEnemy, masterID);
        useEnemy.Setup(useID, squareData, masterID);
    }

    private int UseCharacter(EnemyBase useCharacter, int masterID)
    {
        // �g�p�\��ID���擾���Ďg�p���X�g�ɒǉ�
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
        // �I�u�W�F�N�g�̎擾
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
        // �I�u�W�F�N�g�̎g�p���X�g�ւ̒ǉ�
        while (!IsEnableIndex(_useObjectList, useID)) _useObjectList.Add(null);

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
