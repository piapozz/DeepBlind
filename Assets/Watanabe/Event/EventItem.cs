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
    /// èEÇ¶ÇÈÇ‡ÇÃÇ™Ç†Ç¡ÇΩÇÁUIÇï\é¶
    /// </summary>
    public void EnableInteractUI()
    {
        _uiManager.DisplayIntractUI("Pick:E");
    }

    /// <summary>
    /// ó£ÇÍÇΩÇ∆Ç´Ç…ï\é¶Ç≥ÇÍÇƒÇ¢ÇÈUIÇè¡Ç∑
    /// </summary>
    public void DisableInteractUI()
    {
        _uiManager.DisableIntractUI();
    }
}
