using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoTime : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    float time;
    int hour;
    int min;
    int sec;
    int millsec;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        time = 0;
    }

    void Update()
    {
        time += Time.deltaTime;
        
        hour = Mathf.FloorToInt(time / 3600) % 3600;
        min = Mathf.FloorToInt(time / 60) % 60;
        sec = Mathf.FloorToInt(time) % 60;
        millsec = Mathf.FloorToInt(time * 100) % 100;

        textMesh.SetText("{0:00}:{1:00}:{2:00}:{3:00}", hour, min, sec, millsec);
    }
}
