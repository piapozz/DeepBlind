using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LightManager : MonoBehaviour
{
    private Light _light;

    private bool _enabled = true;

    void Start()
    {
        _light = GetComponentInChildren<Light>();
    }

    public void OnSwitchLight(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (_enabled)
        {
            SetLiteRange(5);
            _enabled = false;
        }
        else
        {
            SetLiteRange(10);
            _enabled = true;
        }

        Debug.Log(_enabled);
    }

    private void SetLiteRange(float range)
    {
        _light.range = range;
    }
}
