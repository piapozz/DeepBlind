using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

// �G�l�~�[�̃��[�e�B���e�B
public class EnemyUtility : MonoBehaviour
{
    /// <summary>
    /// �v���C���[�擾
    /// </summary>
    /// <returns></returns>
    public static Player GetPlayer()
    {
        return Player.instance;
    }

    /// <summary>
    /// �L�����N�^�[�f�[�^�擾
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static EnemyBase GetCharacter(int ID)
    {
        return EnemyManager.instance.Get(ID);
    }

    /// <summary>
    /// �S�ẴL�����N�^�[�ɏ������s
    /// </summary>
    /// <param name="action"></param>
    public static void ExecuteAllCharacter(System.Action<EnemyBase> action)
    {
        EnemyManager.instance.ExecuteAll(action);
    }

    /// <summary>
    /// �S�ẴL�����N�^�[�Ƀ^�X�N���s
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public static async UniTask ExecuteTaskAllCharacter(System.Func<EnemyBase, UniTask> task)
    {
        await EnemyManager.instance.ExecuteAllTask(task);
    }

    /// <summary>
    /// �L�����N�^�[�̎��S����
    /// </summary>
    /// <param name="character"></param>
    /// <returns></returns>
    public static async UniTask DeadCharacter(EnemyBase character)
    {
        // �G�l�~�[���S�̏���
        EnemyManager.instance.UnuseEnemy(character);
        
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// �G�l�~�[����v���C���[�܂ł̋������擾
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static float EnemyToPlayerLength(int ID)
    {
        EnemyBase enemy = GetCharacter(ID);
        Player player = GetPlayer();

        Vector3 start = enemy.transform.position;
        start.y = 0;
        Vector3 end = player.transform.position;
        end.y = 0;

        float length = Vector3.Distance(start, end);
        return length;
    }

    /// <summary>
    /// �G�l�~�[����v���C���[���m�F�ł��邩�̊֐�
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="locker">���b�J�[�𖳎����邩�ǂ���</param>
    /// <returns></returns>
    public static bool CheckViewPlayer(int ID, bool ignoreLocker = true)
    {
        bool isHit = false;

        EnemyBase enemy = GetCharacter(ID);
        Player player = GetPlayer();

        if (enemy == null || player == null) return false;
        if (player.isLocker && !ignoreLocker) return false;

        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = enemy.transform.position;                                                              // ���_
        Vector3 direction = Vector3.Normalize(player.transform.position - enemy.transform.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                                    // Ray�𐶐�;

        RaycastHit hit;
        LayerMask layer = LayerMask.GetMask("Player") | LayerMask.GetMask("Stage");
        isHit = Physics.Raycast(ray, out hit, Vector3.Distance(enemy.transform.position, player.transform.position), layer);

        return isHit && hit.collider.tag == "Player";
    }

    /// <summary>
    /// �A���J�[���c���Ă��邩�m�F����
    /// �c���Ă����玟�̃A���J�[��Ԃ�
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static bool CheckSearchAnchor(int ID)
    {
        EnemyBase enemy = GetCharacter(ID);
        if (enemy == null) return false;
        if (enemy.searchAnchorList.Count < 2) return false;
        enemy.searchAnchorList.RemoveAt(0);
        enemy.SetNavTarget(enemy.searchAnchorList[0].position);

        return true;
    }

    /// <summary>
    /// ���̃A���J�[��ݒ肷��
    /// </summary>
    /// <param name="ID"></param>
    public static void SetSearchAnchor(int ID)
    {
        EnemyBase enemy = GetCharacter(ID);
        if (enemy == null) return;
        enemy.SetSearchAnchor(StageManager.instance.GetRandomEnemyAnchor());
        enemy.SetNavTarget(enemy.searchAnchorList[0].position);
    }
}
