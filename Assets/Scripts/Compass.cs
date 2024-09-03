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
        // ゴールの座標を取得
        goalPos = generateStage.GetGoalPos();
    }

    void Update()
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
}
