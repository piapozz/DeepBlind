using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] GenerateStage generateStage;

    Vector3 playerPos;
    Vector3 goalPos;
    float playerRot;
    float angle;

    void Start()
    {
        // �S�[���̍��W���擾
        goalPos = generateStage.GetGoalPos();
    }

    void Update()
    {
        // �v���C���[�����X�V
        playerPos = player.position;
        playerRot = player.localEulerAngles.y;
        // �S�[���ƃv���C���[�Ƃ̊p�x�����߂�
        Vector2 dir = new Vector2(goalPos.x - playerPos.x, goalPos.z - playerPos.z);
        // �S�[���Ƃ̊p�x�ƃv���C���[�̉�]����p�x�����߂�
        angle = Mathf.Atan2(dir.y, dir.x) * 180 / Mathf.PI - 90 + playerRot;
        gameObject.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
