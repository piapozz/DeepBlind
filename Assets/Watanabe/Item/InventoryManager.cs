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
    [SerializeField] private List<BaseInventorySlot> itemsList = null;
    [SerializeField] private Sprite iconBatsu = null;
    private readonly int SELECTEDSLOT_INITIAL = 0;
    private readonly int INVENTORYSLOT_INITIAL = 5;
    private Image _mainSlotUI = null;
    private int _selectedSlot = -1;

    static public InventoryManager instance { get; private set; } = null;

    public override void Initialize()
    {
        instance = this;
        _mainSlotUI = UIManager.instance.mainSlotUI;
        itemsList = new List<BaseInventorySlot>(INVENTORYSLOT_INITIAL);
        _selectedSlot = SELECTEDSLOT_INITIAL;
    }

    // �}�E�X�z�C�[���ŃA�C�e���ύX�̏���
    public void Update()
    {
        if (IsEmpty(itemsList))
        {
            _mainSlotUI.sprite = iconBatsu;
            return;
        }

        // �I���A�C�e���̐؂�ւ�
        ChangeSlot();

        // �����Ă��Ȃ��A�C�e�����\��
        for (int i = 0, max = itemsList.Count; i < max; i++)
        {
            if (_selectedSlot != i) itemsList[i].ObjectActive(false);
            else itemsList[i].ObjectActive(true);
        }
        // �A�C�e���̃p�b�V�u����
        ItemBase item = itemsList[_selectedSlot].item;
        item.Proc();
    }

    public void AddItem(GameObject item)
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

        // ���łɏ������Ă��Ȃ�������X���b�g�𐶐�
        BaseInventorySlot inventorySlot = new BaseInventorySlot();
        // �A�C�e���𐶐�
        GameObject itemObject = Instantiate(item);
        // �X���b�g�̏����ݒ�
        inventorySlot.Setup(itemObject);
        // �X���b�g���̃A�C�e����������
        inventorySlot.item.Initialize();
        // �X���b�g�����X�g�ɒǉ�
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
            _mainSlotUI.sprite = itemsList[_selectedSlot].item.icon;
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

    public void UseItemEffect(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (IsEmpty(itemsList)) return;

        // �g�p�\��̃A�C�e�����擾
        ItemBase useItem = itemsList[_selectedSlot].item;
        // �������̃A�C�e���������珜�O
        if(useItem.isPassive == true) return;

        // �A�C�e���̌��ʂ����s ��true���A�C�e�������� false���A�C�e������Ȃ�
        bool result = useItem.Effect();

        // �A�C�e���̌��ʂ����s�����珜�O
        if (!result) return;
        // ����A�C�e������������s
        if (useItem.isConsume == true)
        {
            if (itemsList[_selectedSlot].itemCount >= 1) itemsList[_selectedSlot].itemCount -= 1;
            // �A�C�e�����Ȃ��Ȃ����烊�X�g����폜
            if (itemsList[_selectedSlot].itemCount <= 0)
            {
                itemsList.RemoveAt(_selectedSlot);
                if (_selectedSlot != 0) _selectedSlot--;
            }
        }
    }
}