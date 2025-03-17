using System.Collections.Generic;
using UnityEngine;

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

        else
        {
            ResetAllOutlines();
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
}
