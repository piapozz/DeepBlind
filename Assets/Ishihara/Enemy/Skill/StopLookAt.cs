using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyBase;

public class StopLookAt : ISkill
{

    [SerializeField] GameObject mesh;

    // 見てると止まる
    // プレイヤーが敵を見つけた時、敵もプレイヤーを見つける
    public EnemyBase.EnemyInfo Ability(EnemyBase.EnemyInfo info)
    {
        // メッシュフィルターの存在確認
        SkinnedMeshRenderer filter = mesh.GetComponent<SkinnedMeshRenderer>();

        // フィルターのメッシュ情報からバウンドボックスを取得する
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

        // 各コーナーがカメラのビューポートに収まっているかをチェック

        //　カメラ内にオブジェクトがあるかどうか
        bool isInsideCamera = false;
        //　ターゲットポイントがカメラのビューポート内にあるかどうかを調べる
        foreach (var targetPoint in targetPoints)
        {
            Plane[] planes;

            // カメラの視錐台を求める
            planes = GeometryUtility.CalculateFrustumPlanes(info.playerStatus.cam);

            // カメラに写っているか判定
            if (GeometryUtility.TestPlanesAABB(planes, bounds))
            {
                // コーナーからカメラ位置へのレイキャスト
                Vector3 direction = -(targetPoint - info.playerStatus.cam.transform.position);
                Ray ray = new Ray(targetPoint, direction.normalized);
                RaycastHit hit;
                // レイキャストがプレイヤーに直接当たるか確認
                if (Physics.Raycast(ray, out hit, direction.magnitude + 1))
                {
                    // レイを描画する
                    // Debug.DrawLine(ray.origin, ray.origin + ray.direction * (direction.magnitude + 1), Color.green, 0.01f);
                    Debug.DrawLine(ray.origin,hit.point, Color.green, 0.01f);
                    

                    // 障害物がなく直接当たった場合に true を返す
                    if (hit.collider.CompareTag("Player"))
                    {
                        isInsideCamera = true;
                    }
                }
            }
        }

        if (isInsideCamera)
        {
            //映っていたら制止する
            info.animator.speed = 0.0f;                        // アニメーションの再生を停止^
            info.status.isAblity = true;
            //info.status.state = EnemyBase.State.TRACKING;
        }
        else
        {
            info.animator.speed = info.animSpeed;   // 通常再生
            info.status.isAblity = false;
        }


        return info;
    }
}
