using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
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

    int GetItemNum() { return itemNum; }
}
