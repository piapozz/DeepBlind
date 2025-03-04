/*
 * @file ItemMap.cs
 * @brief マップを実装
 * @author sein
 * @date 2025/1/18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 派生クラス：消耗品アイテム
[CreateAssetMenu(fileName = "ItemObject", menuName = "ScriptableObjects/Items/Map")]

public class ItemMap : BaseItem
{
    public override bool ItemEffect()
    {
        return true;
    }
}
