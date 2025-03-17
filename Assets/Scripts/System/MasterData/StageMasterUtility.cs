/*
* @file StageMasterUtility.cs
* @brief ステージのマスターデータユーティリティ
* @author sakakura
* @date 2025/3/14
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Entity_StageData;

public class StageMasterUtility
{
    /// <summary>
    /// ステージマスターデータ取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static Param GetStageMaster()
    {
        return MasterDataManager.stageData[0][0];
    }
}
