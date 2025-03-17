/*
 * @file ItemBattery.cs
 * @brief �o�b�e���[������
 * @author sein
 * @date 2025/1/18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �h���N���X�F���Օi�A�C�e��
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
