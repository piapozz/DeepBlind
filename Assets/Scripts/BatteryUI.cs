using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BatteryUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _TMP = null;

    private void Update()
    {
        // FIX:処理の軽いstringの合成にする
        _TMP.text = Light.instance.GetBatteryPower().ToString() + "%";
    }
}
