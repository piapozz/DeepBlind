using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// 見ている間停止する
public class StopLookAt : ISkill
{
    [SerializeField]
    private GameObject _mesh;           // メッシュ

    private int ID;                     // ID
    private  Bounds bounds;             // メッシュの範囲

    public float detectionRange = 5.0f; // 近づいたとみなす距離
    private bool canCollide = false;    // 当たり判定を持つか

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="animator"></param>
    public void Init(int setID)
    {
        ID = setID;
    }

    /// <summary>
    /// 見てると止まる
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public void Ability()
    {
        if(EnemyManager.instance.Get(ID) ==null) return;
        EnemyBase enemy = EnemyUtility.GetCharacter(ID);
        Transform player = EnemyUtility.GetPlayer().transform;
        SkinnedMeshRenderer filter = enemy.GetComponentInChildren<SkinnedMeshRenderer>();
        bounds = filter.bounds;
        detectionRange = bounds.size.magnitude / 2;
        Vector3[] targetPoints = new Vector3[8];
        // メッシュの8つの頂点を取得
        targetPoints[0] = bounds.min + new Vector3(0.0f, 0.5f, 0.0f);
        targetPoints[1] = new Vector3(bounds.max.x, bounds.min.y + 0.5f, bounds.min.z);
        targetPoints[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        targetPoints[3] = new Vector3(bounds.min.x, bounds.min.y + 0.5f, bounds.max.z);
        targetPoints[4] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
        targetPoints[5] = new Vector3(bounds.max.x, bounds.min.y + 0.5f, bounds.max.z);
        targetPoints[6] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
        targetPoints[7] = bounds.max;
        // 接触間近範囲なら
        float length = EnemyUtility.EnemyToPlayerLength(ID);
        bool isCloseEnough = false;
        if (length < detectionRange)
        {
            isCloseEnough = true;
        }
        else
        {
            isCloseEnough = false;
        }

        // カメラ内にオブジェクトがあるかどうか
        bool isInsideCamera = false;
        // ターゲットポイントがカメラのビューポート内にあるかどうかを調べる
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
                Vector3 direction = -(targetPoint - player.position);
                Ray ray = new Ray(targetPoint, direction.normalized);
                RaycastHit hit;
                // レイキャストがプレイヤーに直接当たるか確認
                LayerMask layer = LayerMask.GetMask("Player") | LayerMask.GetMask("Stage");
                if (Physics.Raycast(ray, out hit, direction.magnitude + 1, layer))
                {
                    // 障害物がなく直接当たった場合に true を返す
                    if (hit.collider.CompareTag("Player"))
                    {
                        isInsideCamera = true;
                    }
                }
            }
        }
        // カメラ内にオブジェクトがあるかどうか
        // 背後から接触間近まで近づいたか
        if (isCloseEnough && isInsideCamera && !enemy.isAbility)
        {
            canCollide = true;
        }
        else if (isInsideCamera)
        {
            canCollide = false;
        }
        else
        {
            canCollide = true;
        }

        if (!canCollide)
        {
            // 停止
            enemy.SetAnimationSpeed(0);
            enemy.SetNavSpeed(0);
            if(!enemy.isAbility) enemy.SetIsAbility();
        }
        else
        {
            // 再開
            enemy.SetAnimationSpeed(Mathf.Min(enemy.speed, 2));
            enemy.SetNavSpeed(enemy.speed);
            if (enemy.isAbility) enemy.SetIsAbility();
        }
    }
}
