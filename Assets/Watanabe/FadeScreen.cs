/*
 * @file FadeScreen.cs
 * @brief フェードイン・フェードアウトを実装
 * @author sein
 * @date 2025/1/17
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;

public class FadeScreen : MonoBehaviour
{
    [SerializeField] private Image fade;            // 黒画面

    [SerializeField] private float fadeInSpeed = 0.05f;       // フェードインしていくスピード       
    [SerializeField] private float fadeOutSpeed = 0.05f;      // フェードアウトしていくスピード

    [SerializeField] public bool fadeIn = false;             // 現在フェードインしているかを管理
    [SerializeField] public bool fadeOut = false;            // 現在フェードアウトしているかを管理

    private float alphaValue = 0.0f;               // Fadeイメージのアルファ値を管理

    const float ALPHA_VALUE_MAX = 1.0f;     // アルファ値の最大値
    const float ALPHA_VALUE_MIN = 0.0f;     // アルファ値の最小値


    private void Update()
    {
        if (fadeOut) _ = FadeOut();

        if (fadeIn) _ = FadeIn();
    }

    /// <summary>
    /// 黒画面を解除する処理
    /// </summary>
    /// <returns></returns>
    public async UniTask FadeIn()
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
    public async UniTask FadeOut()
    {
        while (alphaValue <= ALPHA_VALUE_MAX)
        {
            alphaValue += fadeOutSpeed * Time.deltaTime;

            await UniTask.DelayFrame(1);

            fade.color = new Color(0.0f, 0.0f, 0.0f, alphaValue);
        }

        fadeOut = false;
    }
}
