using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    const int FRAME_RATE = 60;

    void Awake()
    {
        Application.targetFrameRate = FRAME_RATE;
    }
}
