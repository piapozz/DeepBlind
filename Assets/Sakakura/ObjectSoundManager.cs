/*
* @file ObjectSoundManager.cs
* @brief 環境音を管理
* @author sakakura
* @date 2025/2/8
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSoundManager : MonoBehaviour
{
    /// <summary>
    /// 環境音のデータ
    /// </summary>
    private class ObjectSoundData
    {
        public Vector3 position = new Vector3();
        public float effectRadius = -1;
        public float durationTime = -1;

        public void SetParam(Vector3 setPosition, float setRadius, float setTime)
        {
            position = setPosition;
            effectRadius = setRadius;
            durationTime = setTime;
        }
    }

    public static ObjectSoundManager instance = null;

    private List<ObjectSoundData> _soundList = null;

    private const int _SOUND_MAX = 50;

    public void Initialize()
    {
        instance = this;
        _soundList = new List<ObjectSoundData>(_SOUND_MAX);
    }

    /// <summary>
    /// パラメーターを指定して音源リストに加える
    /// </summary>
    /// <param name="setPosition"></param>
    /// <param name="setRadius"></param>
    /// <param name="setTime"></param>
    public void SetSound(Vector3 setPosition, float setRadius, float setTime)
    {
        ObjectSoundData setSound = new ObjectSoundData();
        setSound.SetParam(setPosition, setRadius, setTime);
        _soundList.Add(setSound);
        EraseSound(setTime);
    }

    /// <summary>
    /// 指定した座標に一番近い音源を返す
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector3 GetNearSound(Vector3 position)
    {
        ObjectSoundData nearSound = null;
        float minDistance = float.MaxValue;
        for (int i = 0, max = _soundList.Count; i < max; i++)
        {
            float distance = Vector3.Distance(position, _soundList[i].position);

            if (distance > _soundList[i].effectRadius) continue;

            if (distance < minDistance)
            {
                nearSound = _soundList[i];
                minDistance = distance;
            }
        }

        return nearSound.position;
    }

    // UniTaskで実行する削除関数
    public void EraseSound(float setTime)
    {

    }
}
