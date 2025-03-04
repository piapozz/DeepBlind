/*
 * @file Inventory.cs
 * @brief ƒCƒ“ƒxƒ“ƒgƒŠ‚ğŠÇ—
 * @author sein
 * @date 2025/1/18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<BaseItem> itemList = new List<BaseItem>();

    static public Inventory instance { get; private set; } = null;

    public void Awake()
    {
        instance = this;
    }

    public void AddItem(BaseItem item)
    {
        itemList.Add(item);
    }

    public void RemoveItem(BaseItem item)
    {
        itemList.Remove(item);
    }

    public void UseItem(int index, GameObject character)
    {
        if (index >= 0 && index < itemList.Count)
        {
            itemList[index].ItemEffect(character);
        }
    }

    public void ClearItems()
    {
        itemList.Clear();
    }
}