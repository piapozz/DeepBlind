/*
* @file Section.cs
* @brief ���
* @author sakakura
* @date 2025/3/14
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Section
{
    public int ID { get; private set; } = -1;
    public Vector2Int position { get; private set; } = -Vector2Int.one;
    public RoomType roomType { get; private set; } = RoomType.Invalid;
    public bool[] isConnect = new bool[(int)Direction.Max];

    private const float _SEPARATE_VIRTICAL_OFFSET = StageManager.SECTION_HEIGHT / 2;
    private const float _SEPARATE_HORIZONTAL_OFFSET = StageManager.SECTION_SIZE / 2;

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
    /// �������ۂ�
    /// </summary>
    /// <returns></returns>
    public bool IsRoom()
    {
        return roomType != RoomType.Invalid;
    }

    public void SetRoomType(RoomType setType)
    {
        roomType = setType;
    }

    public void SetIsConnect(Direction direction, bool connect)
    {
        isConnect[(int)direction] = connect;
    }

    private void SetPosition(Vector2Int setPosition)
    {
        position = setPosition;
    }

    /// <summary>
    /// �ڑ����𐶐�����
    /// </summary>
    public void GenerateSeparate(Transform generateRoot)
    {
        if (!IsRoom()) return;

        var sectionObjectAssign = StageManager.instance.sectionObjectAssign;
        for (int i = 0, max = isConnect.Length; i < max; i++)
        {
            Direction direction = (Direction)i;
            GameObject separateObject = null;
            // �ׂɕ��������邩����
            Section nextRoom = StageManager.instance.GetSectionDir(this, direction);
            // �G���A�O�Ȃ��
            if (nextRoom == null)
                separateObject = sectionObjectAssign.wallObject;
            else
            {
                // �ׂ������Ȃ�D�揇�ʂɉ����ăX�L�b�v
                if (nextRoom.IsRoom() &&
                    (direction == Direction.Down || direction == Direction.Left))
                    continue;

                // �Ȃ����Ă��邩�Ńh�A����
                if (isConnect[i]) separateObject = sectionObjectAssign.doorObject;
                else separateObject = sectionObjectAssign.wallObject;
            }

            GenerateSeparateObject(separateObject, (Direction)i, generateRoot);
        }
    }

    /// <summary>
    /// �ڑ����̃I�u�W�F�N�g�𐶐�
    /// </summary>
    /// <param name="separateObject"></param>
    /// <param name="direction"></param>
    private void GenerateSeparateObject(GameObject separateObject, Direction direction, Transform generateRoot)
    {
        // ���W�ݒ�
        Vector3 generatePosition = StageManager.instance.GetSectionWorldPosition(position);
        Vector2Int dirVec = direction.Vector2();
        float x = dirVec.x * _SEPARATE_HORIZONTAL_OFFSET;
        float y = _SEPARATE_VIRTICAL_OFFSET;
        float z = dirVec.y * _SEPARATE_HORIZONTAL_OFFSET;
        Vector3 offset = new Vector3(x, y, z);
        generatePosition += offset;

        // ��]�ݒ�
        Quaternion generateRotation = Quaternion.Euler(0, 90 * (int)direction, 0);

        GameObject.Instantiate(separateObject, generatePosition, generateRotation, generateRoot);
    }

    /// <summary>
    /// �ڑ��󋵂ŘL���̎�ނ����߂�
    /// </summary>
    public CorridorType GetCorridorType()
    {
        int conectCount = GetConnectCount();
        if (conectCount == 4) return CorridorType.X;
        else if (conectCount == 3) return CorridorType.T;
        else if (conectCount == 2)
        {
            if (isConnect[0] == isConnect[2]) return CorridorType.I;
            else return CorridorType.L;
        }
        return CorridorType.Invalid;
    }

    /// <summary>
    /// ��]�������擾
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
            // I����L��������
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
    /// �ڑ������擾
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
