using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Entity_StageData;

public class StageMasterUtility : MonoBehaviour
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
