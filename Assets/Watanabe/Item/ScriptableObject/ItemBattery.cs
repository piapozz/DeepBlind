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
[CreateAssetMenu(fileName = "ItemObject", menuName = "ScriptableObjects/Battery")]

public class ItemBattery : BaseItem
{
    [SerializeField] private int recoveryValue;

    public override void ItemEffect(GameObject character)
    {
        LightManager.instance.SetBattery(recoveryValue);
    }
}
