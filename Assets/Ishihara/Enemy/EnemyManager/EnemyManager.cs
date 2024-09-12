using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] int managementEnemy;   // ��x�ɊǗ�����G�l�~�[�̐�

    [SerializeField] GameObject[] enemies;  // �v���n�u�ꗗ

    [SerializeField] int[] createEnemies;   // ��������G�l�~�[�̐��Ǝ��

    [SerializeField] Player player;     // �v���C���[

    [SerializeField] GenerateStage generateStage;
    
    // �}�b�v�̏��

    EnemyBase.PlayerStatus playerStatus;     // �n���v���C���[�̃f�[�^

    List<EnemyBase> enemyList = new List<EnemyBase>();

    void Start()
    {
        // �G�l�~�[�̐���
        CreateEnemy();

        // �G�l�~�[�ɏ���n��
        SetEnemyStatus();
    }

    void Update()
    {
        // �T����Ԃ̃G�l�~�[�̖ڕW�ʒu��U�蕪����
        DispatchTargetPosition();

        // �G�l�~�[�̏����X�V
        UpdateEnemyData();

        // ���̊Ǘ�

        // �ڐG����Ǘ�

    }

    // �T����Ԃ̃G�l�~�[�̖ڕW�ʒu��U�蕪����
    private void DispatchTargetPosition()
    {
        // �G�l�~�[�̐������J��Ԃ�
        for (int i = 0; i < enemyList.Count; i++)
        {

            // �T����Ԃ��ǂ���
            if (enemyList[i].GetNowState() != EnemyBase.State.SEACH) continue;

            // Debug.LogError("s");
            // ���B���Ă��邩�ǂ���
            if (!enemyList[i].CheckReachingPosition()) continue;


            // �ڕW�ʒu���Đݒ�
            enemyList[i].SetTargetPos(generateStage.GetRandRoomPos());
        }
    }

    // �G�l�~�[�̏����X�V
    private void UpdateEnemyData()
    {

        // �v���C���[����������炤
        playerStatus.cam = player.GetCamera();
        playerStatus.playerPos = player.GetPosition();

        // �G�l�~�[�̐������J��Ԃ�
        for (int i = 0; i < enemyList.Count; i++)
        {
            // �v���C���[�̏��X�V
            enemyList[i].SetPlayerStatus(playerStatus);
        }
    }

    // �G�l�~�[�̐���
    private void CreateEnemy()
    {
        // �J��Ԃ�
        for(int i = 0; i < enemies.Length; i++)
        {
            for (int j = 0; j < createEnemies[i]; j++)
            {
                // ����
                var enemy = Instantiate(enemies[i], generateStage.GetRandCorridorPos(), Quaternion.identity);

                // ���X�g�ɒǉ�
                enemyList.Add(enemy.GetComponent<EnemyBase>());
            }
        }
    }

    // �G�l�~�[�ɏ���n��
    private void SetEnemyStatus()
    {
        // �v���C���[����������炤
        playerStatus.cam = player.GetCamera();
        playerStatus.playerPos = player.GetPosition();

        // �G�l�~�[�̐������J��Ԃ�
        for(int i = 0;i < enemyList.Count; i++)
        {
            // �v���C���[�̏��X�V
            enemyList[i].SetPlayerStatus(playerStatus);

            enemyList[i].SetTargetPos(generateStage.GetRandRoomPos());
        }
    }
}
