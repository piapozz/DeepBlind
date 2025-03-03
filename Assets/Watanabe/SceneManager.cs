/*
 * @file SceneManager.cs
 * @brief シーンをまたいで使いたいオブジェクトを設定する
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
    /// シーンを変更
    /// </summary>
    /// <param name="value"></param>
    public static void SceneChange(SceneName value)
    {
        // シーンを変更
        SceneManager.LoadScene(sceneList[(int)value]);
    }

    /// <summary>
    /// シーンを変更
    /// </summary>
    /// <param name="value"></param>
    public static void SceneChange(string sceneName)
    {
        // シーンを変更
        SceneManager.LoadScene(sceneName);
    }
}
