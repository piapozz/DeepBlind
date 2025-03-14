/*
 * @file Inventory.cs
 * @brief インベントリを管理
 * @author sein
 * @date 2025/1/18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using static CommonModule;

public class InventoryManager : SystemObject
{
    [SerializeField] private List<BaseItem> itemList = null;
    [SerializeField] private List<BaseInventorySlot> itemsList = null;

    [SerializeField] private Image mainSlotUI = null;
    [SerializeField] private Sprite iconBatsu = null;

    [SerializeField] private GameObject compassObject = null;
    [SerializeField] private GameObject mapObject = null;

    private int selectedSlot = -1;
    private readonly int SELECTEDSLOT_INITIAL = 0;
    private readonly int INVENTORYSLOT_INITIAL = 5;

    static public InventoryManager instance { get; private set; } = null;

    public override void Initialize()
    {
        instance = this;
        itemList = new List<BaseItem>();
        itemsList = new List<BaseInventorySlot>(INVENTORYSLOT_INITIAL);
        selectedSlot = SELECTEDSLOT_INITIAL;
    }
    public void Start()
    {
        instance = this;
        itemList = new List<BaseItem>();
        itemsList = new List<BaseInventorySlot>(INVENTORYSLOT_INITIAL);
        selectedSlot = SELECTEDSLOT_INITIAL;
    }

    // マウスホイールでアイテム変更の処理
    public void Update()
    {
        if (IsEmpty(itemsList))
        {
            mainSlotUI.sprite = iconBatsu;
            return;
        }

        ChangeSlot();

        // 手荷物系のアイテムだったら
        if (itemsList[selectedSlot].item is ItemMap)
        {
            mapObject.SetActive(true);
        }
        else { mapObject.SetActive(false); }

        if (itemsList[selectedSlot].item is ItemCompass)
        {
            compassObject.SetActive(true);
        }
        else { compassObject.SetActive(false); }
    }

    public void AddItem(BaseItem item)
    {
        // アイテムのスタック処理
        for (int i = 0, max = itemsList.Count; i < max; i++)
        {
            if (itemsList[i].item == item)
            {
                if (itemsList[i].item.canStack == false) break;
                itemsList[i].itemCount += 1;
                return;
            }
        }

        // すでに所持していなかったら
        BaseInventorySlot inventorySlot = new BaseInventorySlot();
        inventorySlot.item = item;
        inventorySlot.itemCount = 1;

        itemsList.Add(inventorySlot);
    }

    public void RemoveItem(BaseItem item)
    {
        itemList.Remove(item);
    }

    public void ClearItems()
    {
        itemsList.Clear();
    }

    public void ChangeSlot()
    {
        int inventoryCount = itemsList.Count;
        if (selectedSlot < inventoryCount)
        {
            mainSlotUI.sprite = itemsList[selectedSlot].item.icon;
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            selectedSlot = (selectedSlot - 1 + inventoryCount) % inventoryCount;
        }
        else if (scroll < 0f)
        {
            selectedSlot = (selectedSlot + 1) % inventoryCount;
        }
    }

    public void UseItem(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (IsEmpty(itemsList)) return;

        BaseItem useItem = itemsList[selectedSlot].item;
        // アイテムの効果を実行 ※falseが失敗 trueが成功
        bool result = useItem.ItemEffect();

        if (!result) return;
        if (useItem.isConsume == true)
        {
            // アイテム消費処理
            if (itemsList[selectedSlot].itemCount >= 1) itemsList[selectedSlot].itemCount -= 1;
            // アイテムがなくなったらリストから削除
            if (itemsList[selectedSlot].itemCount <= 0)
            {
                itemsList.RemoveAt(selectedSlot);
                if (selectedSlot != 0) selectedSlot--;
            }
        }
    }
}