using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseInventorySlot
{
    private GameObject _itemObject;
    private ItemBase _item;
    private int _itemCount;
    private readonly int _ITEM_COUNT_INITIAL = 1;

    public void Setup(GameObject itemPrefab)
    {
        _itemObject = itemPrefab;
        _item = _itemObject.GetComponent<ItemBase>();
        _itemCount = _ITEM_COUNT_INITIAL;
    }

    public void Teardown()
    {
        ObjectActive(false);
        _item = null;
        _itemCount = -1;
    }

    public ItemBase GetItem() {  return _item; }
    public Sprite GetItemIcon() { return _item.itemIcon; }
    public int GetItemCount() { return _itemCount; }

    public void AddCount(int value) { _itemCount += value; }
    public void RemoveCount(int value) { _itemCount -= value; }
    public void SetCount(int value) { _itemCount = value; }
    public void ObjectActive(bool active) { _itemObject.SetActive(active); }
}
