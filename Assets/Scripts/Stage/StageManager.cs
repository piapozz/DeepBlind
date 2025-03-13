using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class StageManager : SystemObject
{
    public static StageManager instance { get; private set; } = null;

    [SerializeField]
    public SectionObjectAssign sectionObjectAssign = null;

    [SerializeField]
    private Transform _stageRoot = null;

    /// <summary>���̃f�[�^���X�g</summary>
    private List<Section> _sectionList = null;

    /// <summary>�����łȂ�����ID���X�g</summary>
    private List<int> _unroomSectionIDList = null;

    private Entity_StageData.Param _stageMaster = null;

    private int _stageSectionCount = -1;

    public const float SECTION_SIZE = 10.0f;

    public const float SECTION_HEIGHT = 3.0f;

    public override void Initialize()
    {
        _stageMaster = StageMasterUtility.GetStageMaster();
        _stageSectionCount = _stageMaster.widthSize * _stageMaster.heightSize;
        // ����������
        _sectionList = new List<Section>(_stageSectionCount);
        _unroomSectionIDList = new List<int>(_stageSectionCount);
        for (int i = 0; i < _stageSectionCount; i++)
        {
            Section createSection = new Section();
            Vector2Int position = GetSectionPosition(i);
            createSection.Initialize(i, position);
            _sectionList.Add(createSection);
            _unroomSectionIDList.Add(i);
        }

        CreateStage();
        GenerateStage();
    }

    /// <summary>
	/// ID��2�������W�ɕϊ�
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public Vector2Int GetSectionPosition(int ID)
    {
        if (ID < 0 || ID > _stageMaster.widthSize * _stageMaster.heightSize) return -Vector2Int.one;

        Vector2Int result = new Vector2Int();
        result.x = ID % _stageMaster.widthSize;
        result.y = ID / _stageMaster.widthSize;
        return result;
    }

    /// <summary>
    /// 2�������W��ID�ɕϊ�
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public int GetSectionID(Vector2Int position)
    {
        if (position.x < 0 || position.x >= _stageMaster.widthSize ||
            position.y < 0 || position.y >= _stageMaster.heightSize) return -1;

        return position.y * _stageMaster.widthSize + position.x;
    }

    /// <summary>
    /// �X�e�[�W�̍쐬
    /// </summary>
    private void CreateStage()
    {
        // �����̈ʒu����Ɛڑ�
        List<Section> roomList = DecideRoom();
        ConnectRoom(roomList);
    }

    /// <summary>
    /// �����̈ʒu�����߂�
    /// </summary>
    /// <returns></returns>
    private List<Section> DecideRoom()
    {
        List<Section> roomList = new List<Section>(_unroomSectionIDList.Count);
        // �X�^�[�g��������
        int startWidth = Random.Range(0, _stageMaster.widthSize + 1);
        Section startRoom = DecideRoomType(new Vector2Int(startWidth, 0), RoomType.StartRoom);
        roomList.Add(startRoom);

        // �e�����̈ʒu������
        DecideNormalRoom(_stageMaster.roomCount, roomList);

        // ����������
        int keyWidth = Random.Range(0, _stageMaster.widthSize + 1);
        Section keyRoom = DecideRoomType(new Vector2Int(keyWidth, 0), RoomType.KeyRoom);
        roomList.Add(keyRoom);

        return roomList;
    }
    
    /// <summary>
    /// ���W�w��̕����̎�ތ���
    /// </summary>
    /// <param name="position"></param>
    /// <param name="type"></param>
    private Section DecideRoomType(Vector2Int position, RoomType type)
    {
        int ID = GetSectionID(position);
        Section decideRoom = GetSection(ID);
        decideRoom.SetRoomType(type);
        _unroomSectionIDList.Remove(ID);
        return decideRoom;
    }

    /// <summary>
    /// ID�w��̕����̎�ތ���
    /// </summary>
    /// <param name="ID"></param>
    /// <param name="type"></param>
    private Section DecideRoomType(int ID, RoomType type)
    {
        Section decideRoom = GetSection(ID);
        decideRoom.SetRoomType(type);
        _unroomSectionIDList.Remove(ID);
        return decideRoom;
    }

    /// <summary>
    /// ���w��̒ʏ핔���̌���
    /// </summary>
    /// <param name="roomCount"></param>
    /// <param name="roomList"></param>
    private void DecideNormalRoom(int roomCount, List<Section> roomList)
    {
        for (int i = 0; i < roomCount; i++)
        {
            if (_unroomSectionIDList.Count <= 1) return;
            int randomIndex = Random.Range(0, _unroomSectionIDList.Count);
            int roomID = _unroomSectionIDList[randomIndex];
            Section normalRoom = DecideRoomType(roomID, RoomType.NormalRoom);
            roomList.Add(normalRoom);
        }
    }

    /// <summary>
    /// �����ƕ������Ȃ�
    /// </summary>
    /// <param name="roomList"></param>
    private void ConnectRoom(List<Section> roomList)
    {
        // �������אڂ��Ă���Ȃ�Ȃ�
        for (int i = 0, max = roomList.Count - 1; i < max; i++)
        {
            ConnectNextRoom(roomList[i]);
        }

        // ���������ԂɂȂ��ł���
        for (int i = 0, max = roomList.Count - 1; i < max; i++)
        {
            // A*�ŕ���1���畔��2���Ȃ�
            // �ŒZ�o�H����������ꍇ�͊����̓����ł����p���Ă��郋�[�g�ɂ���
            // �Ȃ��Ȃ烉���_���łȂ�

        }
    }

    /// <summary>
    /// ���������͂̕����ƂȂ���
    /// </summary>
    /// <param name="sourceSection"></param>
    private void ConnectNextRoom(Section sourceSection)
    {
        if (!sourceSection.IsRoom()) return;

        for (int i = 0, max = (int)Direction.Max; i < max; i++)
        {
            if (sourceSection.isConnect[i]) continue;

            Direction direction = (Direction)i;
            Section nextSection = GetSectionDir(sourceSection, direction);
            if (nextSection == null || !nextSection.IsRoom()) continue;

            // ���������Ȃ���
            sourceSection.SetIsConnect(direction, true);
            nextSection.SetIsConnect(direction.ReverseDir(), true);
        }
    }

    /// <summary>
    /// �X�e�[�W�̐���
    /// </summary>
    private void GenerateStage()
    {
        for (int i = 0, max = _sectionList.Count; i < max; i++)
        {
            Section section = GetSection(i);
            if (section.IsRoom())
            {
                Transform generateRoot = GenerateRoom(i);
                if (generateRoot == null) continue;
                section.GenerateSeparate(generateRoot);
            }
            else GenerateCorridor(i);
        }
    }

    /// <summary>
    /// �����̐���
    /// </summary>
    /// <param name="ID"></param>
    private Transform GenerateRoom(int ID)
    {
        Section room = GetSection(ID);
        GameObject generateObject = null;
        int rotateCount = 0;
        // �����̎�ނɂ���Đ������e�ݒ�
        switch (room.roomType)
        {
            case RoomType.Invalid:
                return null;
            case RoomType.NormalRoom:
                GameObject[] roomObject = sectionObjectAssign.roomObjectList;
                int randomIndex = Random.Range(0, roomObject.Length);
                generateObject = roomObject[randomIndex];
                rotateCount = Random.Range(0, (int)Direction.Max);
                break;
            case RoomType.StartRoom:
                generateObject = sectionObjectAssign.startRoom;
                break;
            case RoomType.KeyRoom:
                generateObject = sectionObjectAssign.keyRoom;
                break;
        }
        if (generateObject == null) return null;

        // ���W�A��]��ݒ肵�Đ���
        return GenerateObject(generateObject, room.position, rotateCount);
    }

    /// <summary>
    /// �L���̐���
    /// </summary>
    /// <param name="ID"></param>
    private void GenerateCorridor(int ID)
    {
        // �L���̎�ތ���
        Section corridor = GetSection(ID);
        CorridorType corridorType = _sectionList[ID].GetCorridorType();
        if (corridorType == CorridorType.Invalid) return;
        GameObject generateObject = sectionObjectAssign.corridorObjectList[(int)corridorType];
        if (generateObject == null) return;

        // ��]����
        int rotateCount = (int)corridor.GetRotate();
        // ���W�A��]��ݒ肵�Đ���
        GenerateObject(generateObject, corridor.position, rotateCount);
    }

    /// <summary>
    /// �I�u�W�F�N�g�𐶐�����
    /// </summary>
    /// <param name="sectionObject"></param>
    /// <param name="sectionPosition"></param>
    /// <param name="rotateCount"></param>
    private Transform GenerateObject(GameObject sectionObject, Vector2Int sectionPosition, int rotateCount)
    {
        Vector3 position = GetSectionWorldPosition(sectionPosition);
        Quaternion rotation = Quaternion.Euler(0, 90 * rotateCount, 0);
        return Instantiate(sectionObject, position, rotation, _stageRoot).transform;
    }

    /// <summary>
    /// 2�������W��3�����̃��[���h���W�ŕԂ�
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector3 GetSectionWorldPosition(Vector2Int position)
    {
        float x = position.x * SECTION_SIZE;
        float z = position.y * SECTION_SIZE;

        return new Vector3(x, 0, z);
    }

    /// <summary>
    /// �V����������ǉ����ăX�e�[�W�Đ���
    /// </summary>
    /// <param name="addRoomCount"></param>
    public void RegenerateSection(int addRoomCount)
    {
        DestroyAllObject();
        List<Section> addRoomList = new List<Section>(_unroomSectionIDList.Count);
        DecideNormalRoom(addRoomCount, addRoomList);
        ConnectRoom(addRoomList);
        GenerateStage();
    }

    /// <summary>
    /// �X�e�[�W�I�u�W�F�N�g�S�폜
    /// </summary>
    private void DestroyAllObject()
    {
        foreach (Transform child in _stageRoot)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// �w������̋����擾
    /// </summary>
    /// <param name="sourceSection"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Section GetSectionDir(Section sourceSection, Direction direction)
    {
        Vector2Int targetPosition = sourceSection.position + direction.Vector2();
        return GetSection(GetSectionID(targetPosition));
    }

    /// <summary>
    /// ID�w��̋��擾
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public Section GetSection(int ID)
    {
        if (!IsEnableIndex(_sectionList, ID)) return null;
        return _sectionList[ID];
    }
}
