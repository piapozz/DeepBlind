/*
* @file Section.cs
* @brief 区画データ
* @author sakakura
* @date 2025/3/14
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SectionData
{
    public int ID { get; private set; } = -1;
    public Vector2Int position { get; private set; } = -Vector2Int.one;
    public Direction direction { get; private set; } = Direction.Invalid;
    public RoomType roomType { get; private set; } = RoomType.Invalid;
    public CorridorType corridorType { get; private set; } = CorridorType.Invalid;
    public bool[] isConnect = new bool[(int)Direction.Max];
    public Direction preConnect = Direction.Invalid;
    private const float _SEPARATE_VIRTICAL_OFFSET = StageManager.SECTION_HEIGHT / 2;
    private const float _SEPARATE_HORIZONTAL_OFFSET = StageManager.SECTION_SIZE / 2;

    private static System.Func<int, SectionObject> _GetObject = null;
    public static void SetGetObjectCallback(System.Func<int, SectionObject> setCallback)
    {
        _GetObject = setCallback;
    }

    public void Initialize(int setID, Vector2Int setPos)
    {
        ID = setID;
        SetPosition(setPos);
        for (int i = 0, max = isConnect.Length; i < max; i++)
        {
            isConnect[i] = false;
        }
    }

    /// <summary>
    /// 部屋か否か
    /// </summary>
    /// <returns></returns>
    public bool IsRoom()
    {
        return roomType != RoomType.Invalid;
    }

    public bool IsCorridor()
    {
        return corridorType != CorridorType.Invalid;
    }

    public void SetRoomType(RoomType setType)
    {
        roomType = setType;
    }

    public void SetCorridorType(CorridorType setType)
    {
        corridorType = setType;
    }

    public void SetIsConnect(Direction direction, bool connect)
    {
        isConnect[(int)direction] = connect;
    }

    public void SetPreConnect(Direction setDirection)
    {
        preConnect = setDirection;
    }

    public void SetDirection(Direction setDirection)
    {
        direction = setDirection;
    }

    private void SetPosition(Vector2Int setPosition)
    {
        position = setPosition;
    }

    /// <summary>
    /// 接続部を生成する
    /// </summary>
    public void GenerateSeparate(Transform generateRoot)
    {
        var sectionObjectAssign = StageManager.instance.sectionObjectAssign;
        for (int i = 0, max = isConnect.Length; i < max; i++)
        {
            Direction direction = (Direction)i;
            GameObject separateObject = null;
            // 隣に部屋があるか判定
            SectionData nextRoom = StageManager.instance.GetSectionDir(this, direction);
            // エリア外なら壁
            if (nextRoom == null)
            {
                // つながっていなくて廊下ならスキップ
                if (!isConnect[i] && IsCorridor()) continue;

                separateObject = sectionObjectAssign.wallObject;
            }
            else
            {
                // 優先順位に応じてスキップ
                if (nextRoom.IsRoom() &&
                    (direction == Direction.Down || direction == Direction.Left))
                    continue;

                // つながっていなくて廊下ならスキップ
                if (!isConnect[i] && IsCorridor()) continue;

                if (!IsCorridor() || (direction == Direction.Down || direction == Direction.Left))
                {
                    // フレームを生成
                    separateObject = sectionObjectAssign.frameObject;
                    GenerateSeparateObject(separateObject, (Direction)i, generateRoot);
                }

                if (IsCorridor()) continue;

                // つながっているかでドアか壁
                if (isConnect[i]) separateObject = sectionObjectAssign.doorObject;
                else separateObject = sectionObjectAssign.wallObject;
            }
            // オブジェクトの生成
            GenerateSeparateObject(separateObject, (Direction)i, generateRoot);
        }
    }

    /// <summary>
    /// 接続部のオブジェクトを生成
    /// </summary>
    /// <param name="separateObject"></param>
    /// <param name="direction"></param>
    private void GenerateSeparateObject(GameObject separateObject, Direction direction, Transform generateRoot)
    {
        // 座標設定
        Vector3 generatePosition = StageManager.instance.GetSectionWorldPosition(position);
        Vector2Int dirVec = direction.Vector2();
        float x = dirVec.x * _SEPARATE_HORIZONTAL_OFFSET;
        float y = _SEPARATE_VIRTICAL_OFFSET;
        float z = dirVec.y * _SEPARATE_HORIZONTAL_OFFSET;
        Vector3 offset = new Vector3(x, y, z);
        generatePosition += offset;

        // 回転設定
        Quaternion generateRotation = Quaternion.Euler(0, 90 * (int)direction, 0);
        // 生成
        GameObject.Instantiate(separateObject, generatePosition, generateRotation, generateRoot);
    }

    /// <summary>
    /// 接続状況で廊下の種類を決める
    /// </summary>
    public void SetCorridorType()
    {
        int conectCount = GetConnectCount();
        if (conectCount == 4) corridorType = CorridorType.X;
        else if (conectCount == 3) corridorType = CorridorType.T;
        else if (conectCount == 2)
        {
            if (isConnect[0] == isConnect[2]) corridorType = CorridorType.I;
            else corridorType = CorridorType.L;
        }
    }

    /// <summary>
    /// 回転方向を取得
    /// </summary>
    /// <returns></returns>
    public Direction GetRotate()
    {
        int conectCount = GetConnectCount();
        if (conectCount == 4) return Direction.Up;
        else if (conectCount == 3)
        {
            for (int i = 0, max = isConnect.Length; i < max; i++)
            {
                if (!isConnect[i]) return (Direction)i;
            }
        }
        else if (conectCount == 2)
        {
            // I字かL字か判定
            if (isConnect[0] == isConnect[2])
            {
                if (isConnect[0]) return Direction.Up;
                else return Direction.Right;
            }
            else
            {
                for (int i = 0, max = isConnect.Length - 1; i < max; i++)
                {
                    if (isConnect[i] == isConnect[i + 1] && isConnect[i])
                        return (Direction)i;
                }
                return Direction.Left;
            }
        }
        return Direction.Invalid;
    }

    /// <summary>
    /// 接続数を取得
    /// </summary>
    /// <returns></returns>
    private int GetConnectCount()
    {
        int conectCount = 0;
        for (int i = 0, max = isConnect.Length; i < max; i++)
        {
            if (isConnect[i]) conectCount++;
        }

        return conectCount;
    }
}
