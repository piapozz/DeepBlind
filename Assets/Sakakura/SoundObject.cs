/*
* @file SoundObject.cs
* @brief ����
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
    /// <summary>����(0�`1)</summary>
    public float volume { get; private set; } = -1;

    /// <summary>���ʂ̌������x</summary>
    private float _VOLUME_DECREASE = 0.01f;

    /// <summary>
    /// �p�����[�^�[��ݒ�
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
    /// ���ʂ����炵�Ă���
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
