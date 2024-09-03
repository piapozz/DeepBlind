using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateLocker : MonoBehaviour
{
    [SerializeField] GameObject locker;

    GameObject[] lockerPlace;

    float lockerProbability = 0.3f;       // ロッカー生成確率

    void Start()
    {
        lockerPlace = GameObject.FindGameObjectsWithTag("Locker");

        for (int i = 0; i < lockerPlace.Length; i++)
        {
            if (Random.value >= lockerProbability) continue;

            // アイテム生成
            Instantiate(locker, 
                lockerPlace[i].transform.position, 
                Quaternion.Euler(lockerPlace[i].transform.eulerAngles), 
                lockerPlace[i].transform);
        }
    }
}
