/*
* @file ObjectSoundManager.cs
* @brief 環境音を管理
* @author sakakura
* @date 2025/2/8
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 音を鳴らすと音量を減らしながら消えていく
public class SoundObjectManager : MonoBehaviour
{
    public static SoundObjectManager instance = null;

    private List<SoundObject> _soundList = null;

    /// <summary>音の届く標準距離</summary>
    private const float _DEFAULT_ECHO_RADIUS = 100.0f;
    /// <summary>音源の最大数</summary>
    private const int _SOUND_MAX = 100;

    public void Awake()
    {
        instance = this;
        _soundList = new List<SoundObject>(_SOUND_MAX);
        for (int i = 0; i < _SOUND_MAX; i++)
        {
            _soundList.Add(new SoundObject());
        }
    }

    /// <summary>
    /// パラメーターを指定して音源リストに加える
    /// </summary>
    /// <param name="setPosition"></param>
    /// <param name="volume"></param>
    /// <returns></returns>
    public int SetSound(Vector3 setPosition, float volume)
    {
        SoundObject setSound = new SoundObject();
        int setID = SearchSoundID();
        if (setID >= 0)
        {
            setSound.SetParam(setID, setPosition, volume);
            _soundList[setID] = setSound;
        }
        else
        {
            setID = _soundList.Count;
            setSound.SetParam(setID, setPosition, volume);
            _soundList.Add(setSound);
        }
        return setID;
    }

    /// <summary>
    /// 使用可能なIDを探す
    /// </summary>
    /// <returns></returns>
    private int SearchSoundID()
    {
        for (int i = 0; i < _soundList.Count; i++)
        {
            if (_soundList[i] == null)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// 一番大きい音源の位置を取得
    /// </summary>
    /// <param name="position"></param>
    /// <param name="sensMinVolume">可聴音量(0～1)</param>
    /// <returns></returns>
    public Vector3 GetBigSoundPosition(Vector3 position, float sensMinVolume)
    {
        float maxVolume = float.MinValue;
        Vector3 resultPos = Vector3.zero;
        for (int i = 0, max = _soundList.Count; i < max; i++)
        {
            // オブジェクトの距離
            float objectDistance = Vector3.Distance(position, _soundList[i].position);
            // 指定の座標での音量
            float listenVolume = _soundList[i].volume * (1 - objectDistance / _DEFAULT_ECHO_RADIUS);
            // 大きい音かつ感知音量なら
            if (listenVolume <= maxVolume || listenVolume < sensMinVolume) continue;
            maxVolume = listenVolume;
            resultPos = _soundList[i].position;
        }
        return resultPos;
    }

    /// <summary>
    /// 指定したIDの音源を消す
    /// </summary>
    /// <param name="ID"></param>
    public void RemoveSound(int ID)
    {
        _soundList[ID] = null;
    }
}
