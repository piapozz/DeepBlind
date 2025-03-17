/*
 * @file FadeScreen.cs
 * @brief �t�F�[�h�C���E�t�F�[�h�A�E�g������
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
    [SerializeField] private Image fade;            // �����

    [SerializeField] private float fadeInSpeed = 0.05f;       // �t�F�[�h�C�����Ă����X�s�[�h       
    [SerializeField] private float fadeOutSpeed = 0.05f;      // �t�F�[�h�A�E�g���Ă����X�s�[�h

    [SerializeField] public float alphaValue = 0.0f;               // Fade�C���[�W�̃A���t�@�l���Ǘ�

    public bool fadeIn = false;             // ���݃t�F�[�h�C�����Ă��邩���Ǘ�
    public bool fadeOut = false;            // ���݃t�F�[�h�A�E�g���Ă��邩���Ǘ�

    const float ALPHA_VALUE_MAX = 1.0f;     // �A���t�@�l�̍ő�l
    const float ALPHA_VALUE_MIN = 0.0f;     // �A���t�@�l�̍ŏ��l

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
    /// �t�F�[�h�p��Canvas�𐶐�����
    /// </summary>
    public void GenerateFadeScreen()
    {

    }

    /// <summary>
    /// ����ʂ��������鏈��
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
    /// ����ʂɂ��鏈��
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
