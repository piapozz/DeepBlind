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
    private class DistanceNode
    {
        public int distance { get; private set; } = -1;
        public int ID { get; private set; } = -1;
        public Direction direction { get; private set; } = Direction.Invalid;
        public DistanceNode prevNode { get; private set; } = null;

        public DistanceNode(int setDistance, int setSquareID, Direction setDir, DistanceNode setPrevNode)
        {
            distance = setDistance;
            ID = setSquareID;
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
            int diffX = Mathf.Abs(section.position.x - goalPosition.x);
            int diffY = Mathf.Abs(section.position.y - goalPosition.y);
            return diffX + diffY;
        }
    }

    /// <summary>
    /// �m�[�h�e�[�u��
    /// </summary>
    private class DistanceNodeTable
    {
        public DistanceNode goalNode = null;
        public List<DistanceNode> nodeList = null;
        public DistanceNodeTable()
        {
            int sectionCount = StageManager.instance.stageSectionCount;
            nodeList = new List<DistanceNode>(sectionCount);
        }

        public void Clear()
        {
            goalNode = null;
            nodeList.Clear();
        }
    }

    private static DistanceNodeTable _nodeTable = null;
    private static List<DistanceNode> _openList = null;

    public static List<MoveData> RouteSearch(int startSectionID, int goalSectionID)
    {
        // �S�[���m�[�h�ɂ��ǂ蒅���܂Ńm�[�h���J��
        OpenNodeToGoal(startSectionID, goalSectionID);
        // �o�H�쐬
        return null;
    }

    /// <summary>
    /// �S�[���܂Ńm�[�h���J��
    /// </summary>
    /// <param name="startSectionID"></param>
    /// <param name="goalSectionID"></param>
    private static void OpenNodeToGoal(int startSectionID, int goalSectionID)
    {
        InitializeNodeTable();
        // �X�^�[�g���I�[�v�����X�g�ɓ����
        _openList.Add(new DistanceNode(0, startSectionID, Direction.Invalid, null));
        // �S�[�����W���擾
        Section goalSection = StageManager.instance.GetSection(goalSectionID);
        Vector2Int goalPosition = goalSection.position;
        while (_nodeTable.goalNode == null)
        {
            // �X�R�A�ŏ��̒������m�[�h���擾
            DistanceNode baseNode = GetMinScoreNode(goalPosition);
            // ��m�[�h�̎��͂��I�[�v������
            
        }
    }

    /// <summary>
    /// �m�[�h�e�[�u���̏�����
    /// </summary>
    private static void InitializeNodeTable()
    {
        if (_nodeTable == null)
            _nodeTable = new DistanceNodeTable();
        else
            _nodeTable.Clear();
        InitializeList(ref _openList, StageManager.instance.stageSectionCount);
    }

    /// <summary>
    /// �J���Ă�m�[�h����ŏ��̃X�R�A�̃m�[�h���擾
    /// </summary>
    /// <param name="goalPosition"></param>
    private static DistanceNode GetMinScoreNode(Vector2Int goalPosition)
    {
        if (IsEmpty(_openList)) return null;

        int openListCount = _openList.Count;
        List<DistanceNode> minNode = new List<DistanceNode>(openListCount);
        int minScore = int .MaxValue;
        for (int i = 0; i < openListCount; i++)
        {
            DistanceNode node = _openList[i];
            if (node == null) continue;

            // �ŏ��X�R�A�Ȃ烊�X�g�ɒǉ�
            int score = node.GetScore(goalPosition);
            if (score <= minScore)
            {
                minNode.Add(node);
                minScore = score;
            }
        }
        // �ŏ��X�R�A�̃m�[�h���烉���_���Ŏ擾
        int randomIndex = Random.Range(0, minNode.Count);
        return minNode[randomIndex];
    }

    /// <summary>
    /// ��m�[�h�̎��͂̋����I�[�v������
    /// </summary>
    /// <param name="baseNode"></param>
    /// <param name="goalSectionID"></param>
    private static void OpenNodeAround(DistanceNode baseNode, int goalSectionID)
    {
        if (baseNode == null) return;

        Section baseSection = StageManager.instance.GetSection(baseNode.ID);
        for (int i = 0, max = (int)Direction.Max; i < max; i++)
        {
            Direction direction = (Direction)i;
            // �I�[�v������m�[�h�̎擾
            Section openSection = StageManager.instance.GetSectionDir(baseSection, direction);
            if (openSection == null) continue;

            // ���łɃI�[�v���������Ƃ̂���m�[�h�̓X�L�b�v
            if (_nodeTable.nodeList.Exists(node => node.ID == openSection.ID)) continue;

            int distance = baseNode.distance + 1;
            DistanceNode addNode = new DistanceNode(distance, openSection.ID, direction, baseNode);
            // �I�[�v�������m�[�h�Ƃ��Ēǉ�
            _nodeTable.nodeList.Add(addNode);
            _openList.Add(addNode);

            // �S�[������
            if (openSection.ID != goalSectionID) continue;
            _nodeTable.goalNode = addNode;
            return;
        }
    }
}
