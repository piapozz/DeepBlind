/*
* @file StageMasterUtility.cs
* @brief �X�e�[�W�̃}�X�^�[�f�[�^���[�e�B���e�B
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
    /// �X�e�[�W�}�X�^�[�f�[�^�擾
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static Param GetStageMaster()
    {
        return MasterDataManager.stageData[0][0];
    }
}
