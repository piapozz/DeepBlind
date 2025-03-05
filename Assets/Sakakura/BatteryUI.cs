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
        // FIX:ˆ—‚ÌŒy‚¢string‚Ì‡¬‚É‚·‚é
        _TMP.text = LightManager.instance.GetBatteryPower().ToString() + "%";
    }
}
