using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : SystemObject
{
    AudioMixer audioMixer;

    [SerializeField]
    private GameObject _bgmSourceOrigin;

    [SerializeField]
    private GameObject _seSourceOrigin;

    [SerializeField]
    private BGMClip _bgmClip;

    [SerializeField]
    private SEClip _seClip;

    public static AudioManager instance = null;

    private List<AudioSource> BGMSourceList;
    private List<AudioSource> SESourceList;

    public override void Initialize()
    {
        instance = this;

        BGMSourceList = new List<AudioSource>();
        SESourceList = new List<AudioSource>();
    }

    /// <summary>
    /// BGMを流す関数
    /// </summary>
    /// <param name="num"></param>
    public void PlayBGM(BGM num)
    {
        AudioSource source = GetUnusedBGMSource(BGMSourceList);

        source.clip = _bgmClip.bgmClips[(int)num];

        source.Play();
    }

    /// <summary>
    /// BGMを止める関数
    /// </summary>
    /// <param name="num"></param>
    public void StopBGM()
    {
        AudioSource source = BGMSourceList[0];

        source.Stop();
    }

    /// <summary>
    /// SEを鳴らす関数
    /// </summary>
    /// <param name="num"></param>
    public void PlaySE(SE num)
    {
        AudioSource source = GetUnusedSESource(SESourceList);

        source.PlayOneShot(_seClip.seClips[(int)num]);
    }

    // 既存のソースを利用して再生する
    public void PlaySE(SE num, AudioSource source)
    {
        source.PlayOneShot(_seClip.seClips[(int)num]);
    }
    
    /// <summary>
    /// 未使用のソースを取得する
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private AudioSource GetUnusedBGMSource(List<AudioSource> source)
    {
        int number = -1;

        for (int i = 0, max = source.Count; i < max; i++)
        {
            if (source[i].time > 0)
            {
                continue;
            }

            number = i;
            break;
        }

        if (number == -1)
        {
            source.Add(Instantiate(_bgmSourceOrigin, transform).GetComponent<AudioSource>());
            number = source.Count - 1;
        }

        return source[number];
    }

    /// <summary>
    /// 未使用のソースを取得する
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    private AudioSource GetUnusedSESource(List<AudioSource> source)
    {
        int number = -1;

        for (int i = 0, max = source.Count; i < max; i++)
        {
            if (source[i].time > 0)
            {
                continue;
            }

            number = i;
            break;
        }

        if (number == -1)
        {
            source.Add(Instantiate(_seSourceOrigin, transform).GetComponent<AudioSource>());
            number = source.Count - 1;
        }

        return source[number];
    }
}
