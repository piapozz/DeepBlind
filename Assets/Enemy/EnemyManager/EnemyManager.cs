using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // ���̊Ǘ�

        // �ڐG����Ǘ�

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
                var enemy = Instantiate(enemies[i], generateStage.GetRandRoomPos(), Quaternion.identity);

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
        }
    }


}
