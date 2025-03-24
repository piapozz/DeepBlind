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
    [SerializeField] private Canvas canvas;
    [SerializeField] private Image fade;            // 黒画面

    [SerializeField] public float alphaValue = 0.0f;               // Fadeイメージのアルファ値を管理

    private readonly float ALPHA_VALUE_MAX = 1.0f;     // アルファ値の最大値
    private readonly float ALPHA_VALUE_MIN = 0.0f;     // アルファ値の最小値
    private readonly float DEFAULT_FADE_SPEED = 5.0f;


    public static FadeScreen instance { get; private set; } = null;

    private void Start()
    {
        instance = this;
        // canvas = UIManager.instance.canvas.GetComponent<Canvas>();
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
    public async UniTask FadeIn(float fadeInSpeed = 5.0f)
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeInSpeed)
        {
            float delta = Time.deltaTime / fadeInSpeed;
            alphaValue = Mathf.Clamp01(alphaValue - delta);
            fade.color = new Color(0.0f, 0.0f, 0.0f, alphaValue);

            elapsedTime += Time.deltaTime;
            await UniTask.DelayFrame(1);
        }

        // 最後にしっかり ALPHA_VALUE_MIN に設定
        alphaValue = ALPHA_VALUE_MIN;
        fade.color = new Color(0.0f, 0.0f, 0.0f, alphaValue);
        canvas.sortingOrder = 1;
    }

    /// <summary>
    /// 黒画面にする処理
    /// </summary>
    /// <returns></returns>
    public async UniTask FadeOut(float fadeOutSpeed = 2.5f)
    {
        float elapsedTime = 0f;
        canvas.sortingOrder = 3;
        while (elapsedTime < fadeOutSpeed)
        {
            float delta = Time.deltaTime / fadeOutSpeed;
            alphaValue = Mathf.Clamp01(alphaValue + delta);
            fade.color = new Color(0.0f, 0.0f, 0.0f, alphaValue);

            elapsedTime += Time.deltaTime;
            await UniTask.DelayFrame(1);
        }

        // 最後にしっかり ALPHA_VALUE_MAX に設定
        alphaValue = ALPHA_VALUE_MAX;
        fade.color = new Color(0.0f, 0.0f, 0.0f, alphaValue);
    }

    public async UniTask FadeInterval(float sec)
    {
        await FadeScreen.instance.FadeOut(DEFAULT_FADE_SPEED);
        await Task.Delay(TimeSpan.FromSeconds(sec));
        await FadeScreen.instance.FadeIn(DEFAULT_FADE_SPEED);
    }
}
