/*
 * @file ItemBattery.cs
 * @brief バッテリーを実装
 * @author sein
 * @date 2025/1/18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 派生クラス：消耗品アイテム
[CreateAssetMenu(fileName = "ItemObject", menuName = "ScriptableObjects/Items/Battery")]

public class ItemBattery : BaseItem
{
    [SerializeField] private int recoveryValue;

    public override bool ItemEffect()
    {
        Light.instance.SetBattery(recoveryValue);
        return true;
    }
}
