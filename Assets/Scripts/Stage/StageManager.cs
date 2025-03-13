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

    /// <summary>区画のデータリスト</summary>
    private List<Section> _sectionList = null;

    /// <summary>部屋でない区画のIDリスト</summary>
    private List<int> _unroomSectionIDList = null;

    private Entity_StageData.Param _stageMaster = null;

    private int _stageSectionCount = -1;

    public const float SECTION_SIZE = 10.0f;

    public const float SECTION_HEIGHT = 3.0f;

    public override void Initialize()
    {
        _stageMaster = StageMasterUtility.GetStageMaster();
        _stageSectionCount = _stageMaster.widthSize * _stageMaster.heightSize;
        // 区画を初期化
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
	/// IDを2次元座標に変換
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
    /// 2次元座標をIDに変換
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
    /// ステージの作成
    /// </summary>
    private void CreateStage()
    {
        // 部屋の位置決定と接続
        List<Section> roomList = DecideRoom();
        ConnectRoom(roomList);
    }

    /// <summary>
    /// 部屋の位置を決める
    /// </summary>
    /// <returns></returns>
    private List<Section> DecideRoom()
    {
        List<Section> roomList = new List<Section>(_unroomSectionIDList.Count);
        // スタート部屋決定
        int startWidth = Random.Range(0, _stageMaster.widthSize + 1);
        Section startRoom = DecideRoomType(new Vector2Int(startWidth, 0), RoomType.StartRoom);
        roomList.Add(startRoom);

        // 各部屋の位置を決定
        DecideNormalRoom(_stageMaster.roomCount, roomList);

        // 鍵部屋決定
        int keyWidth = Random.Range(0, _stageMaster.widthSize + 1);
        Section keyRoom = DecideRoomType(new Vector2Int(keyWidth, 0), RoomType.KeyRoom);
        roomList.Add(keyRoom);

        return roomList;
    }
    
    /// <summary>
    /// 座標指定の部屋の種類決定
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
    /// ID指定の部屋の種類決定
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
    /// 数指定の通常部屋の決定
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
    /// 部屋と部屋をつなぐ
    /// </summary>
    /// <param name="roomList"></param>
    private void ConnectRoom(List<Section> roomList)
    {
        // 部屋が隣接しているならつなぐ
        for (int i = 0, max = roomList.Count - 1; i < max; i++)
        {
            ConnectNextRoom(roomList[i]);
        }

        // 部屋を順番につないでいく
        for (int i = 0, max = roomList.Count - 1; i < max; i++)
        {
            // A*で部屋1から部屋2をつなぐ
            // 最短経路が複数ある場合は既存の道を最も利用しているルートにする
            // ないならランダムでつなぐ

        }
    }

    /// <summary>
    /// 部屋を周囲の部屋とつなげる
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

            // 両部屋をつなげる
            sourceSection.SetIsConnect(direction, true);
            nextSection.SetIsConnect(direction.ReverseDir(), true);
        }
    }

    /// <summary>
    /// ステージの生成
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
    /// 部屋の生成
    /// </summary>
    /// <param name="ID"></param>
    private Transform GenerateRoom(int ID)
    {
        Section room = GetSection(ID);
        GameObject generateObject = null;
        int rotateCount = 0;
        // 部屋の種類によって生成内容設定
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

        // 座標、回転を設定して生成
        return GenerateObject(generateObject, room.position, rotateCount);
    }

    /// <summary>
    /// 廊下の生成
    /// </summary>
    /// <param name="ID"></param>
    private void GenerateCorridor(int ID)
    {
        // 廊下の種類決定
        Section corridor = GetSection(ID);
        CorridorType corridorType = _sectionList[ID].GetCorridorType();
        if (corridorType == CorridorType.Invalid) return;
        GameObject generateObject = sectionObjectAssign.corridorObjectList[(int)corridorType];
        if (generateObject == null) return;

        // 回転決定
        int rotateCount = (int)corridor.GetRotate();
        // 座標、回転を設定して生成
        GenerateObject(generateObject, corridor.position, rotateCount);
    }

    /// <summary>
    /// オブジェクトを生成する
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
    /// 2次元座標を3次元のワールド座標で返す
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
    /// 新しく部屋を追加してステージ再生成
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
    /// ステージオブジェクト全削除
    /// </summary>
    private void DestroyAllObject()
    {
        foreach (Transform child in _stageRoot)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// 指定方向の区画を取得
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
    /// ID指定の区画取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public Section GetSection(int ID)
    {
        if (!IsEnableIndex(_sectionList, ID)) return null;
        return _sectionList[ID];
    }
}
