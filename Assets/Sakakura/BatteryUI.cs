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
        // FIX:�����̌y��string�̍����ɂ���
        _TMP.text = LightManager.instance.GetBatteryPower().ToString() + "%";
    }
}
