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
        /// ゴールからの距離をスコアとして返す
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
    /// ノードテーブル
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

    /// <summary>基準の実スコア</summary>
    private const int _DEFAULT_ACTUAL_SCORE = 2;
    /// <summary>接続の実スコア</summary>
    private const int _CONNECT_ACTUAL_SCORE = 1;

    /// <summary>
    /// ゴールまでのルートを取得
    /// </summary>
    /// <param name="startSectionID"></param>
    /// <param name="goalSectionID"></param>
    /// <param name="beforeConnectDir"></param>
    /// <returns></returns>
    public static List<MoveData> RouteSearch(int startSectionID, int goalSectionID, Direction beforeConnectDir)
    {
        // ゴールノードにたどり着くまでノードを開く
        OpenNodeToGoal(startSectionID, goalSectionID, beforeConnectDir);
        // 経路作成
        return CreateRoute();
    }

    /// <summary>
    /// ゴールまでノードを開く
    /// </summary>
    /// <param name="startSectionID"></param>
    /// <param name="goalSectionID"></param>
    private static void OpenNodeToGoal(int startSectionID, int goalSectionID, Direction beforeConnectDir)
    {
        InitializeNodeTable();
        // スタートをオープンリストに入れる
        Node startNode = new Node(0, 0, startSectionID, beforeConnectDir, null);
        _nodeTable.openNodeList.Add(startNode);

        // 前回の方向を除く周囲のノードをオープンして
        // スタート部屋をクローズ
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

        // ゴール座標を取得
        Section goalSection = StageManager.instance.GetSection(goalSectionID);
        Vector2Int goalPosition = goalSection.position;
        // ゴールノードにたどり着くまで処理
        int procCount = 0;
        while (_nodeTable.goalNode == null)
        {
            if (procCount > 10000) break;
            procCount++;
            // ノードを開く処理
            OpenNodeProcess(goalPosition, goalSectionID);
        }
    }

    /// <summary>
    /// ノードテーブルの初期化
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
    /// ノードを開く一連の処理
    /// </summary>
    /// <param name="goalPosition"></param>
    /// <param name="goalSectionID"></param>
    private static void OpenNodeProcess(Vector2Int goalPosition, int goalSectionID)
    {
        // 基準ノードを取得
        List<Node> minNodeList = GetMinScoreNode(goalPosition);
        if (minNodeList == null) return;
        Node baseNode = GetConnectNode(minNodeList);
        // つながっているノードがないなら
        if (baseNode == null)
        {
            // ランダムなノードを取得
            int randomIndex = Random.Range(0, minNodeList.Count);
            baseNode = minNodeList[randomIndex];
            // 基準ノードの周囲をオープンする
            OpenNodeAround(baseNode, goalSectionID);
        }
        else
        {
            // 接続方向を取得しオープン
            List<Direction> connectDirection = StageManager.instance.GetConnectSection(baseNode.ID);
            OpenNodeDirection(baseNode, goalSectionID, connectDirection);
        }
    }

    /// <summary>
    /// 開いてるノードから最小のスコアのノードを取得
    /// </summary>
    /// <param name="goalPosition"></param>
    private static List<Node> GetMinScoreNode(Vector2Int goalPosition)
    {
        if (IsEmpty(_nodeTable.openNodeList)) return null;

        int openListCount = _nodeTable.openNodeList.Count;
        List<Node> minNodeList = new List<Node>(openListCount);
        int minScore = int .MaxValue;
        // 最少スコアならリストに追加
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
    /// つながっているノードをリストから取得
    /// </summary>
    /// <param name="minScoreNodeList"></param>
    /// <returns></returns>
    private static Node GetConnectNode(List<Node> minScoreNodeList)
    {
        int nodeCount = minScoreNodeList.Count;
        List<Node> connectNodeList = new List<Node>(nodeCount);
        // ノードがつながっているかつオープンされていないならリストに追加
        for (int i = 0; i < nodeCount; i++)
        {
            List<Direction> connectDirection = StageManager.instance.GetConnectSection(minScoreNodeList[i].ID);
            if (connectDirection.Count == 0 ||
                _nodeTable.openNodeList.Exists(node => node.ID == minScoreNodeList[i].ID)) continue;
            connectNodeList.Add(minScoreNodeList[i]);
        }

        if (IsEmpty(connectNodeList)) return null;

        // ランダムなノードを取得
        int randomIndex = Random.Range(0, connectNodeList.Count);
        return connectNodeList[randomIndex];
    }

    /// <summary>
    /// 基準ノードの指定の方向の区画をオープンする
    /// </summary>
    /// <param name="baseNode"></param>
    /// <param name="direction"></param>
    private static void OpenNodeDirection(Node baseNode, int goalSectionID, List<Direction> directionList)
    {
        if (baseNode == null) return;

        Section baseSection = StageManager.instance.GetSection(baseNode.ID);
        for (int i = 0, max = directionList.Count; i < max; i++)
        {
            // オープンするノードの取得
            Section openSection = StageManager.instance.GetSectionDir(baseSection, directionList[i]);
            if (openSection == null) continue;

            // オープンできないならスキップ
            if (!CanOpenNode(openSection.ID)) continue;

            int distance = baseNode.distance + 1;
            int cost = baseNode.cost + _CONNECT_ACTUAL_SCORE;
            Node addNode = new Node(distance, cost, openSection.ID, directionList[i], baseNode);
            // オープンしたノードとして追加
            _nodeTable.openNodeList.Add(addNode);

            // ゴール判定
            if (openSection.ID != goalSectionID) continue;
            _nodeTable.goalNode = addNode;
            return;
        }

        // FIX:一部しかオープンしないとこのノードが選ばれ続ける
        // 周囲すべてがオープンしていたらクローズ
        if (IsAroundOpen(baseNode.ID))
        {
            _nodeTable.openNodeList.Remove(baseNode);
            _nodeTable.closeNodeList.Add(baseNode);
        }
    }

    /// <summary>
    /// 基準ノードの周囲の区画をオープンする
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
            // オープンするノードの取得
            Section openSection = StageManager.instance.GetSectionDir(baseSection, direction);
            if (openSection == null) continue;

            // オープンできないならスキップ
            if (!CanOpenNode(openSection.ID)) continue;

            int distance = baseNode.distance + 1;
            int cost = baseNode.cost + _DEFAULT_ACTUAL_SCORE;
            Node addNode = new Node(distance, cost, openSection.ID, direction, baseNode);
            // オープンしたノードとして追加
            _nodeTable.openNodeList.Add(addNode);

            // ゴール判定
            if (openSection.ID != goalSectionID) continue;
            _nodeTable.goalNode = addNode;
            return;
        }

        // 基準ノードをクローズ
        _nodeTable.openNodeList.Remove(baseNode);
        _nodeTable.closeNodeList.Add(baseNode);
    }

    /// <summary>
    /// オープンできるノードか判定
    /// </summary>
    /// <param name="openID"></param>
    /// <returns></returns>
    private static bool CanOpenNode(int openID)
    {
        // すでにオープンしたことのあるノードはスキップ
        if (_nodeTable.openNodeList.Exists(node => node.ID == openID)) return false;
        // 既にクローズしているノードはスキップ
        if (_nodeTable.closeNodeList.Exists(node => node.ID == openID)) return false;

        return true;
    }

    /// <summary>
    /// 周囲がオープンされているか
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
    /// 経路を作成する
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
        // ゴールから遡って生成
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
