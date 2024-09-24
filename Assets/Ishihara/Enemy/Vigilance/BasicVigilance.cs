using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static EnemyBase;

public class BasicVigilance : IVigilance
{
    // �X�V������
    EnemyInfo enemyInfo;

    // ��ԊǗ��t���O
    bool search = false;
    bool tracking = false;

    // �s��
    public EnemyInfo Activity(EnemyInfo info)
    {
        // �擾
        GetTarget(info);

        // ���S�Ɍ����������ǂ���
        CheckLookAround();

        // ���ꏈ��
        Ability();

        // �X�V
        StatusUpdate();

        // �ړ�
        Move();

        return enemyInfo;
    }

    // ������
    public void Init()
    {
        enemyInfo = new EnemyInfo();
    }

    // ��������n��
    public void CheckLookAround()
    {
        // �����ɓ�������
        if(Vector3.Distance(enemyInfo.status.position , enemyInfo.status.targetPos) > 1.0f) return;

        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = enemyInfo.status.position;                                                              // ���_
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                                    // Ray�𐶐�;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, enemyInfo.viewLength))                                                 // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            string tag = hit.collider.gameObject.tag;                                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾

            // ����͈͓��Ȃ�
            if (tag == "Player")
            {
                // �ǐՌp��
                Debug.Log("������");

                // �v���C���[��������TRACKING��Ԃɐ؂�ւ���
                tracking = true;
            }
            // ���߂Č������Ă�����
            else if (tag != "Player")
            {
                Debug.Log("������Ȃ�");

                // �v���C���[�����Ȃ�������SEARCH��Ԃɐ؂�ւ�
                search = true;
            }
        }

    }

    // �p�x�v�Z
    private float Template(Vector3 point1)
    {
        float temp;

        temp = Mathf.Atan2(point1.z, point1.x);

        return temp;
    }

    // �p�x�v�Z
    private float Template(Vector3 point1, Vector3 point2)
    {
        float temp;

        temp = Mathf.Atan2(point1.z - point2.z, point1.x - point2.x);

        return temp;
    }

    // ���ꏈ��(�����Ă�����~�܂�)
    public void Ability()
    {
        Vector3[] targetPoints = new Vector3[8];

        targetPoints[0] = enemyInfo.bounds.min;
        targetPoints[1] = new Vector3(enemyInfo.bounds.max.x, enemyInfo.bounds.min.y, enemyInfo.bounds.min.z);
        targetPoints[2] = new Vector3(enemyInfo.bounds.min.x, enemyInfo.bounds.max.y, enemyInfo.bounds.min.z);
        targetPoints[3] = new Vector3(enemyInfo.bounds.min.x, enemyInfo.bounds.min.y, enemyInfo.bounds.max.z);
        targetPoints[4] = new Vector3(enemyInfo.bounds.max.x, enemyInfo.bounds.max.y, enemyInfo.bounds.min.z);
        targetPoints[5] = new Vector3(enemyInfo.bounds.max.x, enemyInfo.bounds.min.y, enemyInfo.bounds.max.z);
        targetPoints[6] = new Vector3(enemyInfo.bounds.min.x, enemyInfo.bounds.max.y, enemyInfo.bounds.max.z);
        targetPoints[7] = enemyInfo.bounds.max;

        // �e�R�[�i�[���J�����̃r���[�|�[�g�Ɏ��܂��Ă��邩���`�F�b�N

        //�@�J�������ɃI�u�W�F�N�g�����邩�ǂ���
        bool isInsideCamera = false;
        //�@�^�[�Q�b�g�|�C���g���J�����̃r���[�|�[�g���ɂ��邩�ǂ����𒲂ׂ�
        foreach (var targetPoint in targetPoints)
        {

            Plane[] planes;

            // �J�����̎���������߂�
            planes = GeometryUtility.CalculateFrustumPlanes(enemyInfo.playerStatus.cam);

            // �J�����Ɏʂ��Ă��邩����
            if (GeometryUtility.TestPlanesAABB(planes, enemyInfo.bounds))
            {
                // �J�����ʒu����R�[�i�[�ւ̃��C�L���X�g
                Vector3 direction = targetPoint - enemyInfo.playerStatus.cam.transform.position;
                Ray ray = new Ray(enemyInfo.playerStatus.cam.transform.position, direction.normalized);
                RaycastHit hit;
                // ���C�L���X�g���R�[�i�[�ɒ��ړ����邩�m�F
                if (Physics.Raycast(ray, out hit, targetPoint.magnitude + 1))
                {
                    Debug.DrawLine(ray.origin, targetPoint, Color.yellow, 0.01f);

                    // ��Q�����Ȃ����ړ��������ꍇ�� true ��Ԃ�
                    if (hit.collider.tag == "Enemy")
                    {
                        isInsideCamera = true;
                    }
                }
            }
        }

        if (isInsideCamera)
        {
            //�f���Ă����琧�~����
            enemyInfo.status.nowSpeed = 0.0f; // �ڕW�ʒu�����݈ʒu��
            enemyInfo.animator.speed = 0.0f;                        // �A�j���[�V�����̍Đ����~
            enemyInfo.status.nowAccelerate = 0.0f;
            enemyInfo.status.isAblity = true;
        }
        else
        {
            enemyInfo.status.nowSpeed = enemyInfo.speed;
            enemyInfo.status.nowAccelerate = enemyInfo.accelerate;
            enemyInfo.animator.speed = 1.0f;   // �ʏ�Đ�
            enemyInfo.status.isAblity = false;
        }
    }


    // �ڕW�ʒu�̎擾
    public void GetTarget(EnemyInfo info)
    {
        // �^�[�Q�b�g�̏��擾
        enemyInfo = info;
    }

    // ���̍X�V
    public void StatusUpdate()
    {
        // �X�e�[�g�̐؂�ւ�
        if (search) enemyInfo.status.state = State.SEARCH;
        if (tracking) enemyInfo.status.state = State.VIGILANCE;
    }

    // �ړ�
    public void Move()
    {

    }
}
