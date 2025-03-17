/*
 * @file FadeScreen.cs
 * @brief フェードイン・フェードアウトを実装
 * @author sein
 * @date 2025/1/17
 */

using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System;

public class FadeScreen : MonoBehaviour
{
    [SerializeField] private Image fade;            // 黒画面

    [SerializeField] private float fadeInSpeed = 0.05f;       // フェードインしていくスピード       
    [SerializeField] private float fadeOutSpeed = 0.05f;      // フェードアウトしていくスピード

    [SerializeField] public float alphaValue = 0.0f;               // Fadeイメージのアルファ値を管理

    public bool fadeIn = false;             // 現在フェードインしているかを管理
    public bool fadeOut = false;            // 現在フェードアウトしているかを管理

    const float ALPHA_VALUE_MAX = 1.0f;     // アルファ値の最大値
    const float ALPHA_VALUE_MIN = 0.0f;     // アルファ値の最小値

    public static FadeScreen instance { get; private set; } = null;

    private void Start()
    {
        instance = this;
    }

    private void Update()
    {
        if (fadeOut) _ = FadeOut();
        if (fadeIn) _ = FadeIn();
    }

    /// <summary>
    /// フェード用のCanvasを生成する
    /// </summary>
    public void GenerateFadeScreen()
    {

    }

    /// <summary>
    /// 黒画面を解除する処理
    /// </summary>
    /// <returns></returns>
    private async UniTask FadeIn()
    {
        while (alphaValue >= ALPHA_VALUE_MIN)
        {
            alphaValue -= fadeInSpeed * Time.deltaTime;

            await UniTask.DelayFrame(1);

            fade.color = new Color(0.0f, 0.0f, 0.0f, alphaValue);
        }

        fadeIn = false;
    }

    /// <summary>
    /// 黒画面にする処理
    /// </summary>
    /// <returns></returns>
    private async UniTask FadeOut()
    {
        while (alphaValue <= ALPHA_VALUE_MAX)
        {
            alphaValue += fadeOutSpeed * Time.deltaTime;

            await UniTask.DelayFrame(1);

            fade.color = new Color(0.0f, 0.0f, 0.0f, alphaValue);
        }

        fadeOut = false;
    }

    public void FadeInRun() { fadeIn = true; }
    public void FadeOutRun() { fadeOut = true; }

    public async UniTask FadeInterval(float sec)
    {
        FadeScreen.instance.FadeOutRun();
        await Task.Delay(TimeSpan.FromSeconds(sec));
        FadeScreen.instance.FadeInRun();
    }
}
