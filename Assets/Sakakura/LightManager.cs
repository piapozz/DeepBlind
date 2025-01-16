using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightManager : MonoBehaviour
{
    private readonly float _BATTERY_MAX = 100;
    private readonly float _BATTERY_CONSUME = 0.01f;

    private Light _light = null;
    public bool nowSwitch { get; private set; } = true;
    public float batteryPower { get; private set; } = -1;
    public static LightManager instance { get; private set; } = null;

    private void Start()
    {
        instance = this;
        batteryPower = _BATTERY_MAX;
        _light = GetComponentInChildren<Light>();
    }

    private void Update()
    {
        ConsumeBattery();
    }

    /// <summary>
    /// バッテリーを消費する
    /// </summary>
    public void ConsumeBattery()
    {
        if (!nowSwitch || batteryPower <= 0) return;

        batteryPower -= _BATTERY_CONSUME;
        if (batteryPower < 0)
        {
            batteryPower = 0;
            SwitchLight();
        }
        Debug.Log("バッテリー" + batteryPower);
    }

    /// <summary>
    /// バッテリーを補充する
    /// </summary>
    /// <param name="setPower"></param>
    public void SetBattery(float addPower)
    {
        batteryPower += addPower;
        if (batteryPower > _BATTERY_MAX)
            batteryPower = _BATTERY_MAX;
    }

    public void OnSwitchLight(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (batteryPower <= 0) return;

        SwitchLight();
    }

    /// <summary>
    /// ライトのオンオフを切り替える
    /// </summary>
    public void SwitchLight()
    {
        if (nowSwitch)
        {
            SetLightParam(5, 0.5f);
            nowSwitch = false;
        }
        else
        {
            SetLightParam(10, 1.5f);
            nowSwitch = true;
        }
    }

    /// <summary>
    /// ライトのパラメータをセットする
    /// </summary>
    /// <param name="range"></param>
    /// <param name="intensity"></param>
    private void SetLightParam(float range, float intensity)
    {
        _light.range = range;
        _light.intensity = intensity;
    }
}
