using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EventItem : MonoBehaviour, IEvent
{
    private bool isUsed = false;

    [SerializeField] private GameObject item = null;

    public void Event()
    {
        if (isUsed != true)
        {
            AudioManager.instance.PlaySE(SE.ITEM_PICK);
            InventoryManager.instance.AddItem(item);
            UIManager.instance.DisableIntractUI();
            isUsed = true;
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// èEÇ¶ÇÈÇ‡ÇÃÇ™Ç†Ç¡ÇΩÇÁUIÇï\é¶
    /// </summary>
    public void EnableInteractUI()
    {
        UIManager.instance.DisplayIntractUI("Pick:E");
    }

    /// <summary>
    /// ó£ÇÍÇΩÇ∆Ç´Ç…ï\é¶Ç≥ÇÍÇƒÇ¢ÇÈUIÇè¡Ç∑
    /// </summary>
    public void DisableInteractUI()
    {
        UIManager.instance.DisableIntractUI();
    }
}
