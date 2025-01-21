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
        // ゴールの座標を取得
        goalPos = generateStage.GetGoalPos();
    }

    protected override void Proc()
    {
        TurnTarget3D(goalPos);
    }

    void TurnCompass2D()
    {
        // プレイヤー情報を更新
        playerPos = player.position;
        playerRot = player.localEulerAngles.y;
        // ゴールとプレイヤーとの角度を求める
        Vector2 dir = new Vector2(goalPos.x - playerPos.x, goalPos.z - playerPos.z);
        // ゴールとの角度とプレイヤーの回転から角度を求める
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
