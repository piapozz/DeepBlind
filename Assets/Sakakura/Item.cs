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

    // アイテムを入手する関数
    void GetItem()
    {
        // アタッチされているアイテムの番号で分岐
        switch ((ItemCategory)itemNum)
        {
            // バッテリーS
            case ItemCategory.BatteryS:

                break;
            // バッテリーL
            case ItemCategory.BatteryL:

                break;
            // 薬
            case ItemCategory.Medicine:

                break;
            // 地図
            case ItemCategory.Map:
                uiManager.DisplayMap();
                break;
            // コンパス
            case ItemCategory.Compass:
                uiManager.DisplayCompass();
                break;
            default: break;
        }
    }
}
