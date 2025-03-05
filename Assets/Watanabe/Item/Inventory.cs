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

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<BaseItem> itemList = null;
    [SerializeField] private Image mainSlotUI = null;
    [SerializeField] private Sprite iconBatsu = null;

    private int selectedSlot = -1;
    private readonly int SELECTEDSLOT_INITIAL = 0;

    static public Inventory instance { get; private set; } = null;

    public void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        itemList = new List<BaseItem>();
        selectedSlot = SELECTEDSLOT_INITIAL;
    }

    // マウスホイールでアイテム変更の処理
    public void Update()
    {
        Debug.Log(selectedSlot);

        if (IsEmpty(itemList))
        {
            mainSlotUI.sprite = iconBatsu;
            return;
        }

        int inventoryCount = itemList.Count;
        if (selectedSlot < inventoryCount)
        {
            mainSlotUI.sprite = itemList[selectedSlot].icon;
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

    public void AddItem(BaseItem item)
    {
        itemList.Add(item);
    }

    public void RemoveItem(BaseItem item)
    {
        itemList.Remove(item);
    }

    public void ClearItems()
    {
        itemList.Clear();
    }

    public void ChangeSlot(int value)
    {

    }

    public void UseItem(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (IsEmpty(itemList)) return;

        BaseItem useItem = itemList[selectedSlot];
        // アイテムの効果を実行 ※falseが失敗 trueが成功
        bool result = useItem.ItemEffect();

        if (!result) return;
        if (useItem.isConsume == true)
        {
            // if(useItem.canStack == false) itemList.RemoveAt(selectedSlot);
            itemList.RemoveAt(selectedSlot);
            if (selectedSlot != 0) selectedSlot--;
        }
    }
}