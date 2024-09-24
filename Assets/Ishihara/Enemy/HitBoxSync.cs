using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBoxSync : MonoBehaviour
{
    BoxCollider boxCollider;
    Bounds bounds;
    public GameObject Bounds;
    [SerializeField] GameObject parent;

    private Quaternion initialLocalRotation;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
        bounds = Bounds.GetComponent<SkinnedMeshRenderer>().bounds;// 子オブジェクトの初期ローカル回転を保存
        initialLocalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        // 親オブジェクトの回転を取得
        Quaternion parentRotation = transform.parent.rotation;

        // 子オブジェクトのローカル回転を、親の逆回転と初期回転に設定
        transform.localRotation = Quaternion.Inverse(parentRotation) * initialLocalRotation;

        // SkinnedMeshRendererのBoundsを取得
        var bounds = Bounds.GetComponent<SkinnedMeshRenderer>().bounds;

        // BoxColliderのローカル座標での中心を計算
        var localCenter = Quaternion.Inverse(boxCollider.gameObject.transform.rotation) *
            new Vector3(
            (bounds.center.x - boxCollider.gameObject.transform.position.x) / boxCollider.gameObject.transform.lossyScale.x,
            (bounds.center.y - boxCollider.gameObject.transform.position.y) / boxCollider.gameObject.transform.lossyScale.y,
            (bounds.center.z - boxCollider.gameObject.transform.position.z) / boxCollider.gameObject.transform.lossyScale.z);

        // BoxColliderの中心を設定
        boxCollider.center = localCenter;

        // BoxColliderのローカル座標でのサイズを計算
        var localSize = new Vector3(
            bounds.size.x / boxCollider.gameObject.transform.lossyScale.x,
            bounds.size.y / boxCollider.gameObject.transform.lossyScale.y,
            bounds.size.z / boxCollider.gameObject.transform.lossyScale.z);

        // BoxColliderのサイズを設定
        boxCollider.size = localSize;
    }
}
