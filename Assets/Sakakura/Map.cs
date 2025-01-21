using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : ItemBase
{
    [SerializeField]
    private Transform _sectionRoot;
    [SerializeField]
    private Transform _pointRoot;

    [SerializeField] GameObject room;
    [SerializeField] GameObject ICorridor;
    [SerializeField] GameObject LCorridor;
    [SerializeField] GameObject TCorridor;
    [SerializeField] GameObject XCorridor;
    [SerializeField] GameObject point;

    private readonly float EDGE_MARGIN_RATE = 0.1f;        // 余白の比率
    private readonly float POINT_SIZE_RATE = 0.05f;        // 赤点の大きさの比率

    private float _miniMapSize;     // ミニマップの大きさ
    private float _edgeMargin;      // 余白の大きさ
    private float _sectionSize;     // 区画の大きさ

    GenerateStage.Section[,] stageLayout;
    GameObject pointObj;

    GameObject[,] miniMap;          // ミニマップの配列

    protected override void Init()
    {
        stageLayout = GenerateStage.instance.GetStage();
        // 短いほうのサイズに合わせる
        if (stageLayout.GetLength(0) < stageLayout.GetLength(1))
        {
            _miniMapSize = transform.localScale.x;
            _edgeMargin = _miniMapSize * EDGE_MARGIN_RATE;
            _sectionSize = (1 - _edgeMargin) / stageLayout.GetLength(1);
        }
        else
        {
            _miniMapSize = transform.localScale.y;
            _edgeMargin = _miniMapSize * EDGE_MARGIN_RATE;
            _sectionSize = (1 - _edgeMargin) / stageLayout.GetLength(0);
        }

        miniMap = new GameObject[stageLayout.GetLength(0), stageLayout.GetLength(1)];

        GenerateMap();
    }

    protected override void Proc()
    {
        // プレイヤーの座標を取得
        Vector3 playerPos = Player.instance.GetPosition();
        Vector2Int sectionPos = GenerateStage.instance.GetNowSection(playerPos);

        // 通過した区画を表示
        DisplaySection(sectionPos);

        // 赤点の更新
        PointMiniMap(playerPos);
    }

    // ミニマップを生成する関数
    private void GenerateMap()
    {
        // 赤点を生成
        Quaternion genPointRot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        pointObj = Instantiate(point, ChangeSectionToLocalPosition(Player.instance.GetNowSection(), _pointRoot), genPointRot, _pointRoot);
        pointObj.transform.localScale = new Vector3(POINT_SIZE_RATE, POINT_SIZE_RATE, 1);

        // 順番に生成
        for (int w = 0; w < stageLayout.GetLength(0); w++)
        {
            for (int h = 0; h < stageLayout.GetLength(1); h++)
            {
                // 生成する画像
                GameObject genImage = null;
                // 区画の種類によって分岐
                switch (stageLayout[w, h].type)
                {
                    // 無し
                    case GenerateStage.SectionType.None:
                        continue;
                    // 部屋
                    case GenerateStage.SectionType.Room:
                    case GenerateStage.SectionType.StartRoom:
                    case GenerateStage.SectionType.GoalRoom:
                        genImage = room;
                        break;
                    // 廊下
                    case GenerateStage.SectionType.CrossCorridor:
                    case GenerateStage.SectionType.OverCorridor:
                        // 廊下の形状で分岐
                        switch (stageLayout[w, h].corridorForm)
                        {
                            case GenerateStage.CorridorForm.I:
                                genImage = ICorridor;
                                break;
                            case GenerateStage.CorridorForm.L:
                                genImage = LCorridor;
                                break;
                            case GenerateStage.CorridorForm.T:
                                genImage = TCorridor;
                                break;
                            case GenerateStage.CorridorForm.X:
                                genImage = XCorridor;
                                break;
                            default: break;
                        }
                        break;
                    default: break;
                }

                // 座標指定
                Vector3 genPos = ChangeSectionToLocalPosition(new Vector2Int(w, h), _sectionRoot);

                // 回転指定
                Quaternion genRot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -90 * (int)stageLayout[w, h].rotate);
                // 生成
                GameObject genObj = Instantiate(genImage, genPos, genRot, _sectionRoot);

                miniMap[w, h] = genObj;

                // 大きさ
                genObj.transform.localScale = new Vector3(_sectionSize, _sectionSize, 1);

                // 非表示にする
                genObj.SetActive(false);
            }
        }
    }

    // 区画の座標からローカルの座標に変換する関数
    private Vector3 ChangeSectionToLocalPosition(Vector2Int pos, Transform parent)
    {
        // 座標指定
        Vector3 adjust =
              new Vector3(_sectionSize / 2 + _edgeMargin / 2, _sectionSize / 2 + _edgeMargin / 2, 0)
            - new Vector3(1, 1, 0) / 2;
        Vector3 genPos = parent.TransformPoint(new Vector3(pos.x, pos.y, 0) * _sectionSize + adjust);
        return genPos;
    }

    // ワールド座標からローカルの座標に変換する関数
    private Vector3 ChangeWorldToLocalPosition(Vector3 pos, Transform parent)
    {
        // 座標指定
        Vector3 adjust =
              new Vector3(_sectionSize / 2 + _edgeMargin / 2, _sectionSize / 2 + _edgeMargin / 2, 0)
            - new Vector3(1, 1, 0) / 2;
        Vector3 genPos = parent.TransformPoint(new Vector3(pos.x, pos.z, 0) * _sectionSize / 10 + adjust);
        return genPos;
    }

    // 区画に入ったときに表示する関数
    private void DisplaySection(Vector2Int pos)
    {
        // 配列外なら参照しない
        if (pos.x < 0 || pos.x > miniMap.GetLength(0) - 1 ||
            pos.y < 0 || pos.y > miniMap.GetLength(1) - 1)
            return;

        if (miniMap[pos.x, pos.y] == null) return;

        if (miniMap[pos.x, pos.y].activeSelf == false)
            miniMap[pos.x, pos.y].SetActive(true);
    }

    // 今いる場所を赤点で表示する関数
    private void PointMiniMap(Vector3 pos)
    {
        // プレイヤーの座標をマップ上の座標に変換
        Vector3 pointPos = ChangeWorldToLocalPosition(pos, _pointRoot);

        pointObj.transform.position = pointPos;
    }
}
