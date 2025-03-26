using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class FadeSceneChange : MonoBehaviour
{
    public static FadeSceneChange instance = null;
    [SerializeField] private bool cursorLock = true;

    private readonly float DEFAULT_DELAY_SECOND = 2.0f;
    private void Start()
    {
        instance = this;
        SetCursorLock(cursorLock);
    }

    public void SetCursorLock(bool isLock)
    {
        if (isLock)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void ChangeSceneEvent(string sceneName)
    {
        _ = ChangeScene(sceneName);
    }

    public void EndGameEvent()
    {
        _ = EndGame();
    }

    private async UniTask ChangeScene(string sceneName, float sec = 2.0f)
    {
    
        await FadeScreen.instance.FadeOut(0.7f);
        await Task.Delay(TimeSpan.FromSeconds(sec));
        SceneManager.LoadScene(sceneName);
        await FadeScreen.instance.FadeIn(2.0f);
    }

    private async UniTask EndGame(float sec = 2.0f)
    {
        await FadeScreen.instance.FadeOut();
        await Task.Delay(TimeSpan.FromSeconds(sec));
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else
    Application.Quit();//ゲームプレイ終了
#endif
    }

    public void NoneFadeChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
