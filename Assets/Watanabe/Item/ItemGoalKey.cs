/*
 * @file ItemKey.cs
 * @brief 鍵を実装
 * @author sein
 * @date 2025/3/17
 */

using UnityEngine;

public class ItemGoalKey : ItemBase
{
    public float distance = 5f;
    public LayerMask goalLayer;
    private Camera mainCamera = null;

    public override void Initialize()
    {
        base.Initialize();
        mainCamera = Player.instance.GetCamera();
    }
    public override void Proc()
    {
        FollowCamera();
    }
    public override bool Effect()
    {
        Player player = Player.instance;
        Camera mainCamera = Camera.main;

        if (mainCamera == null) return false;

        // 画面中央のピクセル座標を取得
        Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, distance, goalLayer))
        {
            var obj = hit.collider;
            IEvent objEvent = obj.GetComponent<IEvent>();
            if (objEvent == null) return false;

            if (objEvent is EventGoal eventGoal)
            {
                if (!eventGoal.canGoal)
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

