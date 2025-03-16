/*
* @file StageManager.cs
* @brief �X�e�[�W�̊Ǘ��N���X
* @author sakakura
* @date 2025/3/14
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

using static CommonModule;

public class StageManager : SystemObject
{
    public static StageManager instance { get; private set; } = null;

    [SerializeField]
    public SectionObjectAssign sectionObjectAssign = null;

    [SerializeField]
    private Transform _stageRoot = null;

    private Entity_StageData.Param _stageMaster = null;

    /// <summary>���̃f�[�^���X�g</summary>
    private List<Section> _sectionList = null;

    /// <summary>�����łȂ�����ID���X�g</summary>
    private List<int> _unroomSectionIDList = null;

    private Section startRoom = null;

    private Section keyRoom = null;

    private Direction keyRoomConnectDir = Direction.Invalid;

    public int stageSectionCount { get; private set; } = -1;

    public const float SECTION_SIZE = 10.0f;

    public const float SECTION_HEIGHT = 3.0f;

    private void Start()
    {
        MasterDataManager.LoadAllData();
        Initialize();
    }

    public override void Initialize()
    {
        instance = this;
        _stageMaster = StageMasterUtility.GetStageMaster();
        stageSectionCount = _stageMaster.widthSize * _stageMaster.heightSize;
        // ����������
        _sectionList = new List<Section>(stageSectionCount);
        _unroomSectionIDList = new List<int>(stageSectionCount);
        for (int i = 0; i < stageSectionCount; i++)
        {
            Section createSection = new Section();
            Vector2Int position = GetSectionPosition(i);
            createSection.Initialize(i, position);
            _sectionList.Add(createSection);
            _unroomSectionIDList.Add(i);
        }

        // �X�e�[�W�쐬
        CreateInitialStage(_stageMaster.roomCount);
        // �X�e�[�W����
        GenerateStage();
        
        RegenerateSection(3);
    }

    /// <summary>
	/// ID��2�������W�ɕϊ�
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public Vector2Int GetSectionPosition(int ID)
    {
        if (ID < 0 || ID > stageSectionCount) return -Vector2Int.one;

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
    /// �����X�e�[�W�̍쐬
    /// </summary>
    private void CreateInitialStage(int normalRoomCount)
    {
        // �����̃��X�g�̏�����
        List<Section> roomList = InitializeRoomList(normalRoomCount + 2);
        // �X�^�[�g��������
        DecideStartRoom(roomList);
        // ����������
        DecideKeyRoom(roomList);
        // �e�����̈ʒu������
        DecideNormalRoom(normalRoomCount, roomList);
        // �������Ȃ���
        ConnectRoom(roomList);
    }

    /// <summary>
    /// ���������������̃��X�g���擾
    /// </summary>
    /// <param name="roomCount"></param>
    /// <returns></returns>
    private List<Section> InitializeRoomList(int roomCount)
    {
        // �����̃��X�g�̏�����
        List<Section> roomList = new List<Section>(roomCount);
        for (int i = 0; i < roomCount; i++)
        {
            roomList.Add(null);
        }
        return roomList;
    }

    /// <summary>
    /// �X�^�[�g���������߂�
    /// </summary>
    /// <returns></returns>
    private void DecideStartRoom(List<Section> roomList)
    {
        int startWidth = Random.Range(0, _stageMaster.widthSize);
        Section setStartRoom = DecideRoomType(new Vector2Int(startWidth, 0), RoomType.StartRoom);
        roomList[0] = setStartRoom;
        startRoom = setStartRoom;
    }

    /// <summary>
    /// �����������߂�
    /// </summary>
    /// <returns></returns>
    private void DecideKeyRoom(List<Section> roomList)
    {
        int keyWidth = Random.Range(0, _stageMaster.widthSize);
        Section setKeyRoom = DecideRoomType(new Vector2Int(keyWidth, _stageMaster.heightSize - 1), RoomType.KeyRoom);
        roomList[roomList.Count - 1] = setKeyRoom;
        keyRoom = setKeyRoom;
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
        if (decideRoom == null) return null;

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
            if (_unroomSectionIDList.Count == 0) return;
            int randomIndex = Random.Range(0, _unroomSectionIDList.Count);
            int roomID = _unroomSectionIDList[randomIndex];
            Section normalRoom = DecideRoomType(roomID, RoomType.NormalRoom);
            int unuseIndex = GetUnuseIndex(roomList);
            roomList[unuseIndex] = normalRoom;
        }
    }

    /// <summary>
    /// �����ƕ������Ȃ�
    /// </summary>
    /// <param name="roomList"></param>
    private void ConnectRoom(List<Section> roomList, Direction beforeConnectDir = Direction.Invalid)
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
            List<MoveData> route = RouteSearcher.RouteSearch(roomList[i].ID, roomList[i + 1].ID, beforeConnectDir);
            if (route == null) continue;

            // ���[�g��������Ȃ���
            beforeConnectDir = ConnectRouteSection(route);
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
    /// �w��̃��[�g���̋�擯�m���Ȃ��A�Ō�̕�����Ԃ�
    /// </summary>
    /// <param name="route"></param>
    /// <returns></returns>
    private Direction ConnectRouteSection(List<MoveData> route)
    {
        Direction lastDirection = Direction.Invalid;
        for (int i = 0, max = route.Count; i < max; i++)
        {
            Direction direction = route[i].direction;
            lastDirection = direction.ReverseDir();
            // �ڑ������Ȃ���
            Section sourceSection = GetSection(route[i].sourceSectionID);
            sourceSection.SetIsConnect(direction, true);
            // �ڑ�����Ȃ���
            Section targetSection = GetSection(route[i].targetSectionID);
            targetSection.SetIsConnect(lastDirection, true);
            if (targetSection.roomType == RoomType.KeyRoom)
                keyRoomConnectDir = lastDirection;
        }
        return lastDirection;
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
                rotateCount = (int)GetConnectSection(ID)[0].ReverseDir();
                break;
            case RoomType.KeyRoom:
                generateObject = sectionObjectAssign.keyRoom;
                rotateCount = (int)GetConnectSection(ID)[0].ReverseDir();
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
    public async UniTask RegenerateSection(int addRoomCount)
    {
        await UniTask.DelayFrame(300);
        // �I�u�W�F�N�g�̍폜
        DestroyAllObject();
        // ����������X�^�[�g�����Ɍo�H����
        List<Section> roomList = InitializeRoomList(addRoomCount + 2);
        roomList[0] = keyRoom;
        roomList[roomList.Count - 1] = startRoom;
        DecideNormalRoom(addRoomCount, roomList);
        ConnectRoom(roomList, keyRoomConnectDir);
        // �I�u�W�F�N�g�̐���������
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
    /// ID�w��̎w������̋����擾
    /// </summary>
    /// <param name="sourceSectionID"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Section GetSectionDir(int sourceSectionID, Direction direction)
    {
        Section sourceSection = GetSection(sourceSectionID);
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

    /// <summary>
    /// �Ȃ����Ă���������擾
    /// </summary>
    /// <param name="sectionID"></param>
    /// <returns></returns>
    public List<Direction> GetConnectSection(int sectionID)
    {
        List<Direction> connectDirection = new List<Direction>((int)Direction.Max);
        Section section = GetSection(sectionID);
        for (int i = 0, max = (int)Direction.Max; i < max; i++)
        {
            if (section.isConnect[i]) connectDirection.Add((Direction)i);
        }
        return connectDirection;
    }
}
