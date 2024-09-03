using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateItem : MonoBehaviour
{
    [SerializeField] GameObject key;            // 鍵
    [SerializeField] GameObject map;            // マップ
    [SerializeField] GameObject compass;        // コンパス
    [SerializeField] GameObject[] itemObj;      // ランダム生成するアイテム

    GameObject[] itemPlace;
    List<Transform> itemPlaceList = new List<Transform>();

    float itemProbability = 0.5f;       // アイテム生成確率

    void Start()
    {
        Init();

        // 鍵を生成
        GenerateKey();
        // 地図を生成
        GenerateMap();
        // コンパスを生成
        GenerateCompass();

        // 他のアイテムを生成する
        GenerateRandomItem();
    }

    // 初期化
    void Init()
    {
        itemPlace = GameObject.FindGameObjectsWithTag("Item");

        // 可変長配列に入れる
        for (int i = 0; i < itemPlace.Length; i++)
        {
            itemPlaceList.Add(itemPlace[i].transform);
        }
    }

    // 鍵を生成する関数
    void GenerateKey()
    {
        int rand = Random.Range(0, itemPlaceList.Count);

        Instantiate(key, itemPlaceList[rand].position, Quaternion.Euler(itemPlaceList[rand].eulerAngles), itemPlaceList[rand].transform);

        itemPlaceList.RemoveAt(rand);
    }

    // 地図を生成する関数
    void GenerateMap()
    {
        int rand = Random.Range(0, itemPlaceList.Count);

        Instantiate(map, itemPlaceList[rand].position, Quaternion.Euler(itemPlaceList[rand].eulerAngles), itemPlaceList[rand].transform);

        itemPlaceList.RemoveAt(rand);
    }

    // コンパスを生成する関数
    void GenerateCompass()
    {
        int rand = Random.Range(0, itemPlaceList.Count);

        Instantiate(compass, itemPlaceList[rand].position, Quaternion.Euler(itemPlaceList[rand].eulerAngles), itemPlaceList[rand].transform);

        itemPlaceList.RemoveAt(rand);
    }

    // ランダムなアイテムを生成する関数
    void GenerateRandomItem()
    {
        // 生成場所をロール
        for (int i = 0; i < itemPlaceList.Count; i++)
        {
            // アイテムを生成するかをランダムに決める
            if (Random.value >= itemProbability) continue;

            // 生成するオブジェクトをランダムに決める
            int rand = Random.Range(0, itemObj.Length);

            // アイテム生成
            Instantiate(itemObj[rand], itemPlaceList[i].position, Quaternion.Euler(itemPlaceList[i].eulerAngles), itemPlaceList[i]);
        }
    }
}
