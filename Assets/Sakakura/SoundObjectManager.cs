/*
* @file ObjectSoundManager.cs
* @brief �������Ǘ�
* @author sakakura
* @date 2025/2/8
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

// ����炷�Ɖ��ʂ����炵�Ȃ�������Ă���
// �G�̎��̗ǂ���
public class SoundObjectManager : MonoBehaviour, SoundObjectObserver
{
    /// <summary>
    /// �����̃f�[�^
    /// </summary>
    private class ObjectSoundData
    {
        public int ID = -1;
        public Vector3 position = new Vector3();
        public float volume = -1;
        public bool isRing = false;

        public void SetParam(int setID, Vector3 setPosition, float setVolume)
        {
            ID = setID;
            position = setPosition;
            volume = setVolume;
            isRing = true;
        }
    }

    /// <summary>
    /// ���̓͂��ő勗��
    /// </summary>
    private const float _ECHO_RADIUS = 100.0f;

    private const int _SOUND_MAX = 50;

    public static SoundObjectManager instance = null;

    private List<ObjectSoundData> _soundList = null;

    public void Initialize()
    {
        instance = this;
        _soundList = new List<ObjectSoundData>(_SOUND_MAX);
        for (int i = 0; i < _SOUND_MAX; i++)
        {
            _soundList.Add(new ObjectSoundData());
        }
    }

    public void UpdatePosition(int ID, Vector3 position)
    {
        _soundList[ID].position = position;
    }

    public void SetRing(int ID, bool ring)
    {
        _soundList[ID].isRing = ring;
    }

    /// <summary>
    /// �p�����[�^�[���w�肵�ĉ������X�g�ɉ�����
    /// </summary>
    /// <param name="setPosition"></param>
    /// <param name="setRadius"></param>
    /// <param name="setTime"></param>
    public int SetSound(Vector3 setPosition, float setRadius)
    {
        ObjectSoundData setSound = new ObjectSoundData();
        int setID = SearchSoundID();
        if (setID >= 0)
        {
            setSound.SetParam(setID, setPosition, setRadius);
            _soundList[setID] = setSound;
        }
        else
        {
            setID = _soundList.Count;
            setSound.SetParam(setID, setPosition, setRadius);
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
    /// �w�肵�����W�Ɉ�ԋ߂�������Ԃ�
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

            //if (distance > _soundList[i].effectRadius) continue;

            if (distance < minDistance)
            {
                nearSound = _soundList[i];
                minDistance = distance;
            }
        }

        return nearSound.position;
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
