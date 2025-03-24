using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OutlineManager : MonoBehaviour
{
    public string targetTag = "ItemObject";
    public float maxDistance = 100f;
    private Camera mainCamera;
    private HashSet<Outline> outlineObjectList = new HashSet<Outline>();

    void Start()
    {
        mainCamera = Camera.main;
        ResetAllOutlines();
    }

    void Update()
    {
        // Outline状況のリセット
        ResetAllOutlines();

        // カメラの中央から Ray を飛ばす
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
        {
            // 当たったオブジェクトのタグを確認
            if (hit.collider.CompareTag(targetTag))
            {
                SetOutlineWidth(hit.collider.gameObject, 2f);
            }
        }
    }

    /// <summary>
    /// Outlineの幅を変更する
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="width"></param>
    private void SetOutlineWidth(GameObject obj, float width)
    {
        Outline outline = obj.GetComponent<Outline>();
        if (outline != null)
        {
            outline.OutlineWidth = width;
            outlineObjectList.Add(outline); // 管理リストに追加
        }
    }

    /// <summary>
    /// Outlineの幅をリセット
    /// </summary>
    private void ResetAllOutlines()
    {
        foreach (Outline outline in outlineObjectList)
        {
            if (outline != null)
            {
                outline.OutlineWidth = 0f;
            }
        }
        outlineObjectList.Clear();
    }

    //private Camera mainCamera = null;
    //private List<Outline> _itemOutlineList = null;
    //private List<Renderer> _itemRendererList = null;
    //private List<GameObject> _itemObjectList = null;

    //void Start()
    //{
    //    mainCamera = Camera.main;
    //    _itemObjectList = ObjectManager.instance.GetItemObjectList();

    //    for (int i = 0, max = _itemObjectList.Count; i < max; i++)
    //    {
    //        _itemRendererList[i] = _itemObjectList[i].GetComponent<Renderer>();
    //        _itemOutlineList[i] = _itemObjectList[i].GetComponent<Outline>();
    //        _itemOutlineList[i].enabled = false; // 初期は非表示
    //    }
    //}

    //void Update()
    //{
    //    Plane[] frustumPlanes = GeometryUtility.CalculateFrustumPlanes(mainCamera);

    //    for (int i = 0, max = _itemRendererList.Count; i < max; i++)
    //    {
    //        if (GeometryUtility.TestPlanesAABB(frustumPlanes, _itemRendererList[i].bounds))
    //        {
    //            Vector3 direction = _itemRendererList[i].bounds.center - mainCamera.transform.position;

    //            if (!Physics.Raycast(mainCamera.transform.position, direction, out RaycastHit hit) || hit.transform == transform)
    //            {
    //                _itemOutlineList[i].enabled = true;
    //            }
    //            else
    //            {
    //                _itemOutlineList[i].enabled = false; // 壁に隠れている
    //            }
    //        }
    //        else
    //        {
    //            _itemOutlineList[i].enabled = false;
    //        }
    //    }
    //}
}
