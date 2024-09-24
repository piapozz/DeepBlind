using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;

public class BasicTracking : ITracking
{
    // ���
    EnemyInfo enemyInfo;

    // �x���t���O
    bool vigilance = false;

    // �s��
    public EnemyInfo Activity(EnemyInfo info)
    {
        // �擾
        GetTarget(info);

        // �����������ǂ���
        CheckTargetLost();

        // ���ꏈ��
        Ability();

        // �ړ�
        Move();

        // �X�V
        StatusUpdate();

        return enemyInfo;
    }

    // ������
    public void Init()
    {
        enemyInfo = new EnemyInfo();

        vigilance = false;              // ������
    }

    // �����������ǂ���
    public void CheckTargetLost()
    {
        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = enemyInfo.status.position;                                                              // ���_
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                                    // Ray�𐶐�;

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, enemyInfo.viewLength))                                                 // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            string tag = hit.collider.gameObject.tag;                                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾

            float toPlayerAngle = Template(enemyInfo.status.position, enemyInfo.playerStatus.playerPos);   // �v���C���[�ւ̊p�x
            float myAngle = Template(enemyInfo.status.dir);                                                // �����Ă�p�x

            // 0 ~ 360�ɃN�����v
            toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
            myAngle = Mathf.Repeat(myAngle, 360);

            // ����͈͓��Ȃ�
            if ((myAngle + (enemyInfo.fieldOfView / 2) > toPlayerAngle &&
                myAngle - (enemyInfo.fieldOfView / 2) < toPlayerAngle) &&
                tag == "Player")
            {
                // ���O�܂Ō������Ă����Ȃ�
                if (enemyInfo.status.isTargetLost) enemyInfo.status.isTargetLost = false; // �Ĕ���
            }
            // ���߂Č������Ă�����
            else if (enemyInfo.status.isTargetLost == false && tag != "Player")
            {
                // ���X�g�|�W�V������ݒ�
                enemyInfo.status.lostPos = enemyInfo.playerStatus.playerPos;
                enemyInfo.status.isTargetLost = true;

                Debug.Log("��������");
            }
            // �Ō�̌��������n�_�ɓ��B���ĂȂ��������Ȃ�������
            else if (Vector3.Distance(enemyInfo.status.lostPos, enemyInfo.status.position) < 2.0f && tag != "Player" && enemyInfo.status.isTargetLost)
            {
                vigilance = true;

                Debug.Log("�T���ɖ߂�");
            }

            if (enemyInfo.status.isTargetLost)
            {
                Debug.DrawLine(enemyInfo.status.position, enemyInfo.status.lostPos, Color.magenta);
            }
        }
    }
    private float Template(Vector3 point1)
    {
        float temp;

        temp = Mathf.Atan2(point1.z, point1.x);

        return temp;
    }

    private float Template(Vector3 point1, Vector3 point2)
    {
        float temp;

        temp = Mathf.Atan2(point1.z - point2.z, point1.x - point2.x);

        return temp;
    }

    // �ڕW�ʒu�̎擾
    public void GetTarget(EnemyInfo info)
    {
        // �^�[�Q�b�g�̏��擾
        enemyInfo = info;
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


    // ���̍X�V
    public void StatusUpdate()
    {
        // �X�e�[�g�̐؂�ւ�
        if (vigilance) enemyInfo.status.state = State.VIGILANCE;
    }

    // �ړ�
    public void Move()
    {
        // �ڕW�ʒu��ݒ�
        if(enemyInfo.status.isTargetLost)�@enemyInfo.status.targetPos = enemyInfo.status.lostPos; // �������Ă�����
        else enemyInfo.status.targetPos = enemyInfo.playerStatus.playerPos;                       // �ǐՒ�

    }
}
