using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineManager : MonoBehaviour
{
    //public string targetTag = "ItemObject";
    //public float maxDistance = 100f;
    //private Camera mainCamera;
    //private HashSet<Outline> outlineObjectList = new HashSet<Outline>();

    //void Start()
    //{
    //    mainCamera = Camera.main;
    //    ResetAllOutlines();
    //}

    //void Update()
    //{
    //    // Outline状況のリセット
    //    ResetAllOutlines();

    //    // カメラの中央から Ray を飛ばす
    //    Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
    //    if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
    //    {
    //        // 当たったオブジェクトのタグを確認
    //        if (hit.collider.CompareTag(targetTag))
    //        {
    //            SetOutlineWidth(hit.collider.gameObject, 2f);
    //        }
    //    }
    //}

    ///// <summary>
    ///// Outlineの幅を変更する
    ///// </summary>
    ///// <param name="obj"></param>
    ///// <param name="width"></param>
    //private void SetOutlineWidth(GameObject obj, float width)
    //{
    //    Outline outline = obj.GetComponent<Outline>();
    //    if (outline != null)
    //    {
    //        outline.OutlineWidth = width;
    //        outlineObjectList.Add(outline); // 管理リストに追加
    //    }
    //}

    ///// <summary>
    ///// Outlineの幅をリセット
    ///// </summary>
    //private void ResetAllOutlines()
    //{
    //    foreach (Outline outline in outlineObjectList)
    //    {
    //        if (outline != null)
    //        {
    //            outline.OutlineWidth = 0f;
    //        }
    //    }
    //    outlineObjectList.Clear();
    //}

    public List<GameObject> _itemObjectList = new List<GameObject>(); // 既存のオブジェクトリスト
    private Plane[] frustumPlanes;
    private float checkInterval = 0.5f; // 0.5秒ごとに判定
    private float nextCheckTime = 0f;

    void Update()
    {
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            CheckVisibility();
        }
    }

    void CheckVisibility()
    {
        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);

        _itemObjectList = ObjectManager.instance.GetItemObjectList();

        foreach (GameObject obj in _itemObjectList)
        {
            if (obj == null) continue;

            Renderer rend = obj.GetComponent<Renderer>();
            if (rend == null)
            {
                rend = obj.GetComponent<Renderer>();
                continue;
            }

            Bounds bounds = rend.bounds;
            bool isVisible = GeometryUtility.TestPlanesAABB(frustumPlanes, bounds);

            Outline outline = obj.GetComponent<Outline>();
            if (outline != null)
            {
                outline.OutlineWidth = 2.0f;
            }
        }
    }
}
