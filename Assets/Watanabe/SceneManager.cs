/*
 * @file SceneManager.cs
 * @brief �V�[�����܂����Ŏg�������I�u�W�F�N�g��ݒ肷��
 * @author sein
 * @date 2025/1/21
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger
{
    public static string[] sceneList = new string[(int)SceneName.MAX];

    public enum SceneName
    {
        INVALID = -1,
        SCENE_TITLE,
        SCENE_GAME,
        SCENE_RESULT,

        MAX
    }

    public SceneChanger()
    {
        sceneList[(int)SceneName.SCENE_TITLE] = "Title";
        sceneList[(int)SceneName.SCENE_GAME] = "Main";
        sceneList[(int)SceneName.SCENE_RESULT] = "Result";
    }

    /// <summary>
    /// �V�[����ύX
    /// </summary>
    /// <param name="value"></param>
    public static void SceneChange(SceneName value)
    {
        // �V�[����ύX
        SceneManager.LoadScene(sceneList[(int)value]);
    }

    /// <summary>
    /// �V�[����ύX
    /// </summary>
    /// <param name="value"></param>
    public static void SceneChange(string sceneName)
    {
        // �V�[����ύX
        SceneManager.LoadScene(sceneName);
    }
}
