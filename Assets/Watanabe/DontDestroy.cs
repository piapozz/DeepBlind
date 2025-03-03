/*
 * @file DontDestroy.cs
 * @brief シーンをまたいで使いたいオブジェクトを設定する
 * @author sein
 * @date 2025/1/20
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class DontDestroy : MonoBehaviour
{
    public static DontDestroy[] instance = new DontDestroy[1];
    public int ObjectNum;

    void Awake()
    {
        CheckInstance();
    }

    // dontdestroyを複数使えるようにしてオブジェクト番号がかぶってたら消す
    void CheckInstance()
    {
        if (instance[ObjectNum] == null)
        {
            instance[ObjectNum] = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

