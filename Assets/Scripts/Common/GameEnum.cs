public enum ItemType
{

}

public enum ItemID
{
    INVALID = -1,
    BATTERY_SMALL,
    BATTERY_LARGE,
    LANDMARK,
    COMPASS,
    EXIT_KEY,
}

public enum BGM
{
    TITLE = 0,
    MAIN_NORMAL,
    MAIN_TRACKING,
    OTHER,

    MAX
}

public enum SE
{
    UI_DECIDE = 0,
    DOOR_OPEN,
    DOOR_CLOSE,
    DOOR_UNLOCK,
    PLAYER_SURPRISE,
    WALK,
    RUN,
    CAUGHT,
    ITEM_PICK,

    MAX
}

// 4����
public enum Direction
{
    Invalid = -1,
    Up,
    Right,
    Down,
    Left,
    Max
}

// �����̎��
public enum RoomType
{
    Invalid = -1,
    NormalRoom,           // �ʏ핔��
    StartRoom,      // �X�^�[�g����
    KeyRoom,        // ������
    Max
}

// �L���̎��
public enum CorridorType
{
    Invalid = -1,
    I,
    L,
    T,
    X,
    Max
}