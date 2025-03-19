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
            var obj = hit.collider;
            IEvent objEvent = obj.GetComponent<IEvent>();
            if (objEvent == null) return false;
            if(objEvent is EventDoor)
            {
                EventDoor eventDoor = objEvent as EventDoor;
                if (eventDoor.doorLock == true)
                {
                    AudioManager.instance.PlaySE(SE.DOOR_UNLOCK);
                    eventDoor.UnlockDoor();
                    return true;
                }
            }
            else if (objEvent is EventGoal)
            {
                EventGoal eventGoal = objEvent as EventGoal;
                if (eventGoal.canGoal != true)
                {
                    AudioManager.instance.PlaySE(SE.DOOR_UNLOCK);
                    eventGoal.UnlockDoor();
                    return true;
                }
            }
        }
        return false;
    }
}

