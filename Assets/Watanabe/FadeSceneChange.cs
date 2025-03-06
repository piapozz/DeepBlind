using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class FadeSceneChange : MonoBehaviour
{
    public static FadeSceneChange instance = null;

    private void Start()
    {
        instance = this;
    }

    public void ChangeSceneEvent(string sceneName)
    {
        _ = ChangeScene(sceneName);
    }

    public void EndGameEvent()
    {
        _ = EndGame();
    }

    public async UniTask EndGame()
    {
        FadeScreen.instance.FadeOutRun();

        await Task.Delay(TimeSpan.FromSeconds(2));

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }

    public async UniTask ChangeScene(string sceneName)
    {
        FadeScreen.instance.FadeOutRun();

        // 3秒間待つ
        await Task.Delay(TimeSpan.FromSeconds(2));

        SceneManager.LoadScene(sceneName);

        FadeScreen.instance.FadeInRun();
    }
}
