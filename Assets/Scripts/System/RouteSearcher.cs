/*
* @file RouteSearch.cs
* @brief �o�H�T��
* @author sakakura
* @date 2025/3/14
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class RouteSearcher
{
    /// <summary>
    /// �m�[�h
    /// </summary>
    private class Node
    {
        public int distance { get; private set; } = -1;
        public int cost { get; private set; } = -1;
        public int ID { get; private set; } = -1;
        public Direction direction { get; private set; } = Direction.Invalid;
        public Node prevNode { get; private set; } = null;

        public Node(int setDistance, int setCost, int setSectionID, Direction setDir, Node setPrevNode)
        {
            distance = setDistance;
            cost = setCost;
            ID = setSectionID;
            direction = setDir;
            prevNode = setPrevNode;
        }

        /// <summary>
        /// �S�[������̋������X�R�A�Ƃ��ĕԂ�
        /// </summary>
        /// <param name="goalX"></param>
        /// <param name="goalY"></param>
        /// <returns></returns>
        public int GetScore(Vector2Int goalPosition)
        {
            Section section = StageManager.instance.GetSection(ID);
            Vector2Int position = section.position;
            return cost + position.Distance(goalPosition);
        }
    }

    /// <summary>
    /// �m�[�h�e�[�u��
    /// </summary>
    private class NodeTable
    {
        public Node goalNode = null;
        public List<Node> openNodeList = null;
        public List<Node> closeNodeList = null;
        public NodeTable()
        {
            int sectionCount = StageManager.instance.stageSectionCount;
            closeNodeList = new List<Node>(sectionCount);
        }

        public void Clear()
        {
            goalNode = null;
            openNodeList.Clear();
            closeNodeList.Clear();
        }
    }

    private static NodeTable _nodeTable = null;

    /// <summary>��̎��X�R�A</summary>
    private const int _DEFAULT_ACTUAL_SCORE = 2;
    /// <summary>�ڑ��̎��X�R�A</summary>
    private const int _CONNECT_ACTUAL_SCORE = 1;

    /// <summary>
    /// �S�[���܂ł̃��[�g���擾
    /// </summary>
    /// <param name="startSectionID"></param>
    /// <param name="goalSectionID"></param>
    /// <param name="beforeConnectDir"></param>
    /// <returns></returns>
    public static List<MoveData> RouteSearch(int startSectionID, int goalSectionID, Direction beforeConnectDir)
    {
        // �S�[���m�[�h�ɂ��ǂ蒅���܂Ńm�[�h���J��
        OpenNodeToGoal(startSectionID, goalSectionID, beforeConnectDir);
        // �o�H�쐬
        return CreateRoute();
    }

    /// <summary>
    /// �S�[���܂Ńm�[�h���J��
    /// </summary>
    /// <param name="startSectionID"></param>
    /// <param name="goalSectionID"></param>
    private static void OpenNodeToGoal(int startSectionID, int goalSectionID, Direction beforeConnectDir)
    {
        InitializeNodeTable();
        // �X�^�[�g���I�[�v�����X�g�ɓ����
        Node startNode = new Node(0, 0, startSectionID, beforeConnectDir, null);
        _nodeTable.openNodeList.Add(startNode);

        // �O��̕������������͂̃m�[�h���I�[�v������
        // �X�^�[�g�������N���[�Y
        int directionMax = (int)Direction.Max;
        List<Direction> connectDirection = new List<Direction>(directionMax);
        for (int i = 0; i < directionMax; i++)
        {
            Direction direction = (Direction)i;
            if (direction == beforeConnectDir) continue;
            connectDirection.Add(direction);
        }
        OpenNodeDirection(startNode, goalSectionID, connectDirection);
        _nodeTable.openNodeList.Remove(startNode);
        _nodeTable.closeNodeList.Add(startNode);

        // �S�[�����W���擾
        Section goalSection = StageManager.instance.GetSection(goalSectionID);
        Vector2Int goalPosition = goalSection.position;
        // �S�[���m�[�h�ɂ��ǂ蒅���܂ŏ���
        int procCount = 0;
        while (_nodeTable.goalNode == null)
        {
            if (procCount > 10000) break;
            procCount++;
            // �m�[�h���J������
            OpenNodeProcess(goalPosition, goalSectionID);
        }
    }

    /// <summary>
    /// �m�[�h�e�[�u���̏�����
    /// </summary>
    private static void InitializeNodeTable()
    {
        if (_nodeTable == null)
            _nodeTable = new NodeTable();
        else
            _nodeTable.Clear();
        InitializeList(ref _nodeTable.openNodeList, StageManager.instance.stageSectionCount);
    }

    /// <summary>
    /// �m�[�h���J����A�̏���
    /// </summary>
    /// <param name="goalPosition"></param>
    /// <param name="goalSectionID"></param>
    private static void OpenNodeProcess(Vector2Int goalPosition, int goalSectionID)
    {
        // ��m�[�h���擾
        List<Node> minNodeList = GetMinScoreNode(goalPosition);
        if (minNodeList == null) return;
        Node baseNode = GetConnectNode(minNodeList);
        // �Ȃ����Ă���m�[�h���Ȃ��Ȃ�
        if (baseNode == null)
        {
            // �����_���ȃm�[�h���擾
            int randomIndex = Random.Range(0, minNodeList.Count);
            baseNode = minNodeList[randomIndex];
            // ��m�[�h�̎��͂��I�[�v������
            OpenNodeAround(baseNode, goalSectionID);
        }
        else
        {
            // �ڑ��������擾���I�[�v��
            List<Direction> connectDirection = StageManager.instance.GetConnectSection(baseNode.ID);
            OpenNodeDirection(baseNode, goalSectionID, connectDirection);
        }
    }

    /// <summary>
    /// �J���Ă�m�[�h����ŏ��̃X�R�A�̃m�[�h���擾
    /// </summary>
    /// <param name="goalPosition"></param>
    private static List<Node> GetMinScoreNode(Vector2Int goalPosition)
    {
        if (IsEmpty(_nodeTable.openNodeList)) return null;

        int openListCount = _nodeTable.openNodeList.Count;
        List<Node> minNodeList = new List<Node>(openListCount);
        int minScore = int .MaxValue;
        // �ŏ��X�R�A�Ȃ烊�X�g�ɒǉ�
        for (int i = 0; i < openListCount; i++)
        {
            Node node = _nodeTable.openNodeList[i];
            if (node == null) continue;

            int score = node.GetScore(goalPosition);
            if (score <= minScore)
            {
                minNodeList.Add(node);
                minScore = score;
            }
        }
        return minNodeList;
    }

    /// <summary>
    /// �Ȃ����Ă���m�[�h�����X�g����擾
    /// </summary>
    /// <param name="minScoreNodeList"></param>
    /// <returns></returns>
    private static Node GetConnectNode(List<Node> minScoreNodeList)
    {
        int nodeCount = minScoreNodeList.Count;
        List<Node> connectNodeList = new List<Node>(nodeCount);
        // �m�[�h���Ȃ����Ă��邩�I�[�v������Ă��Ȃ��Ȃ烊�X�g�ɒǉ�
        for (int i = 0; i < nodeCount; i++)
        {
            List<Direction> connectDirection = StageManager.instance.GetConnectSection(minScoreNodeList[i].ID);
            if (connectDirection.Count == 0 ||
                _nodeTable.openNodeList.Exists(node => node.ID == minScoreNodeList[i].ID)) continue;
            connectNodeList.Add(minScoreNodeList[i]);
        }

        if (IsEmpty(connectNodeList)) return null;

        // �����_���ȃm�[�h���擾
        int randomIndex = Random.Range(0, connectNodeList.Count);
        return connectNodeList[randomIndex];
    }

    /// <summary>
    /// ��m�[�h�̎w��̕����̋����I�[�v������
    /// </summary>
    /// <param name="baseNode"></param>
    /// <param name="direction"></param>
    private static void OpenNodeDirection(Node baseNode, int goalSectionID, List<Direction> directionList)
    {
        if (baseNode == null) return;

        Section baseSection = StageManager.instance.GetSection(baseNode.ID);
        for (int i = 0, max = directionList.Count; i < max; i++)
        {
            // �I�[�v������m�[�h�̎擾
            Section openSection = StageManager.instance.GetSectionDir(baseSection, directionList[i]);
            if (openSection == null) continue;

            // �I�[�v���ł��Ȃ��Ȃ�X�L�b�v
            if (!CanOpenNode(openSection.ID)) continue;

            int distance = baseNode.distance + 1;
            int cost = baseNode.cost + _CONNECT_ACTUAL_SCORE;
            Node addNode = new Node(distance, cost, openSection.ID, directionList[i], baseNode);
            // �I�[�v�������m�[�h�Ƃ��Ēǉ�
            _nodeTable.openNodeList.Add(addNode);

            // �S�[������
            if (openSection.ID != goalSectionID) continue;
            _nodeTable.goalNode = addNode;
            return;
        }

        // FIX:�ꕔ�����I�[�v�����Ȃ��Ƃ��̃m�[�h���I�΂ꑱ����
        // ���͂��ׂĂ��I�[�v�����Ă�����N���[�Y
        if (IsAroundOpen(baseNode.ID))
        {
            _nodeTable.openNodeList.Remove(baseNode);
            _nodeTable.closeNodeList.Add(baseNode);
        }
    }

    /// <summary>
    /// ��m�[�h�̎��͂̋����I�[�v������
    /// </summary>
    /// <param name="baseNode"></param>
    /// <param name="goalSectionID"></param>
    private static void OpenNodeAround(Node baseNode, int goalSectionID)
    {
        if (baseNode == null) return;

        Section baseSection = StageManager.instance.GetSection(baseNode.ID);
        for (int i = 0, max = (int)Direction.Max; i < max; i++)
        {
            Direction direction = (Direction)i;
            // �I�[�v������m�[�h�̎擾
            Section openSection = StageManager.instance.GetSectionDir(baseSection, direction);
            if (openSection == null) continue;

            // �I�[�v���ł��Ȃ��Ȃ�X�L�b�v
            if (!CanOpenNode(openSection.ID)) continue;

            int distance = baseNode.distance + 1;
            int cost = baseNode.cost + _DEFAULT_ACTUAL_SCORE;
            Node addNode = new Node(distance, cost, openSection.ID, direction, baseNode);
            // �I�[�v�������m�[�h�Ƃ��Ēǉ�
            _nodeTable.openNodeList.Add(addNode);

            // �S�[������
            if (openSection.ID != goalSectionID) continue;
            _nodeTable.goalNode = addNode;
            return;
        }

        // ��m�[�h���N���[�Y
        _nodeTable.openNodeList.Remove(baseNode);
        _nodeTable.closeNodeList.Add(baseNode);
    }

    /// <summary>
    /// �I�[�v���ł���m�[�h������
    /// </summary>
    /// <param name="openID"></param>
    /// <returns></returns>
    private static bool CanOpenNode(int openID)
    {
        // ���łɃI�[�v���������Ƃ̂���m�[�h�̓X�L�b�v
        if (_nodeTable.openNodeList.Exists(node => node.ID == openID)) return false;
        // ���ɃN���[�Y���Ă���m�[�h�̓X�L�b�v
        if (_nodeTable.closeNodeList.Exists(node => node.ID == openID)) return false;

        return true;
    }

    /// <summary>
    /// ���͂��I�[�v������Ă��邩
    /// </summary>
    /// <returns></returns>
    private static bool IsAroundOpen(int baseID)
    {
        for (int i = 0, max = (int)Direction.Max; i < max; i++)
        {
            Section nextSection = StageManager.instance.GetSectionDir(baseID, (Direction)i);
            if (nextSection == null) continue;
            int aroundID = nextSection.ID;
            if (!_nodeTable.openNodeList.Exists(node => node.ID == aroundID)) return false;
        }
        return true;
    }

    /// <summary>
    /// �o�H���쐬����
    /// </summary>
    /// <returns></returns>
    private static List<MoveData> CreateRoute()
    {
        if (_nodeTable == null || _nodeTable.goalNode == null) return null;

        int routeCount = _nodeTable.goalNode.distance;
        List<MoveData> route = new List<MoveData>(routeCount);
        for (int i = 0; i < routeCount; i++)
        {
            route.Add(null);
        }

        Node currentNode = _nodeTable.goalNode;
        // �S�[������k���Đ���
        for (int i = routeCount - 1; i >= 0; i--)
        {
            if (currentNode.prevNode == null) break;
            MoveData movedata = new MoveData(currentNode.prevNode.ID, currentNode.ID, currentNode.direction);
            route[i] = movedata;
            currentNode = currentNode.prevNode;
        }
        return route;
    }
}
