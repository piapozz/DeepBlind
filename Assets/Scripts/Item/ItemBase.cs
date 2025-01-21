using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    private readonly float _ITEM_DISTANCE = 0.5f;
    private readonly float _ITEM_HEIGHT = 0.5f;

    private Transform _cameraTransform = null;

    enum ItemCategory
    {
        BatteryS = 0,
        BatteryL,
        Medicine,
        Map,
        Compass,
        Max
    }

    protected abstract void Init();
    protected abstract void Proc();

    private void Start()
    {
        Init();

        _cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        Proc();

        FollowCamera();
    }

    private void FollowCamera()
    {
        float angle = _cameraTransform.localEulerAngles.y;
        Vector3 offset = Vector3.zero;
        offset.x = Mathf.Sin(angle * Mathf.Deg2Rad) * _ITEM_DISTANCE;
        offset.y = -_ITEM_HEIGHT;
        offset.z = Mathf.Cos(angle * Mathf.Deg2Rad) * _ITEM_DISTANCE;
        transform.position = _cameraTransform.position + offset;
        transform.localEulerAngles = new Vector3(0, angle, 0);
    }
}
