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
    [SerializeField] private List<BaseInventorySlot> itemsList = null;
    [SerializeField] private Sprite iconBatsu = null;
    private readonly int SELECTEDSLOT_INITIAL = 0;
    private readonly int INVENTORYSLOT_INITIAL = 5;
    private UIManager uiManager = null;
    private Transform _playerTransform = null;
    private Transform itemAnkerTransform = null;
    private int _selectedSlot = -1;

    static public InventoryManager instance { get; private set; } = null;

    public override void Initialize()
    {
        instance = this;
        itemsList = new List<BaseInventorySlot>(INVENTORYSLOT_INITIAL);
        _selectedSlot = SELECTEDSLOT_INITIAL;
        uiManager = UIManager.instance;
        _playerTransform = Player.instance.transform;
        itemAnkerTransform = Player.instance.itemAnker;
    }

    // マウスホイールでアイテム変更の処理
    public void Update()
    {
        if (IsEmpty(itemsList))
        {
            uiManager.ViewItemSlot(uiManager.noneItemIcon, -1);
            return;
        }

        // 選択アイテムの切り替え
        ChangeSlot();

        // 持っていないアイテムを非表示
        for (int i = 0, max = itemsList.Count; i < max; i++)
        {
            if (_selectedSlot != i) itemsList[i].ObjectActive(false);
            else itemsList[i].ObjectActive(true);
        }

        // アイテムのパッシブ効果
        ItemBase item = itemsList[_selectedSlot].GetItem();
        item.Proc();
        item.FollowCamera();
    }

    public void AddItem(GameObject item)
    {
        // アイテムを生成
        GameObject itemObject = Instantiate(item, itemAnkerTransform.position, itemAnkerTransform.rotation, _playerTransform);
        ItemBase itemBase = itemObject.GetComponent<ItemBase>();
        itemBase.Initialize();
        // ItemBase itemBase = item.GetComponent<ItemBase>();
        // アイテムのスタック処理
        for (int i = 0, max = itemsList.Count; i < max; i++)
        {
            if (itemsList[i].GetItem().itemName == itemBase.itemName)
            {
                if (itemsList[i].GetItem().canStack == false) break;
                itemsList[i].AddCount(1);
                itemObject.SetActive(false);
                return;
            }
        }
        // すでに所持していなかったらスロットを生成
        BaseInventorySlot inventorySlot = new BaseInventorySlot();
        // アイテムを生成
        // GameObject itemObject = Instantiate(item, itemAnkerTransform.position, itemAnkerTransform.rotation, _playerTransform);
        // スロットの初期設定
        inventorySlot.Setup(itemObject);
        // スロットをリストに追加
        itemsList.Add(inventorySlot);
    }

    public void ClearItems()
    {
        itemsList.Clear();
    }

    public void ChangeSlot()
    {
        int inventoryCount = itemsList.Count;
        if (_selectedSlot < inventoryCount)
        {
            Sprite itemSprite = itemsList[_selectedSlot].GetItemIcon();
            int itemCount = itemsList[_selectedSlot].GetItemCount();
            if (!itemsList[_selectedSlot].GetItem().canStack) itemCount = -1;
            uiManager.ViewItemSlot(itemSprite, itemCount);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            _selectedSlot = (_selectedSlot - 1 + inventoryCount) % inventoryCount;
        }
        else if (scroll < 0f)
        {
            _selectedSlot = (_selectedSlot + 1) % inventoryCount;
        }
    }

    public void UseItemEffect()
    {
        if (IsEmpty(itemsList)) return;

        // 使用予定のアイテムを取得
        ItemBase useItem = itemsList[_selectedSlot].GetItem();
        // 持つだけのアイテムだったら除外
        if (useItem.isPassive) return;
        // アイテムの効果を実行 ※trueがアイテムを消費 falseがアイテム消費なし
        bool result = useItem.Effect();
        if (!result) return;
        if (!useItem.isConsume) return;
        if (itemsList[_selectedSlot].GetItemCount() >= 1) itemsList[_selectedSlot].RemoveCount(1);
        if (itemsList[_selectedSlot].GetItemCount() <= 0)
        {
            itemsList[_selectedSlot].Teardown();
            itemsList.RemoveAt(_selectedSlot);
            if (_selectedSlot != 0) _selectedSlot--;
        }
    }
}