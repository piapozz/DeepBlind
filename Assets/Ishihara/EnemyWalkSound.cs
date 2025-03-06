using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EnemyWalkSound : MonoBehaviour
{

    [SerializeField]
    private GameObject _seSourceOrigin;

    [SerializeField]
    private SEClip _seClip;

    private List<AudioSource> SESourceList;

    public void Awake()
    {
        SESourceList = new List<AudioSource>();
    }

    /// <summary>
    /// SEを鳴らす関数
    /// </summary>
    /// <param name="num"></param>
    public void PlaySE()
    {
        AudioSource source = GetUnusedSESource(SESourceList);
        AudioClip clip = _seClip.seClips[Random.Range(0,_seClip.seClips.Length)];

        source.PlayOneShot(clip);
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
