/*
* @file ObjectSoundManager.cs
* @brief �������Ǘ�
* @author sakakura
* @date 2025/2/8
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ����炷�Ɖ��ʂ����炵�Ȃ�������Ă���
public class SoundObjectManager : MonoBehaviour
{
    public static SoundObjectManager instance = null;

    private List<SoundObject> _soundList = null;

    /// <summary>���̓͂��W������</summary>
    private const float _DEFAULT_ECHO_RADIUS = 100.0f;
    /// <summary>�����̍ő吔</summary>
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
    /// �p�����[�^�[���w�肵�ĉ������X�g�ɉ�����
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
    /// �g�p�\��ID��T��
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
    /// ��ԑ傫�������̈ʒu���擾
    /// </summary>
    /// <param name="position"></param>
    /// <param name="sensMinVolume">������(0�`1)</param>
    /// <returns></returns>
    public Vector3 GetBigSoundPosition(Vector3 position, float sensMinVolume)
    {
        float maxVolume = float.MinValue;
        Vector3 resultPos = Vector3.zero;
        for (int i = 0, max = _soundList.Count; i < max; i++)
        {
            // �I�u�W�F�N�g�̋���
            float objectDistance = Vector3.Distance(position, _soundList[i].position);
            // �w��̍��W�ł̉���
            float listenVolume = _soundList[i].volume * (1 - objectDistance / _DEFAULT_ECHO_RADIUS);
            // �傫���������m���ʂȂ�
            if (listenVolume <= maxVolume || listenVolume < sensMinVolume) continue;
            maxVolume = listenVolume;
            resultPos = _soundList[i].position;
        }
        return resultPos;
    }

    /// <summary>
    /// �w�肵��ID�̉���������
    /// </summary>
    /// <param name="ID"></param>
    public void RemoveSound(int ID)
    {
        _soundList[ID] = null;
    }
}
