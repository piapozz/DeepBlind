using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] UIManager uiManager;
    [SerializeField] int itemNum;

    enum ItemCategory
    {
        BatteryS = 0,
        BatteryL,
        Medicine,
        Map,
        Compass,
        Max
    }

    // �A�C�e������肷��֐�
    void GetItem()
    {
        // �A�^�b�`����Ă���A�C�e���̔ԍ��ŕ���
        switch ((ItemCategory)itemNum)
        {
            // �o�b�e���[S
            case ItemCategory.BatteryS:

                break;
            // �o�b�e���[L
            case ItemCategory.BatteryL:

                break;
            // ��
            case ItemCategory.Medicine:

                break;
            // �n�}
            case ItemCategory.Map:
                uiManager.DisplayMap();
                break;
            // �R���p�X
            case ItemCategory.Compass:
                uiManager.DisplayCompass();
                break;
            default: break;
        }
    }
}
