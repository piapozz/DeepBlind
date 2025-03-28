using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilingMaterial : MonoBehaviour
{
    float defaultTiling = 10.0f;

    void Start()
    {
        Material material = gameObject.GetComponent<Renderer>().material;
        if (material.mainTexture == null) return;

        Vector3 scale = gameObject.transform.localScale;
        Vector2 tiling = new Vector2(scale.x, scale.z) * defaultTiling;

        material.mainTextureScale = tiling;
    }
}
