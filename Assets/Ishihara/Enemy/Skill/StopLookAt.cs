using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyBase;

public class StopLookAt : ISkill
{

    [SerializeField] GameObject mesh;

    // ���Ă�Ǝ~�܂�
    // �v���C���[���G�����������A�G���v���C���[��������
    public EnemyBase.EnemyInfo Ability(EnemyBase.EnemyInfo info)
    {
        // ���b�V���t�B���^�[�̑��݊m�F
        SkinnedMeshRenderer filter = mesh.GetComponent<SkinnedMeshRenderer>();

        // �t�B���^�[�̃��b�V����񂩂�o�E���h�{�b�N�X���擾����
        Bounds bounds = filter.bounds;

        Vector3[] targetPoints = new Vector3[8];

        targetPoints[0] = bounds.min + new Vector3(0.0f, 0.5f, 0.0f);
        targetPoints[1] = new Vector3(bounds.max.x, bounds.min.y + 0.5f, bounds.min.z);
        targetPoints[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        targetPoints[3] = new Vector3(bounds.min.x, bounds.min.y + 0.5f, bounds.max.z);
        targetPoints[4] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        targetPoints[5] = new Vector3(bounds.max.x, bounds.min.y + 0.5f, bounds.max.z);
        targetPoints[6] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
        targetPoints[7] = bounds.max;

        // �e�R�[�i�[���J�����̃r���[�|�[�g�Ɏ��܂��Ă��邩���`�F�b�N

        //�@�J�������ɃI�u�W�F�N�g�����邩�ǂ���
        bool isInsideCamera = false;
        //�@�^�[�Q�b�g�|�C���g���J�����̃r���[�|�[�g���ɂ��邩�ǂ����𒲂ׂ�
        foreach (var targetPoint in targetPoints)
        {
            Plane[] planes;

            // �J�����̎���������߂�
            planes = GeometryUtility.CalculateFrustumPlanes(info.playerStatus.cam);

            // �J�����Ɏʂ��Ă��邩����
            if (GeometryUtility.TestPlanesAABB(planes, bounds))
            {
                // �R�[�i�[����J�����ʒu�ւ̃��C�L���X�g
                Vector3 direction = -(targetPoint - info.playerStatus.cam.transform.position);
                Ray ray = new Ray(targetPoint, direction.normalized);
                RaycastHit hit;
                // ���C�L���X�g���v���C���[�ɒ��ړ����邩�m�F
                if (Physics.Raycast(ray, out hit, direction.magnitude + 1))
                {
                    // ���C��`�悷��
                    // Debug.DrawLine(ray.origin, ray.origin + ray.direction * (direction.magnitude + 1), Color.green, 0.01f);
                    Debug.DrawLine(ray.origin,hit.point, Color.green, 0.01f);
                    

                    // ��Q�����Ȃ����ړ��������ꍇ�� true ��Ԃ�
                    if (hit.collider.CompareTag("Player"))
                    {
                        isInsideCamera = true;
                    }
                }
            }
        }

        if (isInsideCamera)
        {
            //�f���Ă����琧�~����
            info.animator.speed = 0.0f;                        // �A�j���[�V�����̍Đ����~^
            info.status.isAblity = true;
            //info.status.state = EnemyBase.State.TRACKING;
        }
        else
        {
            info.animator.speed = info.animSpeed;   // �ʏ�Đ�
            info.status.isAblity = false;
        }


        return info;
    }
}
