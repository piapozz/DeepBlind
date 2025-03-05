using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/BGMClip")]
public class BGMClip : ScriptableObject
{
    public AudioClip[] bgmClips;
}
