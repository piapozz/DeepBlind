using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Compass : ItemBase
{
    [SerializeField] Transform player;
    [SerializeField] GenerateStage generateStage;
    [SerializeField] Transform pin;

    Vector3 playerPos;
    Vector3 goalPos;
    float playerRot;
    float angle;

    protected override void Init()
    {
        // �S�[���̍��W���擾
        goalPos = generateStage.GetGoalPos();
    }

    protected override void Proc()
    {
        TurnTarget3D(goalPos);
    }

    void TurnCompass2D()
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

    private void TurnTarget3D(Vector3 targetPos)
    {
        Vector3 dir = targetPos - player.position;
        dir.y = player.position.y;
        Quaternion dirRot = Quaternion.LookRotation(dir);
        pin.rotation = dirRot;
    }
}
