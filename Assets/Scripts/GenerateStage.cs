using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GenerateStage : MonoBehaviour
{
    // �X�e�[�W���i�[�����I�u�W�F�N�g
    [SerializeField] Transform stage;
    // �X�e�[�W�̕��i�̃v���n�u
    [SerializeField] GameObject[] rooms;
    [SerializeField] GameObject startRoom;
    [SerializeField] GameObject goalRoom;
    [SerializeField] GameObject ICorridor;
    [SerializeField] GameObject LCorridor;
    [SerializeField] GameObject TCorridor;
    [SerializeField] GameObject XCorridor;
    [SerializeField] GameObject door;
    [SerializeField] GameObject wall;

    const int WIDTH_ROOM_MAX = 6;               // ���̕����̐�
    const int HEIGHT_ROOM_MAX = 4;              // �c�̕����̐�
    const float SECTION_SIZE = 10;              // ���̃T�C�Y
    const float ROOM_HEIGHT = 3;                // �����̍���
    const int GENERATE_ATTEMPT_MAX = 100;       // �������s�񐔂̍ő�l
    const float CONECT_PROBABILITY = 0.4f;      // ��悪�Ȃ���\��

    int startPosX;       // �X�^�[�g�̈ʒu
    int goalPosX;        // �S�[���̈ʒu

    GameObject[] corridors;     // �L���̃v���n�u

    List<int[]> roomPos = new List<int[]>();      // �����̔z��
    List<int[]> corridorPos = new List<int[]>();  // �L���̔z��

    // ���p
    public enum Direction
    {
        North = 0,
        East,
        South,
        West,
        Max
    }

    // ���̎��
    public enum SectionType
    {
        None = 0,
        Room,           // ����
        StartRoom,      // �X�^�[�g����
        GoalRoom,       // �S�[������
        OverCorridor,   // �����ƕ������Ȃ��L��(�n���L��)
        CrossCorridor,  // �L���ƘL�����Ȃ��L��(�����L��)
        Max
    }

    // �L���̎��
    public enum CorridorForm
    {
        I = 0,
        L,
        T,
        X,
        Max
    }

    // �X�e�[�W�̍\����
    public struct Section
    {
        public SectionType type;            // ���
        public Direction rotate;            // ��]
        public CorridorForm corridorForm;   // �L���̌`
        public int roomForm;                // �����̌`
        public bool[] connect;              // ������k�ɂȂ����Ă��邩
        public bool route;                  // �S�[���܂ł̃��[�g��

        // �ڑ����̏�����
        public void Init()
        {
            connect = new bool[(int)Direction.Max];
            // �ʂ��ŏ�����
            for(int i = 0; i < (int)Direction.Max; i++)
            {
                connect[i] = false;
            }
        }
    }
    Section[,] creatLayout;     // �X�e�[�W�̍\���̂̓񎟌��z��
    Section[,] stageLayout;     // ���������X�e�[�W�̍\����

    // �S�[���܂ł̃��[�g���L�^����\����
    struct Route
    {
        public bool throuth;
        public bool[] selected;

        // �ڑ����̏�����
        public void Init()
        {
            selected = new bool[(int)Direction.Max];
            // �ʂ��ŏ�����
            for (int i = 0; i < (int)Direction.Max; i++)
            {
                selected[i] = false;
            }
            throuth = false;
        }
    }
    Route[,] route;         // ���[�g�\�z�Ɏg���\���̂̓񎟌��z��
    Route[,] judgeConnect;  // �X�^�[�g����Ȃ����Ă��邩���m���߂�̂Ɏg��

    void Awake()
    {
        // ������
        Init();

        // �X�e�[�W�̐݌v
        StageLayout();

        // ���̐���
        GenerateSection();
    }

    // ������
    void Init()
    {
        // �X�e�[�W�̓񎟌��z��
        creatLayout = new Section[WIDTH_ROOM_MAX * 2 - 1, HEIGHT_ROOM_MAX * 2 - 1];

        // �X�e�[�W�̓񎟌��z��̏�����
        for (int w = 0; w < creatLayout.GetLength(0); w++)
        {
            for (int h = 0; h < creatLayout.GetLength(1); h++)
            {
                // �\���̓��̐ڑ����z��̏�����
                creatLayout[w, h].Init();

                creatLayout[w, h].type = SectionType.None;

                // ����
                if (w % 2 == 0 && h % 2 == 0) 
                    creatLayout[w, h].type = SectionType.Room;
                // �����L��
                else if (w % 2 != 0 && h % 2 != 0)
                    creatLayout[w, h].type = SectionType.CrossCorridor;
                // �n���L��
                else
                    creatLayout[w, h].type = SectionType.OverCorridor;
            }
        }

        // �L���̃v���n�u�̔z��
        corridors = new GameObject[(int)CorridorForm.Max];
        corridors[0] = ICorridor;
        corridors[1] = LCorridor;
        corridors[2] = TCorridor;
        corridors[3] = XCorridor;
    }

    // �X�e�[�W�̐݌v������֐�
    void StageLayout()
    {
        // �S�[���܂ł̃��[�g��S��
        DicisionRoute();

        // �e�}�X���ǂ��ɂȂ����Ă��邩�����߂�
        DicisionSectionConnect();

        // �Ȃ����Ă��Ȃ�����������
        DeleteDisconnectedSection();

        // ���������肷��
        DicisionRoom();

        // �L�������肷��
        JudgeCorridorForm();

        // �X�^�[�g�ƃS�[���n�_���܂߂��z��ɍX�V
        UpdateStageLayout();
    }

    // �X�^�[�g�ƃS�[�������߂Čo�H�����߂�֐�
    // �X�^�[�g����i�߂��悪�S�[���ɓ��B�ł��邩�𔻒肵�Č��߂Ă���
    void DicisionRoute()
    {
        // �X�^�[�g�ƃS�[���̈ʒu�����߂�
        int startRand = UnityEngine.Random.Range(0, WIDTH_ROOM_MAX);
        int goalRand = UnityEngine.Random.Range(0, WIDTH_ROOM_MAX);
        startPosX = startRand * 2;
        goalPosX = goalRand * 2;

        // ���̍��W
        Vector2Int pos;
        // �ړ��������L�^����ϒ��z��
        List<Direction> routeDir = new List<Direction>();
        // ���[�v�񐔂̃J�E���g
        int loopCount;
        // �����t���O
        bool finish = false;

        // �I������܂Ń��[�v
        while (true)
        {
            // ���̍��W��������
            pos = new Vector2Int(startPosX, 0);

            // ���[�g����������
            routeDir.Clear();

            // �ʍs�\�ȕ����̃��X�g
            List<Direction> passableDir = new List<Direction>();

            // ���[�v�񐔂̃J�E���g
            loopCount = 0;

            // ���[�g�̍\���̂̏�����
            route = new Route[creatLayout.GetLength(0), creatLayout.GetLength(1)];
            // �X�e�[�W�̓񎟌��z��̏�����
            for (int w = 0; w < route.GetLength(0); w++)
            {
                for (int h = 0; h < route.GetLength(1); h++)
                {
                    // �\���̓��̐ڑ����z��̏�����
                    route[w, h].Init();

                    // �z��O�Ȃ�I�΂�Ȃ��悤�ɂ���
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

            // �S�[���܂ł̓����o��܂Ń��[�v
            while (true)
            {
                loopCount++;
                // �ړ������̌��̏�����
                passableDir.Clear();

                // �i�߂�����𔻒肷��
                // �e�������܂��I�����Ă��Ȃ��Ēʍs���Ă��Ȃ��Ȃ���ɓ����
                // �k
                if (route[pos.x, pos.y].selected[(int)Direction.North] == false)
                    if (route[pos.x, pos.y + 1].throuth == false)
                        passableDir.Add(Direction.North);
                // ��
                if (route[pos.x, pos.y].selected[(int)Direction.East] == false)
                    if (route[pos.x + 1, pos.y].throuth == false)
                        passableDir.Add(Direction.East);
                // ��
                if (route[pos.x, pos.y].selected[(int)Direction.South] == false)
                    if (route[pos.x, pos.y - 1].throuth == false)
                        passableDir.Add(Direction.South);
                // ��
                if (route[pos.x, pos.y].selected[(int)Direction.West] == false)
                    if (route[pos.x - 1, pos.y].throuth == false)
                        passableDir.Add(Direction.West);

                // �S�[���ɂ��ǂ蒅������I��
                if (pos.x == goalPosX && pos.y == route.GetLength(1) - 1)
                {
                    finish = true;
                    break;
                }

                // �i�s�\�łȂ��Ȃ�
                if (passableDir.Count == 0)
                {
                    // ������}�X��������
                    route[pos.x, pos.y].Init();
                    // �z��O�Ȃ�I�΂�Ȃ��悤�ɂ���
                    if (pos.y == route.GetLength(1) - 1)
                        route[pos.x, pos.y].selected[(int)Direction.North] = true;
                    if (pos.x == route.GetLength(0) - 1)
                        route[pos.x, pos.y].selected[(int)Direction.East] = true;
                    if (pos.y == 0)
                        route[pos.x, pos.y].selected[(int)Direction.South] = true;
                    if (pos.x == 0)
                        route[pos.x, pos.y].selected[(int)Direction.West] = true;

                    // �����ɂ���č��W�̕ύX
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

                    // �߂�
                    routeDir.RemoveAt(routeDir.Count - 1);
                }
                // �i�s�\�Ȃ�
                else
                {
                    // �i�s�����̌�₩�璊�I
                    Direction dir = passableDir[UnityEngine.Random.Range(0, passableDir.Count)];

                    // �ʂ������Ƃ��L�^
                    route[pos.x, pos.y].throuth = true;
                    // ���ɑI���ς݂ɕۑ�
                    route[pos.x, pos.y].selected[(int)dir] = true;

                    // �i�s���������肷��
                    routeDir.Add(dir);

                    // �����ɂ���č��W�̕ύX
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

                // ���[�v�������Ă���΂�蒼��
                if (loopCount >= GENERATE_ATTEMPT_MAX)
                    break;
            }

            Debug.Log("���s��" + loopCount);
            // �������Ă�ΏI��
            if (finish == true)
            {
                Debug.Log("���[�g�\�z����");
                break;
            }
            else
            {
                Debug.Log("���[�g�\�z�����I��");
                continue;
            }
        }

        // ���[�g�̐ڑ�
        // �X�^�[�g�n�_
        pos.x = startPosX;
        pos.y = 0;
        // ���[�g���L�^
        creatLayout[pos.x, pos.y].route = true;
        // �������Ȃ�
        creatLayout[pos.x, pos.y].connect[(int)Direction.South] = true;
        for (int i = 0; i < routeDir.Count; i++)
        {
            // �e�����̐ڑ���ύX���A���W�ύX
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

            // ���[�g���L�^
            creatLayout[pos.x, pos.y].route = true;
        }
        // �o�����Ȃ�
        creatLayout[pos.x, pos.y].connect[(int)Direction.North] = true;
    }

    // ��悪�ǂ��ɂȂ����Ă��邩���߂�֐�
    void DicisionSectionConnect()
    {
        // ����ԗ�
        for (int w = 0; w < creatLayout.GetLength(0); w++)
        {
            for (int h = 0; h < creatLayout.GetLength(1); h++)
            {
                // ��̕ӂłȂ��Ȃ����R�s�[
                if (h > 0)
                    creatLayout[w, h].connect[(int)Direction.South] =
                        creatLayout[w, h - 1].connect[(int)Direction.North];
                // ���̕ӂłȂ��Ȃ琼���R�s�[
                if (w > 0)
                    creatLayout[w, h].connect[(int)Direction.West] =
                        creatLayout[w - 1, h].connect[(int)Direction.East];

                // �ڑ���
                int conectNum = 0;
                // �ڑ����Ă���Ȃ�ڑ������J�E���g
                if (creatLayout[w, h].connect[(int)Direction.North] == true)
                    conectNum++;
                if (creatLayout[w, h].connect[(int)Direction.East] == true)
                    conectNum++;
                if (creatLayout[w, h].connect[(int)Direction.South] == true)
                    conectNum++;
                if (creatLayout[w, h].connect[(int)Direction.West] == true)
                    conectNum++;

                // �X�e�[�W�̕ӂłȂ��ĂȂ����Ă��Ȃ��Ƃ�������ɓ���Ă���
                List<Direction> dir = new List<Direction>();
                // �k�̕ӂłȂ��Ȃ����Ă��Ȃ��Ȃ�
                if (h < creatLayout.GetLength(1) - 1 &&
                    creatLayout[w, h].connect[(int)Direction.North] == false)
                    dir.Add(Direction.North);
                // ���̕ӂłȂ��Ȃ����Ă��Ȃ��Ȃ�
                if (w < creatLayout.GetLength(0) - 1 &&
                    creatLayout[w, h].connect[(int)Direction.East] == false)
                    dir.Add(Direction.East);

                float randConect;

                // �Ȃ����Ă��鐔�ɂ���ĕ���
                switch (conectNum)
                {
                    // �ǂ��ɂ��Ȃ����Ă��Ȃ��ꍇ
                    case 0:
                        // �Q�����ɂȂ����Ȃ�
                        if (dir.Count == 2)
                        {
                            // �m���łȂ��邩�����߂�
                            randConect = UnityEngine.Random.value;
                            if (randConect < CONECT_PROBABILITY)
                            {
                                creatLayout[w, h].connect[(int)dir[0]] = true;
                                creatLayout[w, h].connect[(int)dir[1]] = true;
                            }
                        }
                        break;
                    // �P���������Ȃ����Ă���ꍇ
                    case 1:
                        // �P�����ɂȂ����Ȃ�
                        if (dir.Count == 1)
                            // �Ȃ���
                            creatLayout[w, h].connect[(int)dir[0]] = true;

                        // �Q�����ɂȂ����Ȃ�
                        if (dir.Count == 2)
                        {
                            // �P�����͂Ȃ��ŕЕ��͊m��
                            int randDir = UnityEngine.Random.Range(0, dir.Count);
                            creatLayout[w, h].connect[(int)dir[randDir]] = true;
                            dir.RemoveAt(randDir);

                            randConect = UnityEngine.Random.value;
                            if (randConect < CONECT_PROBABILITY)
                                creatLayout[w, h].connect[(int)dir[0]] = true;
                        }
                        break;
                    // �Q�����Ȃ����Ă���ꍇ
                    case 2:
                        // �m���łȂ���
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
                    // �R�����Ȃ����Ă���ꍇ
                    case 3:
                        // �m���łȂ���
                        randConect = UnityEngine.Random.value;
                        if (randConect < CONECT_PROBABILITY && dir.Count != 0)
                            creatLayout[w, h].connect[(int)dir[0]] = true;
                        break;
                    default: break;
                }
            }
        }
    }

    // �Ȃ����Ă��Ȃ����������֐�
    void DeleteDisconnectedSection()
    {
        // �\���̂̏�����
        judgeConnect = new Route[creatLayout.GetLength(0), creatLayout.GetLength(1)];
        // �X�e�[�W�̓񎟌��z��̏�����
        for (int w = 0; w < judgeConnect.GetLength(0); w++)
        {
            for (int h = 0; h < judgeConnect.GetLength(1); h++)
            {
                // �\���̓��̐ڑ����z��̏�����
                judgeConnect[w, h].Init();

                // �z��O�Ȃ�I�΂�Ȃ��悤�ɂ���
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

        // ���̍��W��������
        // ���̍��W
        Vector2Int pos = new Vector2Int(startPosX, 0);
        // �ʍs�\�ȕ����̃��X�g
        List<Direction> passableDir = new List<Direction>();
        // �ړ��������L�^����ϒ��z��
        List<Direction> routeDir = new List<Direction>();
        // �X�^�[�g�n�_��ʂ������Ƃɂ���
        judgeConnect[pos.x, pos.y].throuth = true;

        // ���ׂĂ̐ڑ�����ʂ�I����܂Ń��[�v
        while (true)
        {
            // �ړ������̌��̏�����
            passableDir.Clear();
            // �i�߂�����𔻒肷��
            // �e�������܂��I�����Ă��Ȃ��Ēʍs���Ă��Ȃ��Ȃ���ɓ����
            // �k
            if (creatLayout[pos.x, pos.y].connect[(int)Direction.North] == true &&
                judgeConnect[pos.x, pos.y].selected[(int)Direction.North] == false)
                passableDir.Add(Direction.North);
            // ��
            if (creatLayout[pos.x, pos.y].connect[(int)Direction.East] == true &&
                judgeConnect[pos.x, pos.y].selected[(int)Direction.East] == false)
                passableDir.Add(Direction.East);
            // ��
            if (creatLayout[pos.x, pos.y].connect[(int)Direction.South] == true &&
                judgeConnect[pos.x, pos.y].selected[(int)Direction.South] == false)
                passableDir.Add(Direction.South);
            // ��
            if (creatLayout[pos.x, pos.y].connect[(int)Direction.West] == true &&
                judgeConnect[pos.x, pos.y].selected[(int)Direction.West] == false)
                passableDir.Add(Direction.West);

            // �i�s�\�łȂ��Ȃ�
            if (passableDir.Count == 0)
            {
                // ���ׂĂ̐ڑ�����ʂ�I������I��
                if (routeDir.Count == 0) break;

                // �����ɂ���č��W�̕ύX
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

                // �߂�
                routeDir.RemoveAt(routeDir.Count - 1);
            }
            // �i�s�\�Ȃ�
            else
            {
                // �i�s�����̌�₩�璊�I
                Direction dir = passableDir[UnityEngine.Random.Range(0, passableDir.Count)];

                // �ʂ������Ƃ��L�^
                judgeConnect[pos.x, pos.y].selected[(int)dir] = true;

                // �i�s�������L�^
                routeDir.Add(dir);

                // �����ɂ���č��W�̕ύX�Ɛڑ��̍X�V
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

                // �ʂ������Ƃ��L�^
                judgeConnect[pos.x, pos.y].throuth = true;
            }
        }

        // �ʉ߂��Ă��Ȃ���������
        for (int w = 0; w < judgeConnect.GetLength(0); w++)
        {
            for (int h = 0; h < judgeConnect.GetLength(1); h++)
            {
                if (judgeConnect[w, h].throuth == false)
                    creatLayout[w, h].type = SectionType.None;
            }
        }
    }

    // ���������肷��֐�
    void DicisionRoom()
    {
        for (int w = 0; w < creatLayout.GetLength(0); w++)
        {
            for (int h = 0; h < creatLayout.GetLength(1); h++)
            {
                // ����
                if (creatLayout[w, h].type == SectionType.Room)
                {
                    // �ڑ���
                    int conectNum = 0;
                    if (creatLayout[w, h].connect[(int)Direction.North] == true)
                        conectNum++;
                    if (creatLayout[w, h].connect[(int)Direction.East] == true)
                        conectNum++;
                    if (creatLayout[w, h].connect[(int)Direction.South] == true)
                        conectNum++;
                    if (creatLayout[w, h].connect[(int)Direction.West] == true)
                        conectNum++;

                    // �Ȃ����Ă��Ȃ��Ȃ�
                    if (conectNum == 0)
                        // ����������
                        creatLayout[w, h].type = SectionType.None;

                    // �����_���ȕ����𒊑I
                    creatLayout[w, h].roomForm = UnityEngine.Random.Range(0, rooms.Length);
                    // �����_���ȉ�]
                    creatLayout[w, h].rotate = (Direction)UnityEngine.Random.Range(0, (int)Direction.Max);
                }
            }
        }
    }

    // �L���̎�ނ����肷��֐�
    void JudgeCorridorForm()
    {
        // �L���̎�ނ����肷��
        for (int w = 0; w < creatLayout.GetLength(0); w++)
        {
            for (int h = 0; h < creatLayout.GetLength(1); h++)
            {
                // �L���łȂ��Ȃ玟��
                if (creatLayout[w, h].type != SectionType.OverCorridor &&
                    creatLayout[w, h].type != SectionType.CrossCorridor)
                    continue;

                // �ڑ���
                int conectNum = 0;
                if (creatLayout[w, h].connect[(int)Direction.North] == true)
                    conectNum++;
                if (creatLayout[w, h].connect[(int)Direction.East] == true)
                    conectNum++;
                if (creatLayout[w, h].connect[(int)Direction.South] == true)
                    conectNum++;
                if (creatLayout[w, h].connect[(int)Direction.West] == true)
                    conectNum++;

                // �Ȃ����Ă��Ȃ��Ȃ�
                if (conectNum == 0 || conectNum == 1)
                    // �L��������
                    creatLayout[w, h].type = SectionType.None;
                // 4�ӂȂ����Ă���Ȃ�X
                else if (conectNum == 4)
                    creatLayout[w, h].corridorForm = CorridorForm.X;
                // 3�ӂȂ����Ă���Ȃ�T
                else if (conectNum == 3)
                {
                    creatLayout[w, h].corridorForm = CorridorForm.T;
                    // �k��
                    if (creatLayout[w, h].connect[(int)Direction.North] == false)
                        creatLayout[w, h].rotate = Direction.North;
                    // ����
                    if (creatLayout[w, h].connect[(int)Direction.East] == false)
                        creatLayout[w, h].rotate = Direction.East;
                    // �쑤
                    if (creatLayout[w, h].connect[(int)Direction.South] == false)
                        creatLayout[w, h].rotate = Direction.South;
                    // ����
                    if (creatLayout[w, h].connect[(int)Direction.West] == false)
                        creatLayout[w, h].rotate = Direction.West;
                }
                // 2�ӂȂ����Ă���
                else
                {
                    // I
                    // �c�����łȂ����Ă���Ȃ�
                    if (creatLayout[w, h].connect[(int)Direction.North] == true &&
                        creatLayout[w, h].connect[(int)Direction.South] == true ||
                        creatLayout[w, h].connect[(int)Direction.East] == true &&
                        creatLayout[w, h].connect[(int)Direction.West] == true)
                    {
                        // ��ސݒ�
                        creatLayout[w, h].corridorForm = CorridorForm.I;
                        // ��]�ݒ�
                        if (creatLayout[w, h].connect[(int)Direction.North] == true)
                            creatLayout[w, h].rotate = Direction.North;
                        else
                            creatLayout[w, h].rotate = Direction.East;
                    }
                    // L
                    else
                    {
                        // ��ސݒ�
                        creatLayout[w, h].corridorForm = CorridorForm.L;
                        // ��]�ݒ�
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

    // �X�^�[�g�ƃS�[���n�_���܂߂��z��ɍX�V����֐�
    void UpdateStageLayout()
    {
        stageLayout = new Section[creatLayout.GetLength(0), creatLayout.GetLength(1) + 4];

        // �z��̏�����
        for (int w = 0; w < stageLayout.GetLength(0); w++)
        {
            for (int h = 0; h < stageLayout.GetLength(1); h++)
            {
                stageLayout[w, h].Init();
            }
        }

        // �R�s�[
        for (int w = 0; w < creatLayout.GetLength(0); w++)
        {
            for (int h = 0; h < creatLayout.GetLength(1); h++)
            {
                stageLayout[w, h + 2] = creatLayout[w, h];
            }
        }

        // �X�^�[�g����
        stageLayout[startPosX, 0].type = SectionType.StartRoom;
        stageLayout[startPosX, 0].connect[(int)Direction.North] = true;
        stageLayout[startPosX, 0].route = true;
        // �X�^�[�g�L��
        stageLayout[startPosX, 1].type = SectionType.CrossCorridor;
        stageLayout[startPosX, 1].corridorForm = CorridorForm.I;
        stageLayout[startPosX, 1].connect[(int)Direction.North] = true;
        stageLayout[startPosX, 1].connect[(int)Direction.South] = true;
        stageLayout[startPosX, 1].route = true;
        // �S�[������
        stageLayout[goalPosX, stageLayout.GetLength(1) - 1].type = SectionType.GoalRoom;
        stageLayout[goalPosX, stageLayout.GetLength(1) - 1].connect[(int)Direction.South] = true;
        stageLayout[goalPosX, stageLayout.GetLength(1) - 1].route = true;
        // �S�[���L��
        stageLayout[goalPosX, stageLayout.GetLength(1) - 2].type = SectionType.CrossCorridor;
        stageLayout[goalPosX, stageLayout.GetLength(1) - 2].corridorForm = CorridorForm.I;
        stageLayout[goalPosX, stageLayout.GetLength(1) - 2].connect[(int)Direction.North] = true;
        stageLayout[goalPosX, stageLayout.GetLength(1) - 2].connect[(int)Direction.South] = true;
        stageLayout[goalPosX, stageLayout.GetLength(1) - 2].route = true;

        // �����̔z������
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
        // �����̔z������
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

    // ���̐���������֐�
    void GenerateSection()
    {
        // �������ɐ���
        for (int w = 0; w < stageLayout.GetLength(0); w++)
        {
            for (int h = 0; h < stageLayout.GetLength(1); h++)
            {
                if (stageLayout[w, h].type == SectionType.None) continue;

                // ��������v���n�u
                GameObject genSection = null;
                switch (stageLayout[w, h].type)
                {
                    // ����
                    case SectionType.Room:
                        genSection = rooms[stageLayout[w, h].roomForm];
                        break;
                    // �X�^�[�g����
                    case SectionType.StartRoom:
                        genSection = startRoom;
                        break;
                    // �S�[������
                    case SectionType.GoalRoom:
                        genSection = goalRoom;
                        break;
                    // �L��
                    case SectionType.CrossCorridor:
                    case SectionType.OverCorridor:
                        genSection = corridors[(int)stageLayout[w, h].corridorForm];
                        break;
                    // ����ȊO�Ȃ�
                    default: break;
                }

                // ���W���w��
                Vector3 genSectionPos = new Vector3(w * SECTION_SIZE, 0, h * SECTION_SIZE);

                // ��]��ݒ�
                Quaternion genSectionRot = Quaternion.Euler(0, 90 * (int)stageLayout[w, h].rotate, 0);

                // ����
                GameObject section = Instantiate(genSection, genSectionPos, genSectionRot, stage);

                if (stageLayout[w, h].type == SectionType.None ||
                    stageLayout[w, h].type == SectionType.CrossCorridor ||
                    stageLayout[w, h].type == SectionType.OverCorridor)
                    continue;

                for (int i = 0; i < (int)Direction.Max; i++)
                {
                    // �I�u�W�F�N�g�ݒ�
                    GameObject genObj;
                    // �h�A�̐ݒu
                    if (stageLayout[w, h].connect[i] == true)
                        genObj = door;
                    // �ǂ̐ݒu
                    else
                        genObj = wall;

                    // ���W�ݒ�
                    // �ꏊ�ɂ��I�t�Z�b�g���w�肵�����̍��W�Ƒ���
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

                    // ��]�ݒ�
                    // �����ɂ���ĉ�]
                    Quaternion genRot = Quaternion.Euler(0, 90 * i, 0);

                    Instantiate(genObj, genPos, genRot, section.transform);
                }
            }
        }
    }

    // �X�^�[�g�n�_��Ԃ��֐�
    public Vector3 GetStartPos()
    {
        return new Vector3(startPosX, 0, 0) * SECTION_SIZE;
    }

    // �S�[���n�_��Ԃ��֐�
    public Vector3 GetGoalPos()
    {
        return new Vector3(goalPosX, 0, stageLayout.GetLength(1) - 1) * SECTION_SIZE;
    }

    // �X�e�[�W����Ԃ��֐�
    public Section[,] GetStage()
    {
        return stageLayout;
    }

    // ���̑傫����Ԃ��֐�
    public float GetSectionSize()
    {
        return SECTION_SIZE;
    }

    // ��悩����W��Ԃ��֐�
    public Vector3 GetPos(int w, int h)
    {
        float width = w * GetSectionSize();
        float height = h * GetSectionSize();

        return new Vector3(width, 0, height);
    }

    // �����_���ȕ����̍��W��Ԃ��֐�
    public Vector3 GetRandRoomPos()
    {
        int rand = UnityEngine.Random.Range(0, roomPos.Count);

        return GetPos(roomPos[rand][0], roomPos[rand][1]);
    }

    // �����_���ȕ����̍��W��Ԃ��֐�
    public Vector3 GetRandCorridorPos()
    {
        int rand = UnityEngine.Random.Range(0, corridorPos.Count);

        return GetPos(corridorPos[rand][0], corridorPos[rand][1]);
    }
}
