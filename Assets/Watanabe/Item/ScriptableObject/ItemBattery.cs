/*
 * @file ItemBattery.cs
 * @brief バッテリーを実装
 * @author sein
 * @date 2025/3/17
 */

using UnityEngine;

public class ItemBattery : ItemBase
{
    [SerializeField] private int recoveryValue;
    private Player _player;

    public override void Proc()
    {
        FollowCamera();
    }
    public override bool Effect()
    {
        Player.instance.selfLight.SetBattery(recoveryValue);
        return true;
    }

}
