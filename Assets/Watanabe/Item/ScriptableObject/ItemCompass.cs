/*
 * @file ItemCompass.cs
 * @brief �R���p�X������
 * @author sein
 * @date 2025/1/18
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �h���N���X�F���Օi�A�C�e��
[CreateAssetMenu(fileName = "ItemObject", menuName = "ScriptableObjects/Items/Compass")]

public class ItemCompass : BaseItem
{
    public override bool ItemEffect()
    {
        return true;
    }
}

