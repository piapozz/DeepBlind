using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : SystemObject
{
    private enum ObjectType
    {
        INVALID = -1,
        BATTERY_SMALL,
        BATTERY_LARGE,
        MAP,
        COMPASS,
        EXIT_KEY,
        LOCKER,
        MAX
    }

    [SerializeField]
    private ObjectAssign _objectAssign = null;

    public override void Initialize()
    {
        GenerateAllItem();
        GenerateLocker();
        StageManager.instance.NavMeshBake();
    }
    
    /// <summary>
    /// すべてのアイテムを生成
    /// </summary>
    private void GenerateAllItem()
    {
        for (int i = 0, max = (int)ObjectType.EXIT_KEY; i < max; i++)
        {
            GenerateItem((ObjectType)i);
        }
    }

    /// <summary>
    /// タイプ指定のアイテム生成
    /// </summary>
    /// <param name="type"></param>
    private void GenerateItem(ObjectType type)
    {
        int typeIndex = (int)type;
        int generateCount = ObjectMasterUtility.GetObjectMaster(typeIndex).generateCount;
        for (int i = 0; i < generateCount; i++)
        {
            Transform itemAnchor = StageManager.instance.GetRandomItemAnchor();
            if (itemAnchor == null) return;

            GameObject genObj = _objectAssign.itemObjectList[typeIndex];
            Instantiate(genObj, itemAnchor.position, itemAnchor.rotation, itemAnchor);
        }
    }

    /// <summary>
    /// ロッカーの生成
    /// </summary>
    private void GenerateLocker()
    {
        int typeIndex = (int)ObjectType.LOCKER;
        int generateCount = ObjectMasterUtility.GetObjectMaster(typeIndex).generateCount;
        for (int i = 0; i < generateCount; i++)
        {
            Transform lockerAnchor = StageManager.instance.GetRandomLockerAnchor();
            if (lockerAnchor == null) return;

            GameObject genObj = _objectAssign.lockerObject;
            Instantiate(genObj, lockerAnchor.position, lockerAnchor.rotation, lockerAnchor);
        }
    }

    private void GenerateFlavorObject()
    {

    }
}
