using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventItem : MonoBehaviour, IEvent
{
    private UIManager _uiManager = null;
    private InventoryManager _inventory = null;

    private bool isUsed = false;

    [SerializeField] private BaseItem item = null;

    void Start()
    {
        _uiManager = UIManager.instance;
        _inventory = InventoryManager.instance;
    }

    public void Event()
    {
        if (isUsed != true)
        {
            AudioManager.instance.PlaySE(SE.ITEM_PICK);
            _inventory.AddItem(item);
            _uiManager.DisableIntractUI();
            isUsed = true;
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// �E������̂���������UI��\��
    /// </summary>
    public void EnableInteractUI()
    {
        _uiManager.DisplayIntractUI("Pick:E");
    }

    /// <summary>
    /// ���ꂽ�Ƃ��ɕ\������Ă���UI������
    /// </summary>
    public void DisableInteractUI()
    {
        _uiManager.DisableIntractUI();
    }
}
