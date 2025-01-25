using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; } = null;

    [SerializeField]
    private GameObject[] _enemies;              // �v���n�u�ꗗ

    [SerializeField]
    private int[] _createEnemies;               // ��������G�l�~�[�̐��Ǝ��

    [SerializeField]
    private Player _player;                     // �v���C���[

    [SerializeField]
    private GenerateStage _generateStage;       // �X�e�[�W�̏��

    List<EnemyBase> enemyList = new List<EnemyBase>();  // �Ǘ�����G���X�g

    void Start()
    {
        Instance = this;

        // �G�l�~�[�̐���
        CreateEnemy();

        // �񓯊����[�v���J�n
        StartEnemyBehaviorLoop().Forget();
    }

    /// <summary>
    /// �񓯊��̃G�l�~�[�Ǘ����[�v
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid StartEnemyBehaviorLoop()
    {
        while (true)
        {
            // �G�l�~�[�̏����X�V
            UpdateEnemyData();

            // �G�l�~�[�ƃv���C���[���ڐG���Ă��邩���m�F
            if (HittingDecision())
            {
                Debug.Log("�I��");
                break;
            }

            // ���̃t���[���܂őҋ@
            await UniTask.Yield();
        }
    }

    /// <summary>
    /// �v���C���[�̗\�����[�g���擾����
    /// </summary>
    /// <returns></returns>
    public List<EnemyBase.ViaSeachData> SpecifiedExpectedPosition()
    {
        // �ڕW�ʒu���Đݒ�
        return _generateStage.GetPredictionPlayerPos(_player.GetPosition(), _player.GetMoveVec());
    }

    /// <summary>
    /// �T����Ԃ̃G�l�~�[�̖ڕW�ʒu��U�蕪����
    /// </summary>
    public Vector3 DispatchTargetPosition()
    {
        // �ڕW�ʒu���Đݒ�
        return _generateStage.GetRandRoomPos();
    }

    /// <summary>
    /// �G�l�~�[�̏����X�V
    /// </summary>
    private void UpdateEnemyData()
    {
        EnemyBase.PlayerStatus playerStatus;

        // �v���C���[��������擾
        playerStatus.cam = _player.GetCamera();
        playerStatus.playerPos = _player.GetPosition();

        // �e�G�l�~�[�Ƀv���C���[�̏���n��
        foreach (var enemy in enemyList)
        {
            enemy.SetPlayerStatus(playerStatus);
        }
    }

    /// <summary>
    /// �G�l�~�[�̐���
    /// </summary>
    private void CreateEnemy()
    {
        for (int i = 0; i < _enemies.Length; i++)
        {
            for (int j = 0; j < _createEnemies[i]; j++)
            {
                // ����
                var enemy = Instantiate(_enemies[i], _generateStage.GetRandCorridorPos(), Quaternion.identity, this.transform);

                // ���X�g�ɒǉ�
                enemyList.Add(enemy.GetComponent<EnemyBase>());
            }
        }
    }

    /// <summary>
    /// �G�l�~�[�ƃv���C���[���ڐG���Ă��邩���ׂ�
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
