/*
 * @file FadeScreen.cs
 * @brief �t�F�[�h�C���E�t�F�[�h�A�E�g������
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
    [SerializeField] private Image fade;            // �����

    [SerializeField] private float fadeInSpeed = 0.05f;       // �t�F�[�h�C�����Ă����X�s�[�h       
    [SerializeField] private float fadeOutSpeed = 0.05f;      // �t�F�[�h�A�E�g���Ă����X�s�[�h

    [SerializeField] public bool fadeIn = false;             // ���݃t�F�[�h�C�����Ă��邩���Ǘ�
    [SerializeField] public bool fadeOut = false;            // ���݃t�F�[�h�A�E�g���Ă��邩���Ǘ�

    private float alphaValue = 0.0f;               // Fade�C���[�W�̃A���t�@�l���Ǘ�

    const float ALPHA_VALUE_MAX = 1.0f;     // �A���t�@�l�̍ő�l
    const float ALPHA_VALUE_MIN = 0.0f;     // �A���t�@�l�̍ŏ��l


    private void Update()
    {
        if (fadeOut) _ = FadeOut();

        if (fadeIn) _ = FadeIn();
    }

    /// <summary>
    /// ����ʂ��������鏈��
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
    /// ����ʂɂ��鏈��
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
