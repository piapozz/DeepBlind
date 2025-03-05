/*
 * @file ItemKey.cs
 * @brief 鍵を実装
 * @author sein
 * @date 2025/1/18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 派生クラス：消耗品アイテム
[CreateAssetMenu(fileName = "ItemObject", menuName = "ScriptableObjects/Items/Key")]

public class ItemKey : BaseItem
{
    public float distance = 5f;
    public LayerMask doorLayer;

    public override bool ItemEffect()
    {
        Player player = Player.instance;
        Ray ray = new Ray(player.GetPosition(), player.GetTransform().forward);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance, doorLayer))
        {
            if (hit.collider.CompareTag("Door"))
            {
                EventDoor eventDoor = hit.collider.GetComponent<EventDoor>();
                if (eventDoor.doorLock == true)
                {
                    AudioManager.instance.PlaySE(SE.DOOR_UNLOCK);
                    eventDoor.UnlockDoor();
                    return true;
                }
            }
        }
        return false;
    }
}
