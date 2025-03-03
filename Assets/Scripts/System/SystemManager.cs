using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    [SerializeField] private List<SystemObject> originSystemList = null;
    private List<SystemObject> systemList = null;

    private const int _FRAME_RATE = 60;

    private void Awake()
    {
        // マウスカーソルを固定・非表示
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // FPSを固定
        Application.targetFrameRate = 60;

        // マネージャーを初期化
        Initialize();
    }

    private void Initialize()
    {
        Application.targetFrameRate = _FRAME_RATE;
        systemList = new List<SystemObject>();

        for (int i = 0, max = originSystemList.Count; i < max; i++)
        {
            SystemObject origin = originSystemList[i];
            if (origin == null) continue;

            SystemObject createObj = Instantiate(origin, transform);
            createObj.Initialize();

            systemList.Add(createObj);
        }
    }

    private void Update()
    {
        foreach (SystemObject _systemList in systemList)
        {
            _systemList.Proc();
        }
    }
}
