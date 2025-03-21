using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [SerializeField] private ItemData itemData;

    public GameObject itemModel {  get; private set; } = null;
    public Sprite itemIcon { get; private set; } = null;
    public string itemName { get; private set; }
    public bool canStack { get; private set; }
    public bool isConsume { get; private set; }
    public bool isPassive { get; private set; }
     
    private readonly float _ITEM_DISTANCE = 0.4f;
    private readonly float _ITEM_HEIGHT = 0.5f;
    private readonly float _ITEM_WIDTH = 0.5f;

    private Transform _cameraTransform = null;

    public virtual void Initialize()
    {
        itemName = itemData.itemName;
        itemIcon = itemData.itemIcon;
        canStack = itemData.canStack;
        isConsume = itemData.isConsume;

        _cameraTransform = Camera.main.transform;
    }

    /// <summary>
    /// アイテムのパッシブ効果を実装
    /// </summary>
    public abstract void Proc();

    /// <summary>
    /// アイテムのアクティブ効果を実装
    /// 戻り値がtrueならアイテムを消費、falseならアイテムの消費をなし
    /// </summary>
    /// <returns></returns>
    public abstract bool Effect();

    public void FollowCamera()
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
