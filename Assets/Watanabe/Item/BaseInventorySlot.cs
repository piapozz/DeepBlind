using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInventorySlot
{
    public GameObject itemObject;
    public ItemBase item;
    public int itemCount;
    private readonly int _ITEM_COUNT_INITIAL = 1;

    public void Setup(GameObject itemPrefab)
    {
        itemObject = itemPrefab;
        item = itemObject.GetComponent<ItemBase>();
        itemCount = _ITEM_COUNT_INITIAL;
    }

    public void ObjectActive(bool active) { itemObject.SetActive(active); }
}
