using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// ���Ă���Ԓ�~����
public class StopLookAt : ISkill
{
    [SerializeField]
    private GameObject _mesh;           // ���b�V��

    private int ID;                     // ID
    private  Bounds bounds;             // ���b�V���͈̔�

    public float detectionRange = 5.0f; // �߂Â����Ƃ݂Ȃ�����
    private bool canCollide = false;    // �����蔻�������

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="animator"></param>
    public void Init(int setID)
    {
        ID = setID;
    }

    /// <summary>
    /// ���Ă�Ǝ~�܂�
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public void Ability()
    {
        if(EnemyManager.instance.Get(ID) ==null) return;
        EnemyBase enemy = EnemyUtility.GetCharacter(ID);
        Transform player = EnemyUtility.GetPlayer().transform;
        SkinnedMeshRenderer filter = enemy.GetComponentInChildren<SkinnedMeshRenderer>();
        bounds = filter.bounds;
        detectionRange = bounds.size.magnitude / 2;
        Vector3[] targetPoints = new Vector3[8];
        // ���b�V����8�̒��_���擾
        targetPoints[0] = bounds.min + new Vector3(0.0f, 0.5f, 0.0f);
        targetPoints[1] = new Vector3(bounds.max.x, bounds.min.y + 0.5f, bounds.min.z);
        targetPoints[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        targetPoints[3] = new Vector3(bounds.min.x, bounds.min.y + 0.5f, bounds.max.z);
        targetPoints[4] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        targetPoints[5] = new Vector3(bounds.max.x, bounds.min.y + 0.5f, bounds.max.z);
        targetPoints[6] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
        targetPoints[7] = bounds.max;
        // �ڐG�ԋߔ͈͂Ȃ�
        float length = EnemyUtility.EnemyToPlayerLength(ID);
        bool isCloseEnough = false;
        if (length < detectionRange)
        {
            isCloseEnough = true;
        }
        else
        {
            isCloseEnough = false;
        }

        // �J�������ɃI�u�W�F�N�g�����邩�ǂ���
        bool isInsideCamera = false;
        // �^�[�Q�b�g�|�C���g���J�����̃r���[�|�[�g���ɂ��邩�ǂ����𒲂ׂ�
        foreach (var targetPoint in targetPoints)
        {
            Plane[] planes;
            Camera camera = Camera.main;
            // �J�����̎���������߂�
            planes = GeometryUtility.CalculateFrustumPlanes(camera);

            // �J�����Ɏʂ��Ă��邩����
            if (GeometryUtility.TestPlanesAABB(planes, bounds))
            {
                // �R�[�i�[����J�����ʒu�ւ̃��C�L���X�g
                Vector3 direction = -(targetPoint - player.position);
                Ray ray = new Ray(targetPoint, direction.normalized);
                RaycastHit hit;
                // ���C�L���X�g���v���C���[�ɒ��ړ����邩�m�F
                LayerMask layer = LayerMask.GetMask("Player") | LayerMask.GetMask("Stage");
                if (Physics.Raycast(ray, out hit, direction.magnitude + 1, layer))
                {
                    // ��Q�����Ȃ����ړ��������ꍇ�� true ��Ԃ�
                    if (hit.collider.CompareTag("Player"))
                    {
                        isInsideCamera = true;
                    }
                }
            }
        }
        // �J�������ɃI�u�W�F�N�g�����邩�ǂ���
        // �w�ォ��ڐG�ԋ߂܂ŋ߂Â�����
        if (isCloseEnough && isInsideCamera && !enemy.isAbility)
        {
            canCollide = true;
        }
        else if (isInsideCamera)
        {
            canCollide = false;
        }
        else
        {
            canCollide = true;
        }

        if (!canCollide)
        {
            // ��~
            enemy.SetAnimationSpeed(0);
            enemy.SetNavSpeed(0);
            if(!enemy.isAbility) enemy.SetIsAbility();
        }
        else
        {
            // �ĊJ
            enemy.SetAnimationSpeed(Mathf.Min(enemy.speed, 2));
            enemy.SetNavSpeed(enemy.speed);
            if (enemy.isAbility) enemy.SetIsAbility();
        }
    }
}
