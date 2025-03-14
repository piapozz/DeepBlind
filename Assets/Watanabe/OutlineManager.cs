using UnityEngine;

public class OutlineManager : MonoBehaviour
{
    public Camera mainCamera;  // プレイヤーのカメラ
    public float maxDistance = 10f; // レイの最大距離
    public float maxOutlineStrength = 1.5f; // 最大アウトライン強度
    public float fadeDistance = 3f; // アウトラインが弱まる距離
    public string targetTag = "ItemObject"; // タグを設定

    private Material lastHitMaterial; // 最後にヒットしたオブジェクトのマテリアル

    void Update()
    {
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            if (hit.collider.CompareTag(targetTag)) // 特定のタグを持つオブジェクトのみ適用
            {
                Debug.Log("bbb");
                Renderer rend = hit.collider.GetComponent<Renderer>();
                if (rend != null)
                {
                    Material mat = rend.material;
                    lastHitMaterial = mat;

                    float distance = hit.distance;
                    float outlineStrength = Mathf.Lerp(maxOutlineStrength, 0, distance / fadeDistance);
                    mat.SetFloat("_OutlineStrength", outlineStrength);
                }
            }
        }
        else
        {
            // ヒットしなかったらアウトラインを消す
            if (lastHitMaterial != null)
            {
                lastHitMaterial.SetFloat("_OutlineStrength", 0);
                lastHitMaterial = null;
            }
        }
    }
}
