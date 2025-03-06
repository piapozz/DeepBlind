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
        UnityEditor.EditorApplication.isPlaying = false;//�Q�[���v���C�I��
#else
    Application.Quit();//�Q�[���v���C�I��
#endif
    }

    public async UniTask ChangeScene(string sceneName)
    {
        FadeScreen.instance.FadeOutRun();

        // 3�b�ԑ҂�
        await Task.Delay(TimeSpan.FromSeconds(2));

        SceneManager.LoadScene(sceneName);

        FadeScreen.instance.FadeInRun();
    }
}
