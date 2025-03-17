using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Light
{
    private readonly float _BATTERY_MAX = 100;
    private readonly float _BATTERY_CONSUME = 0.01f;

    private UnityEngine.Light _light = null;
    public bool _nowSwitch = true;
    private float _batteryPower = -1;
    public static Light instance { get; private set; } = null;

    public void Initialize(UnityEngine.Light light)
    {
        instance = this;
        _batteryPower = _BATTERY_MAX;
        _light = light;
    }

    /// <summary>
    /// バッテリーを消費する
    /// </summary>
    public void ConsumeBattery()
    {
        if (!_nowSwitch || _batteryPower <= 0) return;

        _batteryPower -= _BATTERY_CONSUME;
        if (_batteryPower < 0)
        {
            _batteryPower = 0;
            SwitchLight();
        }
    }

    /// <summary>
    /// バッテリーを補充する
    /// </summary>
    /// <param name="setPower"></param>
    public void SetBattery(float addPower)
    {
        _batteryPower += addPower;
        if (_batteryPower > _BATTERY_MAX)
            _batteryPower = _BATTERY_MAX;
    }

    public void OnSwitchLight(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (_batteryPower <= 0) return;

        SwitchLight();
    }

    /// <summary>
    /// ライトのオンオフを切り替える
    /// </summary>
    public void SwitchLight()
    {
        if (_nowSwitch)
        {
            SetLightParam(5.0f, 0.5f);
            _nowSwitch = false;
        }
        else
        {
            SetLightParam(10.0f, 1.0f);
            _nowSwitch = true;
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

    /// <summary>
    /// バッテリー残量を取得する
    /// </summary>
    /// <returns></returns>
    public int GetBatteryPower()
    {
        return (int)(_batteryPower / _BATTERY_MAX * 100) + 1;
    }
}
