using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventItem : MonoBehaviour, IEvent
{
    private UIManager _uiManager = null;
    private Inventory _inventory = null;

    [SerializeField] private BaseItem item = null;

    void Start()
    {
        _uiManager = UIManager.instance;
        _inventory = Inventory.instance;
    }

    public void Event()
    {
        _inventory.AddItem(item);
        Destroy(this.gameObject);
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
