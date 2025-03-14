/*
 * @file Inventory.cs
 * @brief �C���x���g�����Ǘ�
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

    // �}�E�X�z�C�[���ŃA�C�e���ύX�̏���
    public void Update()
    {
        if (IsEmpty(itemsList))
        {
            mainSlotUI.sprite = iconBatsu;
            return;
        }

        ChangeSlot();

        // ��ו��n�̃A�C�e����������
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
        // �A�C�e���̃X�^�b�N����
        for (int i = 0, max = itemsList.Count; i < max; i++)
        {
            if (itemsList[i].item == item)
            {
                if (itemsList[i].item.canStack == false) break;
                itemsList[i].itemCount += 1;
                return;
            }
        }

        // ���łɏ������Ă��Ȃ�������
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
        // �A�C�e���̌��ʂ����s ��false�����s true������
        bool result = useItem.ItemEffect();

        if (!result) return;
        if (useItem.isConsume == true)
        {
            // �A�C�e�������
            if (itemsList[selectedSlot].itemCount >= 1) itemsList[selectedSlot].itemCount -= 1;
            // �A�C�e�����Ȃ��Ȃ����烊�X�g����폜
            if (itemsList[selectedSlot].itemCount <= 0)
            {
                itemsList.RemoveAt(selectedSlot);
                if (selectedSlot != 0) selectedSlot--;
            }
        }
    }
}