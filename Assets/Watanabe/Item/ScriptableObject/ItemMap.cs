/*
 * @file ItemMap.cs
 * @brief �}�b�v������
 * @author sein
 * @date 2025/1/18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �h���N���X�F���Օi�A�C�e��
[CreateAssetMenu(fileName = "ItemObject", menuName = "ScriptableObjects/Items/Map")]

public class ItemMap : BaseItem
{
    public override bool ItemEffect()
    {
        return true;
    }
}
