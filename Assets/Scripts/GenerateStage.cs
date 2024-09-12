using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenerateStage : MonoBehaviour
{
    // ステージを格納する空オブジェクト
    [SerializeField] Transform stage;
    // ステージの部品のプレハブ
    [SerializeField] GameObject[] rooms;
    [SerializeField] GameObject startRoom;
    [SerializeField] GameObject goalRoom;
    [SerializeField] GameObject ICorridor;
    [SerializeField] GameObject LCorridor;
    [SerializeField] GameObject TCorridor;
    [SerializeField] GameObject XCorridor;
    [SerializeField] GameObject door;
    [SerializeField] GameObject wall;

    const int WIDTH_ROOM_MAX = 6;               // 横の部屋の数
    const int HEIGHT_ROOM_MAX = 4;              // 縦の部屋の数
    const float SECTION_SIZE = 10;              // 区画のサイズ
    const float ROOM_HEIGHT = 3;                // 部屋の高さ
    const int GENERATE_ATTEMPT_MAX = 100;       // 生成試行回数の最大値
    const float CONECT_PROBABILITY = 0.4f;      // 区画がつながる可能性

    int startPosX;       // スタートの位置
    int goalPosX;        // ゴールの位置

    GameObject[] corridors;     // 廊下のプレハブ

    List<int[]> roomPos = new List<int[]>();      // 部屋の配列
    List<int[]> corridorPos = new List<int[]>();  // 廊下の配列

    // 方角
    public enum Direction
    {
        North = 0,
        East,
        South,
        West,
        Max
    }

    // 区画の種類
    public enum SectionType
    {
        None = 0,
        Room,           // 部屋
        StartRoom,      // スタート部屋
        GoalRoom,       // ゴール部屋
        OverCorridor,   // 部屋と部屋をつなぐ廊下(渡し廊下)
        CrossCorridor,  // 廊下と廊下をつなぐ廊下(交差廊下)
        Max
    }

    // 廊下の種類
    public enum CorridorForm
    {
        I = 0,
        L,
        T,
        X,
        Max
    }

    // ステージの構造体
    public struct Section
    {
        public SectionType type;            // 種類
        public Direction rotate;            // 回転
        public CorridorForm corridorForm;   // 廊下の形
        public int roomForm;                // 部屋の形
        public bool[] connect;              // 東西南北につながっているか
        public bool route;                  // ゴールまでのルートか

        // 接続情報の初期化
        public void Init()
        {
            connect = new bool[(int)Direction.Max];
            // 通れるで初期化
            for(int i = 0; i < (int)Direction.Max; i++)
            {
                connect[i] = false;
            }
        }
    }
    Section[,] creatLayout;     // ステージの構造体の二次元配列
    Section[,] stageLayout;     // 完成したステージの構造体

    // ゴールまでのルートを記録する構造体
    struct Route
    {
        public bool throuth;
        public bool[] selected;

        // 接続情報の初期化
        public void Init()
        {
            selected = new bool[(int)Direction.Max];
            // 通れるで初期化
            for (int i = 0; i < (int)Direction.Max; i++)
            {
                selected[i] = false;
            }
            throuth = false;
        }
    }
    Route[,] route;         // ルート構築に使う構造体の二次元配列
    Route[,] judgeConnect;  // スタートからつながっているかを確かめるのに使う

    void Awake()
    {
        // 初期化
        Init();

        // ステージの設計
        StageLayout();

        // 区画の生成
        GenerateSection();
    }

    // 初期化
    void Init()
    {
        // ステージの二次元配列
        creatLayout = new Section[WIDTH_ROOM_MAX * 2 - 1, HEIGHT_ROOM_MAX * 2 - 1];

        // ステージの二次元配列の初期化
        for (int w = 0; w < creatLayout.GetLength(0); w++)
        {
            for (int h = 0; h < creatLayout.GetLength(1); h++)
            {
                // 構造体内の接続情報配列の初期化
                creatLayout[w, h].Init();

                creatLayout[w, h].type = SectionType.None;

                // 部屋
                if (w % 2 == 0 && h % 2 == 0) 
                    creatLayout[w, h].type = SectionType.Room;
                // 交差廊下
                else if (w % 2 != 0 && h % 2 != 0)
                    creatLayout[w, h].type = SectionType.CrossCorridor;
                // 渡し廊下
                else
                    creatLayout[w, h].type = SectionType.OverCorridor;
            }
        }

        // 廊下のプレハブの配列
        corridors = new GameObject[(int)CorridorForm.Max];
        corridors[0] = ICorridor;
        corridors[1] = LCorridor;
        corridors[2] = TCorridor;
        corridors[3] = XCorridor;
    }

    // ステージの設計をする関数
    void StageLayout()
    {
        // ゴールまでのルートを担保
        DicisionRoute();

        // 各マスがどこにつながっているかを決める
        DicisionSectionConnect();

        // つながっていない部屋を消す
        DeleteDisconnectedSection();

        // 部屋を決定する
        DicisionRoom();

        // 廊下を決定する
        JudgeCorridorForm();

        // スタートとゴール地点を含めた配列に更新
        UpdateStageLayout();
    }

    // スタートとゴールを決めて経路を決める関数
    // スタートから進める区画がゴールに到達できるかを判定して決めていく
    void DicisionRoute()
    {
        // スタートとゴールの位置を決める
        int startRand = UnityEngine.Random.Range(0, WIDTH_ROOM_MAX);
        int goalRand = UnityEngine.Random.Range(0, WIDTH_ROOM_MAX);
        startPosX = startRand * 2;
        goalPosX = goalRand * 2;

        // 今の座標
        Vector2Int pos;
        // 移動方向を記録する可変長配列
        List<Direction> routeDir = new List<Direction>();
        // ループ回数のカウント
        int loopCount;
        // 完了フラグ
        bool finish = false;

        // 終了するまでループ
        while (true)
        {
            // 今の座標を初期化
            pos = new Vector2Int(startPosX, 0);

            // ルート情報を初期化
            routeDir.Clear();

            // 通行可能な方向のリスト
            List<Direction> passableDir = new List<Direction>();

            // ループ回数のカウント
            loopCount = 0;

            // ルートの構造体の初期化
            route = new Route[creatLayout.GetLength(0), creatLayout.GetLength(1)];
            // ステージの二次元配列の初期化
            for (int w = 0; w < route.GetLength(0); w++)
            {
                for (int h = 0; h < route.GetLength(1); h++)
                {
                    // 構造体内の接続情報配列の初期化
                    route[w, h].Init();

                    // 配列外なら選ばれないようにする
                    if (h == route.GetLength(1) - 1)
                        route[w, h].selected[(int)Direction.North] = true;
                    if (w == route.GetLength(0) - 1)
                        route[w, h].selected[(int)Direction.East] = true;
                    if (h == 0)
                        route[w, h].selected[(int)Direction.South] = true;
                    if (w == 0)
                        route[w, h].selected[(int)Direction.West] = true;
                }
            }

            // ゴールまでの道が出るまでループ
            while (true)
            {
                loopCount++;
                // 移動方向の候補の初期化
                passableDir.Clear();

                // 進める方向を判定する
                // 各方向がまだ選択していなくて通行していないなら候補に入れる
                // 北
                if (route[pos.x, pos.y].selected[(int)Direction.North] == false)
                    if (route[pos.x, pos.y + 1].throuth == false)
                        passableDir.Add(Direction.North);
                // 東
                if (route[pos.x, pos.y].selected[(int)Direction.East] == false)
                    if (route[pos.x + 1, pos.y].throuth == false)
                        passableDir.Add(Direction.East);
                // 南
                if (route[pos.x, pos.y].selected[(int)Direction.South] == false)
                    if (route[pos.x, pos.y - 1].throuth == false)
                        passableDir.Add(Direction.South);
                // 西
                if (route[pos.x, pos.y].selected[(int)Direction.West] == false)
                    if (route[pos.x - 1, pos.y].throuth == false)
                        passableDir.Add(Direction.West);

                // ゴールにたどり着いたら終了
                if (pos.x == goalPosX && pos.y == route.GetLength(1) - 1)
                {
                    finish = true;
                    break;
                }

                // 進行可能でないなら
                if (passableDir.Count == 0)
                {
                    // 今いるマスを初期化
                    route[pos.x, pos.y].Init();
                    // 配列外なら選ばれないようにする
                    if (pos.y == route.GetLength(1) - 1)
                        route[pos.x, pos.y].selected[(int)Direction.North] = true;
                    if (pos.x == route.GetLength(0) - 1)
                        route[pos.x, pos.y].selected[(int)Direction.East] = true;
                    if (pos.y == 0)
                        route[pos.x, pos.y].selected[(int)Direction.South] = true;
                    if (pos.x == 0)
                        route[pos.x, pos.y].selected[(int)Direction.West] = true;

                    // 方向によって座標の変更
                    switch (routeDir[routeDir.Count - 1])
                    {
                        case Direction.North:
                            pos.y -= 1;
                            break;
                        case Direction.East:
                            pos.x -= 1;
                            break;
                        case Direction.South:
                            pos.y += 1;
                            break;
                        case Direction.West:
                            pos.x += 1;
                            break;
                        default: break;
                    }

                    // 戻る
                    routeDir.RemoveAt(routeDir.Count - 1);
                }
                // 進行可能なら
                else
                {
                    // 進行方向の候補から抽選
                    Direction dir = passableDir[UnityEngine.Random.Range(0, passableDir.Count)];

                    // 通ったことを記録
                    route[pos.x, pos.y].throuth = true;
                    // 既に選択済みに保存
                    route[pos.x, pos.y].selected[(int)dir] = true;

                    // 進行方向を決定する
                    routeDir.Add(dir);

                    // 方向によって座標の変更
                    switch (dir)
                    {
                        case Direction.North:
                            pos.y += 1;
                            break;
                        case Direction.East:
                            pos.x += 1;
                            break;
                        case Direction.South:
                            pos.y -= 1;
                            break;
                        case Direction.West:
                            pos.x -= 1;
                            break;
                        default: break;
                    }
                }

                // ループしすぎていればやり直し
                if (loopCount >= GENERATE_ATTEMPT_MAX)
                    break;
            }

            Debug.Log("試行回数" + loopCount);
            // 完了してれば終了
            if (finish == true)
            {
                Debug.Log("ルート構築完了");
                break;
            }
            else
            {
                Debug.Log("ルート構築強制終了");
                continue;
            }
        }

        // ルートの接続
        // スタート地点
        pos.x = startPosX;
        pos.y = 0;
        // ルートを記録
        creatLayout[pos.x, pos.y].route = true;
        // 入口をつなぐ
        creatLayout[pos.x, pos.y].connect[(int)Direction.South] = true;
        for (int i = 0; i < routeDir.Count; i++)
        {
            // 各方向の接続を変更し、座標変更
            switch (routeDir[i])
            {
                case Direction.North:
                    creatLayout[pos.x, pos.y].connect[(int)Direction.North] = true;
                    creatLayout[pos.x, pos.y + 1].connect[(int)Direction.South] = true;
                    pos.y += 1;
                    break;
                case Direction.East:
                    creatLayout[pos.x, pos.y].connect[(int)Direction.East] = true;
                    creatLayout[pos.x + 1, pos.y].connect[(int)Direction.West] = true;
                    pos.x += 1;
                    break;
                case Direction.South:
                    creatLayout[pos.x, pos.y].connect[(int)Direction.South] = true;
                    creatLayout[pos.x, pos.y - 1].connect[(int)Direction.North] = true;
                    pos.y -= 1;
                    break;
                case Direction.West:
                    creatLayout[pos.x, pos.y].connect[(int)Direction.West] = true;
                    creatLayout[pos.x - 1, pos.y].connect[(int)Direction.East] = true;
                    pos.x -= 1;
                    break;
                default: break;
            }

            // ルートを記録
            creatLayout[pos.x, pos.y].route = true;
        }
        // 出口をつなぐ
        creatLayout[pos.x, pos.y].connect[(int)Direction.North] = true;
    }

    // 区画がどこにつながっているか決める関数
    void DicisionSectionConnect()
    {
        // 区画を網羅
        for (int w = 0; w < creatLayout.GetLength(0); w++)
        {
            for (int h = 0; h < creatLayout.GetLength(1); h++)
            {
                // 南の辺でないなら南をコピー
                if (h > 0)
                    creatLayout[w, h].connect[(int)Direction.South] =
                        creatLayout[w, h - 1].connect[(int)Direction.North];
                // 西の辺でないなら西をコピー
                if (w > 0)
                    creatLayout[w, h].connect[(int)Direction.West] =
                        creatLayout[w - 1, h].connect[(int)Direction.East];

                // 接続数
                int conectNum = 0;
                // 接続しているなら接続数をカウント
                if (creatLayout[w, h].connect[(int)Direction.North] == true)
                    conectNum++;
                if (creatLayout[w, h].connect[(int)Direction.East] == true)
                    conectNum++;
                if (creatLayout[w, h].connect[(int)Direction.South] == true)
                    conectNum++;
                if (creatLayout[w, h].connect[(int)Direction.West] == true)
                    conectNum++;

                // ステージの辺でなくてつながっていないところを候補に入れていく
                List<Direction> dir = new List<Direction>();
                // 北の辺でなくつながっていないなら
                if (h < creatLayout.GetLength(1) - 1 &&
                    creatLayout[w, h].connect[(int)Direction.North] == false)
                    dir.Add(Direction.North);
                // 東の辺でなくつながっていないなら
                if (w < creatLayout.GetLength(0) - 1 &&
                    creatLayout[w, h].connect[(int)Direction.East] == false)
                    dir.Add(Direction.East);

                float randConect;

                // つながっている数によって分岐
                switch (conectNum)
                {
                    // どこにもつながっていない場合
                    case 0:
                        // ２か所につながれるなら
                        if (dir.Count == 2)
                        {
                            // 確率でつなげるかを決める
                            randConect = UnityEngine.Random.value;
                            if (randConect < CONECT_PROBABILITY)
                            {
                                creatLayout[w, h].connect[(int)dir[0]] = true;
                                creatLayout[w, h].connect[(int)dir[1]] = true;
                            }
                        }
                        break;
                    // １か所だけつながっている場合
                    case 1:
                        // １か所につながれるなら
                        if (dir.Count == 1)
                            // つなげる
                            creatLayout[w, h].connect[(int)dir[0]] = true;

                        // ２か所につながれるなら
                        if (dir.Count == 2)
                        {
                            // １か所はつないで片方は確率
                            int randDir = UnityEngine.Random.Range(0, dir.Count);
                            creatLayout[w, h].connect[(int)dir[randDir]] = true;
                            dir.RemoveAt(randDir);

                            randConect = UnityEngine.Random.value;
                            if (randConect < CONECT_PROBABILITY)
                                creatLayout[w, h].connect[(int)dir[0]] = true;
                        }
                        break;
                    // ２か所つながっている場合
                    case 2:
                        // 確率でつなげる
                        if (dir.Count != 0)
                        {
                            for (int i = 0; i < dir.Count; i++)
                            {
                                randConect = UnityEngine.Random.value;
                                if (randConect < CONECT_PROBABILITY)
                                    creatLayout[w, h].connect[(int)dir[i]] = true;
                            }
                        }
                        break;
                    // ３か所つながっている場合
                    case 3:
                        // 確率でつなげる
                        randConect = UnityEngine.Random.value;
                        if (randConect < CONECT_PROBABILITY && dir.Count != 0)
                            creatLayout[w, h].connect[(int)dir[0]] = true;
                        break;
                    default: break;
                }
            }
        }
    }

    // つながっていない区画を消す関数
    void DeleteDisconnectedSection()
    {
        // 構造体の初期化
        judgeConnect = new Route[creatLayout.GetLength(0), creatLayout.GetLength(1)];
        // ステージの二次元配列の初期化
        for (int w = 0; w < judgeConnect.GetLength(0); w++)
        {
            for (int h = 0; h < judgeConnect.GetLength(1); h++)
            {
                // 構造体内の接続情報配列の初期化
                judgeConnect[w, h].Init();

                // 配列外なら選ばれないようにする
                if (h == judgeConnect.GetLength(1) - 1)
                    judgeConnect[w, h].selected[(int)Direction.North] = true;
                if (w == judgeConnect.GetLength(0) - 1)
                    judgeConnect[w, h].selected[(int)Direction.East] = true;
                if (h == 0)
                    judgeConnect[w, h].selected[(int)Direction.South] = true;
                if (w == 0)
                    judgeConnect[w, h].selected[(int)Direction.West] = true;
            }
        }

        // 今の座標を初期化
        // 今の座標
        Vector2Int pos = new Vector2Int(startPosX, 0);
        // 通行可能な方向のリスト
        List<Direction> passableDir = new List<Direction>();
        // 移動方向を記録する可変長配列
        List<Direction> routeDir = new List<Direction>();
        // スタート地点を通ったことにする
        judgeConnect[pos.x, pos.y].throuth = true;

        // すべての接続部を通り終えるまでループ
        while (true)
        {
            // 移動方向の候補の初期化
            passableDir.Clear();
            // 進める方向を判定する
            // 各方向がまだ選択していなくて通行していないなら候補に入れる
            // 北
            if (creatLayout[pos.x, pos.y].connect[(int)Direction.North] == true &&
                judgeConnect[pos.x, pos.y].selected[(int)Direction.North] == false)
                passableDir.Add(Direction.North);
            // 東
            if (creatLayout[pos.x, pos.y].connect[(int)Direction.East] == true &&
                judgeConnect[pos.x, pos.y].selected[(int)Direction.East] == false)
                passableDir.Add(Direction.East);
            // 南
            if (creatLayout[pos.x, pos.y].connect[(int)Direction.South] == true &&
                judgeConnect[pos.x, pos.y].selected[(int)Direction.South] == false)
                passableDir.Add(Direction.South);
            // 西
            if (creatLayout[pos.x, pos.y].connect[(int)Direction.West] == true &&
                judgeConnect[pos.x, pos.y].selected[(int)Direction.West] == false)
                passableDir.Add(Direction.West);

            // 進行可能でないなら
            if (passableDir.Count == 0)
            {
                // すべての接続部を通り終えたら終了
                if (routeDir.Count == 0) break;

                // 方向によって座標の変更
                switch (routeDir[routeDir.Count - 1])
                {
                    case Direction.North:
                        pos.y -= 1;
                        break;
                    case Direction.East:
                        pos.x -= 1;
                        break;
                    case Direction.South:
                        pos.y += 1;
                        break;
                    case Direction.West:
                        pos.x += 1;
                        break;
                    default: break;
                }

                // 戻る
                routeDir.RemoveAt(routeDir.Count - 1);
            }
            // 進行可能なら
            else
            {
                // 進行方向の候補から抽選
                Direction dir = passableDir[UnityEngine.Random.Range(0, passableDir.Count)];

                // 通ったことを記録
                judgeConnect[pos.x, pos.y].selected[(int)dir] = true;

                // 進行方向を記録
                routeDir.Add(dir);

                // 方向によって座標の変更と接続の更新
                switch (dir)
                {
                    case Direction.North:
                        pos.y += 1;
                        judgeConnect[pos.x, pos.y].selected[(int)Direction.South] = true;
                        break;
                    case Direction.East:
                        pos.x += 1;
                        judgeConnect[pos.x, pos.y].selected[(int)Direction.West] = true;
                        break;
                    case Direction.South:
                        pos.y -= 1;
                        judgeConnect[pos.x, pos.y].selected[(int)Direction.North] = true;
                        break;
                    case Direction.West:
                        pos.x -= 1;
                        judgeConnect[pos.x, pos.y].selected[(int)Direction.East] = true;
                        break;
                    default: break;
                }

                // 通ったことを記録
                judgeConnect[pos.x, pos.y].throuth = true;
            }
        }

        // 通過していない区画を消す
        for (int w = 0; w < judgeConnect.GetLength(0); w++)
        {
            for (int h = 0; h < judgeConnect.GetLength(1); h++)
            {
                if (judgeConnect[w, h].throuth == false)
                    creatLayout[w, h].type = SectionType.None;
            }
        }
    }

    // 部屋を決定する関数
    void DicisionRoom()
    {
        for (int w = 0; w < creatLayout.GetLength(0); w++)
        {
            for (int h = 0; h < creatLayout.GetLength(1); h++)
            {
                // 部屋
                if (creatLayout[w, h].type == SectionType.Room)
                {
                    // 接続数
                    int conectNum = 0;
                    if (creatLayout[w, h].connect[(int)Direction.North] == true)
                        conectNum++;
                    if (creatLayout[w, h].connect[(int)Direction.East] == true)
                        conectNum++;
                    if (creatLayout[w, h].connect[(int)Direction.South] == true)
                        conectNum++;
                    if (creatLayout[w, h].connect[(int)Direction.West] == true)
                        conectNum++;

                    // つながっていないなら
                    if (conectNum == 0)
                        // 部屋を消す
                        creatLayout[w, h].type = SectionType.None;

                    // ランダムな部屋を抽選
                    creatLayout[w, h].roomForm = UnityEngine.Random.Range(0, rooms.Length);
                    // ランダムな回転
                    creatLayout[w, h].rotate = (Direction)UnityEngine.Random.Range(0, (int)Direction.Max);
                }
            }
        }
    }

    // 廊下の種類を決定する関数
    void JudgeCorridorForm()
    {
        // 廊下の種類を決定する
        for (int w = 0; w < creatLayout.GetLength(0); w++)
        {
            for (int h = 0; h < creatLayout.GetLength(1); h++)
            {
                // 廊下でないなら次へ
                if (creatLayout[w, h].type != SectionType.OverCorridor &&
                    creatLayout[w, h].type != SectionType.CrossCorridor)
                    continue;

                // 接続数
                int conectNum = 0;
                if (creatLayout[w, h].connect[(int)Direction.North] == true)
                    conectNum++;
                if (creatLayout[w, h].connect[(int)Direction.East] == true)
                    conectNum++;
                if (creatLayout[w, h].connect[(int)Direction.South] == true)
                    conectNum++;
                if (creatLayout[w, h].connect[(int)Direction.West] == true)
                    conectNum++;

                // つながっていないなら
                if (conectNum == 0 || conectNum == 1)
                    // 廊下を消す
                    creatLayout[w, h].type = SectionType.None;
                // 4辺つながっているならX
                else if (conectNum == 4)
                    creatLayout[w, h].corridorForm = CorridorForm.X;
                // 3辺つながっているならT
                else if (conectNum == 3)
                {
                    creatLayout[w, h].corridorForm = CorridorForm.T;
                    // 北側
                    if (creatLayout[w, h].connect[(int)Direction.North] == false)
                        creatLayout[w, h].rotate = Direction.North;
                    // 東側
                    if (creatLayout[w, h].connect[(int)Direction.East] == false)
                        creatLayout[w, h].rotate = Direction.East;
                    // 南側
                    if (creatLayout[w, h].connect[(int)Direction.South] == false)
                        creatLayout[w, h].rotate = Direction.South;
                    // 西側
                    if (creatLayout[w, h].connect[(int)Direction.West] == false)
                        creatLayout[w, h].rotate = Direction.West;
                }
                // 2辺つながっていて
                else
                {
                    // I
                    // 縦か横でつながっているなら
                    if (creatLayout[w, h].connect[(int)Direction.North] == true &&
                        creatLayout[w, h].connect[(int)Direction.South] == true ||
                        creatLayout[w, h].connect[(int)Direction.East] == true &&
                        creatLayout[w, h].connect[(int)Direction.West] == true)
                    {
                        // 種類設定
                        creatLayout[w, h].corridorForm = CorridorForm.I;
                        // 回転設定
                        if (creatLayout[w, h].connect[(int)Direction.North] == true)
                            creatLayout[w, h].rotate = Direction.North;
                        else
                            creatLayout[w, h].rotate = Direction.East;
                    }
                    // L
                    else
                    {
                        // 種類設定
                        creatLayout[w, h].corridorForm = CorridorForm.L;
                        // 回転設定
                        if (creatLayout[w, h].connect[(int)Direction.North] == true &&
                            creatLayout[w, h].connect[(int)Direction.East] == true)
                            creatLayout[w, h].rotate = Direction.North;
                        else if (creatLayout[w, h].connect[(int)Direction.East] == true &&
                            creatLayout[w, h].connect[(int)Direction.South] == true)
                            creatLayout[w, h].rotate = Direction.East;
                        else if (creatLayout[w, h].connect[(int)Direction.South] == true &&
                            creatLayout[w, h].connect[(int)Direction.West] == true)
                            creatLayout[w, h].rotate = Direction.South;
                        else
                            creatLayout[w, h].rotate = Direction.West;
                    }
                }
            }
        }
    }

    // スタートとゴール地点を含めた配列に更新する関数
    void UpdateStageLayout()
    {
        stageLayout = new Section[creatLayout.GetLength(0), creatLayout.GetLength(1) + 4];

        // 配列の初期化
        for (int w = 0; w < stageLayout.GetLength(0); w++)
        {
            for (int h = 0; h < stageLayout.GetLength(1); h++)
            {
                stageLayout[w, h].Init();
            }
        }

        // コピー
        for (int w = 0; w < creatLayout.GetLength(0); w++)
        {
            for (int h = 0; h < creatLayout.GetLength(1); h++)
            {
                stageLayout[w, h + 2] = creatLayout[w, h];
            }
        }

        // スタート部屋
        stageLayout[startPosX, 0].type = SectionType.StartRoom;
        stageLayout[startPosX, 0].connect[(int)Direction.North] = true;
        stageLayout[startPosX, 0].route = true;
        // スタート廊下
        stageLayout[startPosX, 1].type = SectionType.CrossCorridor;
        stageLayout[startPosX, 1].corridorForm = CorridorForm.I;
        stageLayout[startPosX, 1].connect[(int)Direction.North] = true;
        stageLayout[startPosX, 1].connect[(int)Direction.South] = true;
        stageLayout[startPosX, 1].route = true;
        // ゴール部屋
        stageLayout[goalPosX, stageLayout.GetLength(1) - 1].type = SectionType.GoalRoom;
        stageLayout[goalPosX, stageLayout.GetLength(1) - 1].connect[(int)Direction.South] = true;
        stageLayout[goalPosX, stageLayout.GetLength(1) - 1].route = true;
        // ゴール廊下
        stageLayout[goalPosX, stageLayout.GetLength(1) - 2].type = SectionType.CrossCorridor;
        stageLayout[goalPosX, stageLayout.GetLength(1) - 2].corridorForm = CorridorForm.I;
        stageLayout[goalPosX, stageLayout.GetLength(1) - 2].connect[(int)Direction.North] = true;
        stageLayout[goalPosX, stageLayout.GetLength(1) - 2].connect[(int)Direction.South] = true;
        stageLayout[goalPosX, stageLayout.GetLength(1) - 2].route = true;

        // 部屋の配列を作る
        for (int w = 0; w < stageLayout.GetLength(0); w++)
        {
            for (int h = 0; h < stageLayout.GetLength(1); h++)
            {
                if (stageLayout[w, h].type == SectionType.Room)
                {
                    int[] pos = new int[2];
                    pos[0] = w;
                    pos[1] = h;
                    roomPos.Add(pos);
                }
            }
        }
        // 部屋の配列を作る
        for (int w = 0; w < stageLayout.GetLength(0); w++)
        {
            for (int h = 0; h < stageLayout.GetLength(1); h++)
            {
                if (stageLayout[w, h].type == SectionType.CrossCorridor ||
                    stageLayout[w, h].type == SectionType.OverCorridor)
                {
                    int[] pos = new int[2];
                    pos[0] = w;
                    pos[1] = h;
                    corridorPos.Add(pos);
                }
            }
        }
    }

    // 区画の生成をする関数
    void GenerateSection()
    {
        // 区画を順に生成
        for (int w = 0; w < stageLayout.GetLength(0); w++)
        {
            for (int h = 0; h < stageLayout.GetLength(1); h++)
            {
                if (stageLayout[w, h].type == SectionType.None) continue;

                // 生成するプレハブ
                GameObject genSection = null;
                switch (stageLayout[w, h].type)
                {
                    // 部屋
                    case SectionType.Room:
                        genSection = rooms[stageLayout[w, h].roomForm];
                        break;
                    // スタート部屋
                    case SectionType.StartRoom:
                        genSection = startRoom;
                        break;
                    // ゴール部屋
                    case SectionType.GoalRoom:
                        genSection = goalRoom;
                        break;
                    // 廊下
                    case SectionType.CrossCorridor:
                    case SectionType.OverCorridor:
                        genSection = corridors[(int)stageLayout[w, h].corridorForm];
                        break;
                    // それ以外なら
                    default: break;
                }

                // 座標を指定
                Vector3 genSectionPos = new Vector3(w * SECTION_SIZE, 0, h * SECTION_SIZE);

                // 回転を設定
                Quaternion genSectionRot = Quaternion.Euler(0, 90 * (int)stageLayout[w, h].rotate, 0);

                // 生成
                GameObject section = Instantiate(genSection, genSectionPos, genSectionRot, stage);

                if (stageLayout[w, h].type == SectionType.None ||
                    stageLayout[w, h].type == SectionType.CrossCorridor ||
                    stageLayout[w, h].type == SectionType.OverCorridor)
                    continue;

                for (int i = 0; i < (int)Direction.Max; i++)
                {
                    // オブジェクト設定
                    GameObject genObj;
                    // ドアの設置
                    if (stageLayout[w, h].connect[i] == true)
                        genObj = door;
                    // 壁の設置
                    else
                        genObj = wall;

                    // 座標設定
                    // 場所によるオフセットを指定し部屋の座標と足す
                    Vector3 genPos = new Vector3(w * SECTION_SIZE, 0, h * SECTION_SIZE);
                    Vector3 offset;
                    if (i == (int)Direction.North)
                        offset = new Vector3(0, ROOM_HEIGHT / 2, SECTION_SIZE / 2);
                    else if (i == (int)Direction.East)
                        offset = new Vector3(SECTION_SIZE / 2, ROOM_HEIGHT / 2, 0);
                    else if (i == (int)Direction.South)
                        offset = new Vector3(0, ROOM_HEIGHT / 2, -SECTION_SIZE / 2);
                    else if (i == (int)Direction.West)
                        offset = new Vector3(-SECTION_SIZE / 2, ROOM_HEIGHT / 2, 0);
                    else
                        offset = Vector3.zero;
                    genPos += offset;

                    // 回転設定
                    // 方向によって回転
                    Quaternion genRot = Quaternion.Euler(0, 90 * i, 0);

                    Instantiate(genObj, genPos, genRot, section.transform);
                }
            }
        }
    }

    // スタート地点を返す関数
    public Vector3 GetStartPos()
    {
        return new Vector3(startPosX, 0, 0) * SECTION_SIZE;
    }

    // ゴール地点を返す関数
    public Vector3 GetGoalPos()
    {
        return new Vector3(goalPosX, 0, stageLayout.GetLength(1) - 1) * SECTION_SIZE;
    }

    // ステージ情報を返す関数
    public Section[,] GetStage()
    {
        return stageLayout;
    }

    // 区画の大きさを返す関数
    public float GetSectionSize()
    {
        return SECTION_SIZE;
    }

    // 区画から座標を返す関数
    public Vector3 GetPos(int w, int h)
    {
        float width = w * GetSectionSize();
        float height = h * GetSectionSize();

        return new Vector3(width, 0, height);
    }

    // ランダムな部屋の座標を返す関数
    public Vector3 GetRandRoomPos()
    {
        int rand = UnityEngine.Random.Range(0, roomPos.Count);

        return GetPos(roomPos[rand][0], roomPos[rand][1]);
    }

    // ランダムな部屋の座標を返す関数
    public Vector3 GetRandCorridorPos()
    {
        int rand = UnityEngine.Random.Range(0, corridorPos.Count);

        return GetPos(corridorPos[rand][0], corridorPos[rand][1]);
    }
}
