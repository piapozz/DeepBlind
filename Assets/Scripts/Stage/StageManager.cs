/*
* @file StageManager.cs
* @brief �X�e�[�W�̊Ǘ��N���X
* @author sakakura
* @date 2025/3/14
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Cysharp.Threading.Tasks;

using static CommonModule;

public class StageManager : MonoBehaviour
{
    public static StageManager instance { get; private set; } = null;
    [SerializeField]
    public SectionObjectAssign sectionObjectAssign = null;
    /// <summary>�X�e�[�W�̐�����</summary>
    [SerializeField]
    private Transform _activeRoot = null;
    [SerializeField]
    private Transform _unactiveRoot = null;
    [SerializeField]
    private NavMeshSurface _meshSurface = null;
    /// <summary>�X�e�[�W�̃}�X�^�[�f�[�^</summary>
    private Entity_StageData.Param _stageMaster = null;
    /// <summary>���̃f�[�^���X�g</summary>
    private List<SectionData> _sectionDataList = null;
    private List<SectionObject> _sectionObjectList = null;
    private List<SectionData> _enemySearchRoomList = null;
    private List<SectionData> _corridorList = null;
    /// <summary>�����łȂ�����ID���X�g</summary>
    private List<int> _unroomSectionIDList = null;
    /// <summary>�X�^�[�g����</summary>
    private SectionData _startRoom = null;
    /// <summary>������</summary>
    private SectionData _keyRoom = null;
    private List<Transform> _enemyAnchorList = null;
    private List<Transform> _itemAnchorList = null;
    private List<Transform> _lockerAnchorList = null;
    /// <summary>�X�e�[�W�̋��̐�</summary>
    public int stageSectionCount { get; private set; } = -1;
    public Vector2Int stageSize { get; private set; } = -Vector2Int.one;
    /// <summary>���̃T�C�Y</summary>
    public const float SECTION_SIZE = 10.0f;
    /// <summary>���̍���</summary>
    public const float SECTION_HEIGHT = 3.0f;
    /// <summary>���̗]��</summary>
    private const float _SECTION_MARGIN = 0.12f;

    public async UniTask Initialize()
    {
        instance = this;
        SectionData.SetGetObjectCallback(GetSectionObject);
        _stageMaster = StageMasterUtility.GetStageMaster();
        stageSize = new Vector2Int(_stageMaster.widthSize, _stageMaster.heightSize);
        stageSectionCount = stageSize.x * stageSize.y;
        // ����������
        _sectionDataList = new List<SectionData>(stageSectionCount);
        _sectionObjectList = new List<SectionObject>(stageSectionCount);
        _enemySearchRoomList = new List<SectionData>(stageSectionCount);
        _corridorList = new List<SectionData>(stageSectionCount);
        _unroomSectionIDList = new List<int>(stageSectionCount);
        for (int i = 0; i < stageSectionCount; i++)
        {
            SectionData createSection = new SectionData();
            Vector2Int position = GetSectionPosition(i);
            createSection.Initialize(i, position);
            _sectionDataList.Add(createSection);
            _sectionObjectList.Add(null);
            _unroomSectionIDList.Add(i);
        }

        // �X�e�[�W�쐬
        await CreateInitialStage(_stageMaster.normalRoomCount);
        // �X�e�[�W����
        await GenerateStage();
    }

    /// <summary>
    /// �x�C�N����
    /// </summary>
    public async UniTask NavMeshBake()
    {
        _meshSurface.BuildNavMesh();
    }

    /// <summary>
    /// ���̃I�u�W�F�N�g���擾
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    private SectionObject GetSectionObject(int ID)
    {
        if (!IsEnableIndex(_sectionObjectList, ID)) return null;

        return _sectionObjectList[ID];
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
    private async UniTask CreateInitialStage(int normalRoomCount)
    {
        // �����̃��X�g�̏�����
        List<SectionData> roomList = InitializeRoomList(normalRoomCount + 2);
        // �X�^�[�g��������
        DecideStartRoom(roomList);
        // ����������
        DecideKeyRoom(roomList);
        // �e�����̈ʒu������
        DecideNormalRoom(normalRoomCount, roomList);
        // �������Ȃ���
        await ConnectRoom(roomList);
    }

    /// <summary>
    /// ���������������̃��X�g���擾
    /// </summary>
    /// <param name="roomCount"></param>
    /// <returns></returns>
    private List<SectionData> InitializeRoomList(int roomCount)
    {
        // �����̃��X�g�̏�����
        List<SectionData> roomList = new List<SectionData>(roomCount);
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
    private void DecideStartRoom(List<SectionData> roomList)
    {
        int startWidth = Random.Range(0, _stageMaster.widthSize);
        SectionData setStartRoom = DecideRoomType(new Vector2Int(startWidth, 0), RoomType.StartRoom);
        roomList[0] = setStartRoom;
        _startRoom = setStartRoom;
    }

    /// <summary>
    /// �����������߂�
    /// </summary>
    /// <returns></returns>
    private void DecideKeyRoom(List<SectionData> roomList)
    {
        int keyWidth = Random.Range(0, _stageMaster.widthSize);
        SectionData setKeyRoom = DecideRoomType(new Vector2Int(keyWidth, _stageMaster.heightSize - 1), RoomType.KeyRoom);
        roomList[roomList.Count - 1] = setKeyRoom;
        _keyRoom = setKeyRoom;
    }

    /// <summary>
    /// ���W�w��̕����̎�ތ���
    /// </summary>
    /// <param name="position"></param>
    /// <param name="type"></param>
    private SectionData DecideRoomType(Vector2Int position, RoomType type)
    {
        int ID = GetSectionID(position);
        SectionData decideRoom = GetSection(ID);
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
    private SectionData DecideRoomType(int ID, RoomType type)
    {
        SectionData decideRoom = GetSection(ID);
        decideRoom.SetRoomType(type);
        _unroomSectionIDList.Remove(ID);
        return decideRoom;
    }

    /// <summary>
    /// ���w��̒ʏ핔���̌���
    /// </summary>
    /// <param name="roomCount"></param>
    /// <param name="roomList"></param>
    private void DecideNormalRoom(int roomCount, List<SectionData> roomList)
    {
        for (int i = 0; i < roomCount; i++)
        {
            if (_unroomSectionIDList.Count == 0) return;
            int randomIndex = Random.Range(0, _unroomSectionIDList.Count);
            int roomID = _unroomSectionIDList[randomIndex];
            SectionData normalRoom = DecideRoomType(roomID, RoomType.NormalRoom);
            int unuseIndex = GetUnuseIndex(roomList);
            roomList[unuseIndex] = normalRoom;
        }
    }

    /// <summary>
    /// �����ƕ������Ȃ�
    /// </summary>
    /// <param name="roomList"></param>
    private async UniTask ConnectRoom(List<SectionData> roomList)
    {
        int roomCount = roomList.Count;
        // �������אڂ��Ă���Ȃ�Ȃ�
        for (int i = 0; i < roomCount - 1; i++)
        {
            ConnectAroundRoom(roomList[i]);
        }

        // ���������ԂɂȂ��ł���
        for (int i = 0; i < roomCount; i++)
        {
            await ConnectNextRoom(roomList, i);
        }

        await UniTask.DelayFrame(1);
    }

    /// <summary>
    /// ���������͂̕����ƂȂ���
    /// </summary>
    /// <param name="sourceSection"></param>
    private void ConnectAroundRoom(SectionData sourceSection)
    {
        if (!sourceSection.IsRoom()) return;

        for (int i = 0, max = (int)Direction.Max; i < max; i++)
        {
            if (sourceSection.isConnect[i]) continue;

            Direction direction = (Direction)i;
            SectionData nextSection = GetSectionDir(sourceSection, direction);
            if (nextSection == null || !nextSection.IsRoom()) continue;

            // ���������Ȃ���
            sourceSection.SetIsConnect(direction, true);
            nextSection.SetIsConnect(direction.ReverseDir(), true);
        }
    }

    /// <summary>
    /// ���̕����ƂȂ���
    /// </summary>
    /// <param name="roomList"></param>
    /// <param name="roomCount"></param>
    private async UniTask ConnectNextRoom(List<SectionData> roomList, int roomCount)
    {
        // �Ō�̕����Ȃ�ŏ��ƂȂ���
        int nextRoomCount = roomCount + 1;
        if (nextRoomCount >= roomList.Count)
            nextRoomCount = 0;

        List<MoveData> route;
        // A*�ŕ���1���畔��2���Ȃ�
        route = await RouteSearcher.RouteSearch(roomList[roomCount].ID, roomList[nextRoomCount].ID, roomList[roomCount].preConnect);
        if (route == null) return;

        // ���[�g��������Ȃ���
        ConnectRouteSection(route);

        await UniTask.DelayFrame(1);
    }

    /// <summary>
    /// �w��̃��[�g���̋�擯�m���Ȃ��A�Ō�̕�����Ԃ�
    /// </summary>
    /// <param name="route"></param>
    /// <returns></returns>
    private void ConnectRouteSection(List<MoveData> route)
    {
        Direction lastDirection = Direction.Invalid;
        for (int i = 0, max = route.Count; i < max; i++)
        {
            Direction direction = route[i].direction;
            lastDirection = direction.ReverseDir();
            // �ڑ������Ȃ���
            SectionData sourceSection = GetSection(route[i].sourceSectionID);
            sourceSection.SetIsConnect(direction, true);
            sourceSection.SetPreConnect(direction);
            // �ڑ�����Ȃ���
            SectionData targetSection = GetSection(route[i].targetSectionID);
            targetSection.SetIsConnect(lastDirection, true);
            targetSection.SetPreConnect(lastDirection);
        }
    }

    /// <summary>
    /// �X�e�[�W�̐���
    /// </summary>
    private async UniTask GenerateStage()
    {
        for (int i = 0, max = _sectionDataList.Count; i < max; i++)
        {
            SectionData section = GetSection(i);
            if (section == null) continue;

            Transform generateRoot;
            if (section.IsRoom())
                generateRoot = GenerateRoom(i);
            else
                generateRoot = GenerateCorridor(i);

            if (generateRoot == null) continue;

            section.GenerateSeparate(generateRoot);

            await UniTask.DelayFrame(1);
        }

        // �e��A���J�[�̎擾
        CollectEnemyAnchor();
        CollectItemAnchor();
        CollectLockerAnchor();
    }

    /// <summary>
    /// �����̐���
    /// </summary>
    /// <param name="ID"></param>
    private Transform GenerateRoom(int ID)
    {
        SectionData room = GetSection(ID);
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
                _enemySearchRoomList.Add(room);
                break;
            case RoomType.StartRoom:
                generateObject = sectionObjectAssign.startRoom;
                rotateCount = (int)GetConnectSection(ID)[0];
                break;
            case RoomType.KeyRoom:
                generateObject = sectionObjectAssign.keyRoom;
                rotateCount = (int)GetConnectSection(ID)[0];
                break;
        }
        if (generateObject == null) return null;

        // ���W�A��]��ݒ肵�Đ���
        return GenerateObject(generateObject, room, rotateCount);
    }

    /// <summary>
    /// �L���̐���
    /// </summary>
    /// <param name="ID"></param>
    private Transform GenerateCorridor(int ID)
    {
        // �L���̎�ތ���
        SectionData corridor = GetSection(ID);
        _sectionDataList[ID].SetCorridorType();
        CorridorType corridorType = _sectionDataList[ID].corridorType;
        if (corridorType == CorridorType.Invalid) return null;
        corridor.SetCorridorType(corridorType);
        GameObject generateObject = sectionObjectAssign.corridorObjectList[(int)corridorType];
        if (generateObject == null) return null;

        _corridorList.Add(corridor);
        // ��]����
        int rotateCount = (int)corridor.GetRotate();
        // ���W�A��]��ݒ肵�Đ���
        return GenerateObject(generateObject, corridor, rotateCount);
    }

    /// <summary>
    /// �I�u�W�F�N�g�𐶐�����
    /// </summary>
    /// <param name="generateObject"></param>
    /// <param name="sectionPosition"></param>
    /// <param name="rotateCount"></param>
    private Transform GenerateObject(GameObject generateObject, SectionData section, int rotateCount)
    {
        Vector3 position = GetSectionWorldPosition(section.position);
        Quaternion rotation = Quaternion.Euler(0, 90 * rotateCount, 0);
        section.SetDirection((Direction)rotateCount);

        Transform objectTransform = Instantiate(generateObject, position, rotation, _activeRoot).transform;
        // ���̃I�u�W�F�N�g�擾
        SectionObject sectionObject = objectTransform.GetComponent<SectionObject>();
        int sectionID = section.ID;
        _sectionObjectList[sectionID] = sectionObject;
        return objectTransform;
    }

    /// <summary>
    /// 2�������W��3�����̃��[���h���W�ŕԂ�
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public Vector3 GetSectionWorldPosition(Vector2Int position)
    {
        float x = position.x * SECTION_SIZE + position.x * _SECTION_MARGIN;
        float z = position.y * SECTION_SIZE + position.y * _SECTION_MARGIN;

        return new Vector3(x, 0, z);
    }

    /// <summary>
    /// �G�l�~�[�̃A���J�[���W��
    /// </summary>
    private void CollectEnemyAnchor()
    {
        // �X�e�[�W�̂��ׂẴG�l�~�[�A���J�[���W��
        _enemyAnchorList = new List<Transform>();
        for (int i = 0, sectionMax = stageSectionCount; i < sectionMax; i++)
        {
            if (_sectionObjectList[i] == null) continue;

            List<Transform> enemyAnchorList = _sectionObjectList[i].GetEnemyAnchor();
            _enemyAnchorList.AddRange(enemyAnchorList);
        }
    }

    /// <summary>
    /// �A�C�e���A���J�[���W��
    /// </summary>
    private void CollectItemAnchor()
    {
        // �X�e�[�W�̂��ׂẴA�C�e���A���J�[���W��
        _itemAnchorList = new List<Transform>();
        for (int i = 0, sectionMax = stageSectionCount; i < sectionMax; i++)
        {
            if (_sectionObjectList[i] == null) continue;

            List<Transform> itemAnchorList = _sectionObjectList[i].GetItemAnchor();
            _itemAnchorList.AddRange(itemAnchorList);
        }
    }

    /// <summary>
    /// ���b�J�[�A���J�[���W��
    /// </summary>
    private void CollectLockerAnchor()
    {
        // �X�e�[�W�̂��ׂẴ��b�J�[�A���J�[���W��
        _lockerAnchorList = new List<Transform>();
        for (int i = 0, sectionMax = stageSectionCount; i < sectionMax; i++)
        {
            if (_sectionObjectList[i] == null) continue;

            List<Transform> lockerAnchorList = _sectionObjectList[i].GetLockerAnchor();
            _lockerAnchorList.AddRange(lockerAnchorList);
        }
    }

    /// <summary>
    /// �V����������ǉ����ăX�e�[�W�Đ���
    /// </summary>
    /// <param name="addRoomCount"></param>
    public void RegenerateSection(int addRoomCount)
    {
        // �I�u�W�F�N�g�̍폜
        DestroyAllObject();
        // ����������X�^�[�g�����Ɍo�H����
        List<SectionData> roomList = InitializeRoomList(addRoomCount + 2);
        roomList[0] = _keyRoom;
        roomList[roomList.Count - 1] = _startRoom;
        DecideNormalRoom(addRoomCount, roomList);
        ConnectRoom(roomList);
        // �I�u�W�F�N�g�̐���������
        GenerateStage();
    }

    /// <summary>
    /// �X�e�[�W�I�u�W�F�N�g�S�폜
    /// </summary>
    private void DestroyAllObject()
    {
        foreach (Transform child in _activeRoot)
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
    public SectionData GetSectionDir(SectionData sourceSection, Direction direction)
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
    public SectionData GetSectionDir(int sourceSectionID, Direction direction)
    {
        SectionData sourceSection = GetSection(sourceSectionID);
        Vector2Int targetPosition = sourceSection.position + direction.Vector2();
        return GetSection(GetSectionID(targetPosition));
    }

    /// <summary>
    /// ID�w��̋��擾
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public SectionData GetSection(int ID)
    {
        if (!IsEnableIndex(_sectionDataList, ID)) return null;
        return _sectionDataList[ID];
    }

    /// <summary>
    /// �Ȃ����Ă���������擾
    /// </summary>
    /// <param name="sectionID"></param>
    /// <returns></returns>
    public List<Direction> GetConnectSection(int sectionID)
    {
        List<Direction> connectDirection = new List<Direction>((int)Direction.Max);
        SectionData section = GetSection(sectionID);
        for (int i = 0, max = (int)Direction.Max; i < max; i++)
        {
            if (section.isConnect[i]) connectDirection.Add((Direction)i);
        }
        return connectDirection;
    }

    /// <summary>
    /// �X�^�[�g������3�������W���擾
    /// </summary>
    /// <returns></returns>
    public Vector3 GetStartRoomPosition()
    {
        Vector2Int startPosition = _startRoom.position;
        return GetSectionWorldPosition(startPosition);
    }

    /// <summary>
    /// ��������3�������W���擾
    /// </summary>
    /// <returns></returns>
    public Vector3 GetKeyRoomPosition()
    {
        Vector2Int keyPosition = _keyRoom.position;
        return GetSectionWorldPosition(keyPosition);
    }

    /// <summary>
    /// �G�l�~�[�̃A���J�[�����Ԃ�Ȃ������_���擾
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetRandomEnemyAnchorList(int getCount)
    {
        // ���ׂẴG�l�~�[�A���J�[���炩�Ԃ�Ȃ��Œ��o
        List<Transform> result = new List<Transform>();
        for (int i = 0; i < getCount; i++)
        {
            if (_enemyAnchorList[i] == null) continue;
            int randomIndex = Random.Range(0, _enemyAnchorList.Count);
            result.Add(_enemyAnchorList[randomIndex]);
            _enemyAnchorList.RemoveAt(randomIndex);
        }
        return result;
    }

    /// <summary>
    /// �����_���ȕ����̃G�l�~�[�A���J�[���擾
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetRandomEnemyAnchor()
    {
        int randomIndex = Random.Range(0, _enemySearchRoomList.Count);
        int sectionID = _enemySearchRoomList[randomIndex].ID;
        return _sectionObjectList[sectionID].GetEnemyAnchor();
    }

    /// <summary>
    /// ���W�w��̃G�l�~�[�A���J�[�擾
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public List<Transform> GetEnemyAnchor(Vector3 position)
    {
        Vector2Int sectionPosition = GetSectionPosition(position);
        int sectionID = GetSectionID(sectionPosition);
        return _sectionObjectList[sectionID].GetEnemyAnchor();
    }

    /// <summary>
    /// ���[���h���W������̍��W���擾
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Vector2Int GetSectionPosition(Vector3 pos)
    {
        float width = pos.x / SECTION_SIZE + 0.5f;
        float height = pos.z / SECTION_SIZE + 0.5f;
        return new Vector2Int(Mathf.FloorToInt(width), Mathf.FloorToInt(height));
    }

    /// <summary>
    /// �����_���ȃA�C�e���A���J�[���擾
    /// </summary>
    /// <returns></returns>
    public Transform GetRandomItemAnchor()
    {
        if (IsEmpty(_itemAnchorList)) return null;
        int randomIndex = Random.Range(0, _itemAnchorList.Count);
        Transform result = _itemAnchorList[randomIndex];
        _itemAnchorList.RemoveAt(randomIndex);
        return result;
    }

    /// <summary>
    /// �����_���ȃ��b�J�[�A���J�[���擾
    /// </summary>
    /// <returns></returns>
    public Transform GetRandomLockerAnchor()
    {
        if (IsEmpty(_lockerAnchorList)) return null;
        int randomIndex = Random.Range(0, _lockerAnchorList.Count);
        Transform result = _lockerAnchorList[randomIndex];
        _lockerAnchorList.RemoveAt(randomIndex);
        return result;
    }

    /// <summary>
    /// �v���C���[�̏����ʒu���擾
    /// </summary>
    /// <returns></returns>
    public Transform GetPlayerStartTransform()
    {
        return _sectionObjectList[_startRoom.ID].GetPlayerAnchor()[0];
    }

    public Transform GetActiveRoot()
    {
        return _activeRoot;
    }

    public Transform GetUnactiveRoot()
    {
        return _unactiveRoot;
    }

    /// <summary>
    /// ��Еt������
    /// </summary>
    public void Teardown()
    {
        // �I�u�W�F�N�g�̍폜
        for (int i = 0, max = _sectionObjectList.Count; i < max; i++)
        {
            _sectionObjectList[i].Teardown();
        }
    }
}
