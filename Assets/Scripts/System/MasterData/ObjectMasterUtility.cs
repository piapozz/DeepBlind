using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static Entity_ObjectData;

public class ObjectMasterUtility
{
    /// <summary>
    /// ステージマスターデータ取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static Param GetObjectMaster(int ID)
    {
        return MasterDataManager.objectData[0][ID];
    }
}
