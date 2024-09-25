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
        //Ability();

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
        Debug.DrawLine(enemyInfo.status.position, enemyInfo.status.targetPos, Color.blue, 0.01f);

        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = enemyInfo.status.position;                                                   // ���_
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                    // Ray�𐶐�;

        RaycastHit hit;

        for (int i = 0; i < 100; i++)
        {
            float myAngle1 = Template(enemyInfo.status.dir);

            Debug.DrawLine(ray.origin,
                ray.origin + (
                Quaternion.Euler(
                    new Vector3(0, Mathf.Repeat(myAngle1, 360) - (enemyInfo.fieldOfView / 2) + ((enemyInfo.fieldOfView / 100) * i), 0)) * enemyInfo.status.dir * enemyInfo.viewLength),
                Color.gray,
                0.01f);
        }

        if (Physics.Raycast(ray, out hit, enemyInfo.viewLength + 1, 1))                                                       // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            Debug.DrawLine(ray.origin, enemyInfo.status.targetPos, Color.red, 0.01f);
            // Debug.DrawLine(ray.origin, ray.origin + (enemyInfo.status.dir * enemyInfo.viewLength), Color.blue, 0.01f);

            string tag = hit.collider.gameObject.tag;                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾

            // �v���C���[�Ȃ�
            if (tag == "Player")
            {

                float toPlayerAngle = Template(enemyInfo.status.position, enemyInfo.playerStatus.playerPos);   // �v���C���[�ւ̊p�x
                float myAngle = Template(enemyInfo.status.dir);                                     // �����Ă�p�x

                // 0 ~ 360�ɃN�����v
                toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
                myAngle = Mathf.Repeat(myAngle, 360);

                // ����͈͓��Ȃ�
                if (myAngle + (enemyInfo.fieldOfView / 2) > toPlayerAngle &&
                    myAngle - (enemyInfo.fieldOfView / 2) < toPlayerAngle)
                {
                    Debug.Log("����");
                    // ������
                    tracking = true;
                }
            }
        }

        // �����ɓ�������
        if (Vector3.Distance(enemyInfo.status.position , enemyInfo.status.targetPos) > 3.0f) return;

        //// �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        //Vector3 origin = enemyInfo.status.position;                                                              // ���_
        //Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X��������\���x�N�g��
        //Ray ray = new Ray(origin, direction);                                                                    // Ray�𐶐�;

        //RaycastHit hit;
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
        else
        {
            Debug.Log("������Ȃ�");

            // �v���C���[�����Ȃ�������SEARCH��Ԃɐ؂�ւ�
            search = true;
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
                // �R�[�i�[����J�����ʒu�ւ̃��C�L���X�g
                Vector3 direction = -(targetPoint - enemyInfo.playerStatus.cam.transform.position);
                Ray ray = new Ray(targetPoint, direction.normalized);
                RaycastHit hit;
                // ���C�L���X�g���v���C���[�ɒ��ړ����邩�m�F
                if (Physics.SphereCast(ray, 0.1f, out hit, direction.magnitude + 1))
                {
                    // ���C��`�悷��
                   // Debug.DrawLine(ray.origin, ray.origin + ray.direction * (direction.magnitude + 1), Color.green, 0.01f);

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
            enemyInfo.status.nowSpeed = 0.0f; // �ڕW�ʒu�����݈ʒu��
            enemyInfo.animator.speed = 0.0f;                        // �A�j���[�V�����̍Đ����~
            enemyInfo.status.nowAccelerate = 0.0f;
            enemyInfo.status.isAblity = true;
        }
        else
        {
            enemyInfo.status.nowSpeed = enemyInfo.speed;
            enemyInfo.status.nowAccelerate = enemyInfo.accelerate;
            enemyInfo.animator.speed = enemyInfo.animSpeed; ;   // �ʏ�Đ�
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
