using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static EnemyBase;

public class StopLookAt : ISkill
{

    [SerializeField]
    private GameObject _mesh;

    private Animator _animator;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="animator"></param>
    public void Init()
    {
    }

    /// <summary>
    /// 見てると止まる
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public void Ability()
    {
        SkinnedMeshRenderer filter = _mesh.GetComponent<SkinnedMeshRenderer>();

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

        //　カメラ内にオブジェクトがあるかどうか
        bool isInsideCamera = false;

        //　ターゲットポイントがカメラのビューポート内にあるかどうかを調べる
        foreach (var targetPoint in targetPoints)
        {
            Plane[] planes;
            Camera camera = Camera.main;
            // カメラの視錐台を求める
            planes = GeometryUtility.CalculateFrustumPlanes(camera);

            // カメラに写っているか判定
            if (GeometryUtility.TestPlanesAABB(planes, bounds))
            {
                // コーナーからカメラ位置へのレイキャスト
                Vector3 direction = -(targetPoint - camera.transform.position);
                Ray ray = new Ray(targetPoint, direction.normalized);
                RaycastHit hit;
                // レイキャストがプレイヤーに直接当たるか確認
                if (Physics.Raycast(ray, out hit, direction.magnitude + 1))
                {
                    // 障害物がなく直接当たった場合に true を返す
                    if (hit.collider.CompareTag("Player"))
                    {
                        isInsideCamera = true;
                    }
                }
            }
        }
    }
}
