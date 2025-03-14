/*
* @file RouteSearch.cs
* @brief 経路探索
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
    /// ノード
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
        /// ゴールからの距離をスコアとして返す
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
    /// ノードテーブル
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
        // ゴールノードにたどり着くまでノードを開く
        OpenNodeToGoal(startSectionID, goalSectionID);
        // 経路作成
        return null;
    }

    /// <summary>
    /// ゴールまでノードを開く
    /// </summary>
    /// <param name="startSectionID"></param>
    /// <param name="goalSectionID"></param>
    private static void OpenNodeToGoal(int startSectionID, int goalSectionID)
    {
        InitializeNodeTable();
        // スタートをオープンリストに入れる
        _openList.Add(new DistanceNode(0, startSectionID, Direction.Invalid, null));
        // ゴール座標を取得
        Section goalSection = StageManager.instance.GetSection(goalSectionID);
        Vector2Int goalPosition = goalSection.position;
        while (_nodeTable.goalNode == null)
        {
            // スコア最小の中から基準ノードを取得
            DistanceNode baseNode = GetMinScoreNode(goalPosition);
            // 基準ノードの周囲をオープンする
            
        }
    }

    /// <summary>
    /// ノードテーブルの初期化
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
    /// 開いてるノードから最小のスコアのノードを取得
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

            // 最少スコアならリストに追加
            int score = node.GetScore(goalPosition);
            if (score <= minScore)
            {
                minNode.Add(node);
                minScore = score;
            }
        }
        // 最少スコアのノードからランダムで取得
        int randomIndex = Random.Range(0, minNode.Count);
        return minNode[randomIndex];
    }

    /// <summary>
    /// 基準ノードの周囲の区画をオープンする
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
            // オープンするノードの取得
            Section openSection = StageManager.instance.GetSectionDir(baseSection, direction);
            if (openSection == null) continue;

            // すでにオープンしたことのあるノードはスキップ
            if (_nodeTable.nodeList.Exists(node => node.ID == openSection.ID)) continue;

            int distance = baseNode.distance + 1;
            DistanceNode addNode = new DistanceNode(distance, openSection.ID, direction, baseNode);
            // オープンしたノードとして追加
            _nodeTable.nodeList.Add(addNode);
            _openList.Add(addNode);

            // ゴール判定
            if (openSection.ID != goalSectionID) continue;
            _nodeTable.goalNode = addNode;
            return;
        }
    }
}
