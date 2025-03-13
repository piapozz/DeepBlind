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

// 4方向
public enum Direction
{
    Invalid = -1,
    Up,
    Right,
    Down,
    Left,
    Max
}

// 部屋の種類
public enum RoomType
{
    Invalid = -1,
    NormalRoom,           // 通常部屋
    StartRoom,      // スタート部屋
    KeyRoom,        // 鍵部屋
    Max
}

// 廊下の種類
public enum CorridorType
{
    Invalid = -1,
    I,
    L,
    T,
    X,
    Max
}