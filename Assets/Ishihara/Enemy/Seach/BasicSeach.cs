using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnemyBase;
using static UnityEngine.UI.GridLayoutGroup;

public class BasicSeach : ISeach
{
    EnemyInfo enemyInfo = new EnemyInfo();
    
    bool tracking;              // ������

    // �s��
    public EnemyInfo Activity(EnemyInfo info)
    {
        // �擾
        GetTarget(info);

        // ���������ǂ���
        CheckTracking();

        // �x�������𖞂��������ǂ���
        CheckVigilance();

        // ���ꏈ��
        Ability();

        // �X�V
        StatusUpdate(info);

        return enemyInfo;
    }

    // ������
    public void Init()
    {
        enemyInfo = new EnemyInfo();

        tracking = false;              // ������
    }

    // ���������ǂ���
    public void CheckTracking()
    {
        // �v���C���[�Ƃ̊Ԃɏ�Q�������邩�ǂ���
        Vector3 origin = enemyInfo.status.position;                                                   // ���_
        Vector3 direction = Vector3.Normalize(enemyInfo.playerStatus.playerPos - enemyInfo.status.position);     // X��������\���x�N�g��
        Ray ray = new Ray(origin, direction);                                                    // Ray�𐶐�;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, enemyInfo.viewLength + 1))                                                       // ����Ray�𓊎˂��ĉ��炩�̃R���C�_�[�ɏՓ˂�����
        {
            Debug.DrawLine(ray.origin, enemyInfo.status.targetPos, Color.red,0.01f);
            Debug.DrawLine(ray.origin, ray.origin + (enemyInfo.status.dir * enemyInfo.viewLength), Color.blue, 0.01f);

            string tag = hit.collider.gameObject.tag;                                            // �Փ˂�������I�u�W�F�N�g�̖��O���擾
            
            if(tag != "Player") return;                                                          // �v���C���[�ȊO�Ȃ�I���

            float toPlayerAngle = Template(enemyInfo.status.position, enemyInfo.playerStatus.playerPos) * 180 / Mathf.PI;   // �v���C���[�ւ̊p�x
            float myAngle = Template(enemyInfo.status.dir);                                     // �����Ă�p�x

            // 0 ~ 360�ɃN�����v
            toPlayerAngle = Mathf.Repeat(toPlayerAngle, 360);
            myAngle = Mathf.Repeat(myAngle, 360);

            // ����͈͓��Ȃ�
            if (myAngle + (enemyInfo.fieldOfView / 2) > toPlayerAngle &&
                myAngle - (enemyInfo.fieldOfView / 2) < toPlayerAngle)
            {
                // ������
                tracking = true;
            }
        }
    }
    private float Template(Vector3 point1)
    {
        float temp;

        // point1.Normalize();

        temp = Mathf.Atan2(point1.z , point1.x);

        return temp;
    }

    private float Template(Vector3 point1 , Vector3 point2)
    {
        float temp;

        // point1.Normalize();
        // point2.Normalize();

        temp = Mathf.Atan2(point1.z - point2.z , point1.x - point2.x);

        return temp; 
    }


    // ���ꏈ��(�����Ă�����~�܂�)
    public void Ability()
    {
        //    Plane[] planes;

        //    // �J�����̎���������߂�
        //    planes = GeometryUtility.CalculateFrustumPlanes(enemyInfo.playerStatus.cam);

        //    // �J�����Ɏʂ��Ă��邩����
        //    if (GeometryUtility.TestPlanesAABB(planes, enemyInfo.bounds))
        //    {
        //        // �J�����ʒu����R�[�i�[�ւ̃��C�L���X�g
        //        Vector3 direction = enemyInfo.status.position - enemyInfo.playerStatus.cam.transform.position;
        //        Ray ray = new Ray(enemyInfo.playerStatus.cam.transform.position, direction.normalized);
        //        RaycastHit hit;
        //        // ���C�L���X�g���R�[�i�[�ɒ��ړ����邩�m�F
        //        if (Physics.Raycast(ray, out hit, direction.magnitude + 1))
        //        {
        //            Debug.DrawLine(ray.origin, hit.point, Color.yellow, 0.01f);

        //            // ��Q�����Ȃ����ړ��������ꍇ�� true ��Ԃ�
        //            if (hit.collider.tag == "Enemy")
        //            {
        //                // �f���Ă����琧�~����
        //                enemyInfo.status.targetPos = enemyInfo.status.position; // �ڕW�ʒu�����݈ʒu��
        //                enemyInfo.animator.speed = 0.0f;                        // �A�j���[�V�����̍Đ����~
        //                Debug.Log("�����Ă���");
        //                enemyInfo.status.isAblity = true;
        //            }
        //            else
        //            {
        //                enemyInfo.animator.speed = 1.0f;   // �ʏ�Đ�
        //                enemyInfo.status.isAblity = false;
        //            }
        //        }
        //        else
        //        {
        //            enemyInfo.animator.speed = 1.0f;   // �ʏ�Đ�
        //            enemyInfo.status.isAblity = false;
        //        }

        //    }
        //    else
        //    {
        //        enemyInfo.animator.speed = 1.0f;   // �ʏ�Đ�
        //        enemyInfo.status.isAblity = false;
        //    }

        //Vector3[] targetPoints = new Vector3[8];

        //targetPoints[0] = enemyInfo.bounds.min;
        //targetPoints[1] = new Vector3(enemyInfo.bounds.max.x, enemyInfo.bounds.min.y, enemyInfo.bounds.min.z);
        //targetPoints[2] = new Vector3(enemyInfo.bounds.min.x, enemyInfo.bounds.max.y, enemyInfo.bounds.min.z);
        //targetPoints[3] = new Vector3(enemyInfo.bounds.min.x, enemyInfo.bounds.min.y, enemyInfo.bounds.max.z);
        //targetPoints[4] = new Vector3(enemyInfo.bounds.max.x, enemyInfo.bounds.max.y, enemyInfo.bounds.min.z);
        //targetPoints[5] = new Vector3(enemyInfo.bounds.max.x, enemyInfo.bounds.min.y, enemyInfo.bounds.max.z);
        //targetPoints[6] = new Vector3(enemyInfo.bounds.min.x, enemyInfo.bounds.max.y, enemyInfo.bounds.max.z);
        //targetPoints[7] = enemyInfo.bounds.max;

        //// �e�R�[�i�[���J�����̃r���[�|�[�g�Ɏ��܂��Ă��邩���`�F�b�N

        ////�@�J�������ɃI�u�W�F�N�g�����邩�ǂ���
        //bool isInsideCamera = false;
        ////�@�J�����̃r���[�|�[�g�ʒu
        //Vector2 viewportPoint;
        ////�@�^�[�Q�b�g�|�C���g���J�����̃r���[�|�[�g���ɂ��邩�ǂ����𒲂ׂ�
        //foreach (var targetPoint in targetPoints)
        //{
        //    //�@�r���[�|�[�g�̌v�Z
        //    viewportPoint = Camera.main.WorldToViewportPoint(targetPoint);

        //    if (0f <= viewportPoint.x && viewportPoint.x <= 1f
        //        && 0f <= viewportPoint.y && viewportPoint.y <= 1f
        //        )
        //    {
        //        // �J�����ʒu����R�[�i�[�ւ̃��C�L���X�g
        //        Vector3 direction = targetPoint - enemyInfo.playerStatus.cam.transform.position;
        //        Ray ray = new Ray(enemyInfo.playerStatus.cam.transform.position, direction.normalized);
        //        RaycastHit hit;
        //        // ���C�L���X�g���R�[�i�[�ɒ��ړ����邩�m�F
        //        if (Physics.Raycast(ray, out hit, direction.magnitude + 1))
        //        {
        //            Debug.DrawLine(ray.origin, hit.point, Color.yellow , 0.01f);

        //            // ��Q�����Ȃ����ړ��������ꍇ�� true ��Ԃ�
        //            if (hit.collider.tag == "Enemy")
        //            {
        //                isInsideCamera = true;
        //            }
        //        }
        //    }
        //}

        //if (isInsideCamera)
        //{
        //    //�f���Ă����琧�~����
        //    enemyInfo.status.targetPos = enemyInfo.status.position; // �ڕW�ʒu�����݈ʒu��
        //    enemyInfo.animator.speed = 0.0f;                        // �A�j���[�V�����̍Đ����~
        //    enemyInfo.status.isAblity = true;
        //}
        //else
        //{
        //    enemyInfo.animator.speed = 1.0f;   // �ʏ�Đ�
        //    enemyInfo.status.isAblity = false;
        //}
        enemyInfo.animator.speed = 2.0f;
    }

        // �x�������𖞂��������ǂ���
        public void CheckVigilance()
    {

    }

    // �ڕW�ʒu�̎擾
    public void GetTarget(EnemyInfo info)
    {
        // �^�[�Q�b�g�̏��擾
        enemyInfo = info; 
    }

    // ���̍X�V
    public void StatusUpdate(EnemyInfo info)
    {
        //// ����
        //enemyInfo = info;

        //// �X�e�[�^�X�X�V
        //enemyInfo.status = enemyStatus;

        // �X�e�[�g�̐؂�ւ�
        if (tracking) enemyInfo.status.state = State.TRACKING;
    }

}
