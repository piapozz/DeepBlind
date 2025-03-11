/*
* @file SoundObject.cs
* @brief 環境音
* @author sakakura
* @date 2025/3/11
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class SoundObject
{
    public int ID { get; private set; } = -1;
    public Vector3 position { get; private set; } = new Vector3();
    /// <summary>音量(0～1)</summary>
    public float volume { get; private set; } = -1;

    /// <summary>音量の減衰速度</summary>
    private float _VOLUME_DECREASE = 0.01f;

    /// <summary>
    /// パラメーターを設定
    /// </summary>
    /// <param name="setID"></param>
    /// <param name="setPosition"></param>
    /// <param name="setVolume"></param>
    public void SetParam(int setID, Vector3 setPosition, float setVolume)
    {
        ID = setID;
        position = setPosition;
        volume = Mathf.Clamp01(setVolume);
        UniTask task = UpdateVolume();
    }

    /// <summary>
    /// 音量を減らしていく
    /// </summary>
    /// <returns></returns>
    private async UniTask UpdateVolume()
    {
        while (volume > 0)
        {
            volume -= _VOLUME_DECREASE;

            await UniTask.DelayFrame(1);
        }
        Teardown();
    }

    public void Teardown()
    {
        ID = -1;
        position = Vector3.zero;
        volume = -1;
        SoundObjectManager.instance.RemoveSound(ID);
    }
}
