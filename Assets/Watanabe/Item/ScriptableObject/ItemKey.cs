/*
 * @file ItemKey.cs
 * @brief Œ®‚ðŽÀ‘•
 * @author sein
 * @date 2025/3/17
 */

using UnityEngine;

public class ItemKey : ItemBase
{
    public float distance = 5f;
    public LayerMask doorLayer;

    public override void Initialize()
    {
        base.Initialize();
    }
    public override void Proc()
    {
        FollowCamera();
    }
    public override bool Effect()
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

