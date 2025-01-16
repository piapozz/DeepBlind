using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    [SerializeField] GameObject room;
    [SerializeField] GameObject ICorridor;
    [SerializeField] GameObject LCorridor;
    [SerializeField] GameObject TCorridor;
    [SerializeField] GameObject XCorridor;
    [SerializeField] GameObject point;
    [SerializeField] Player player;

    [SerializeField] GenerateStage generateStage;

    float miniMapSize;                  // ミニマップの大きさ
    float edgeMargin;                   // 余白の大きさ
    float sectionSize;                  // 区画の大きさ
    float edgeMarginRate = 0.1f;        // 余白の比率
    float pointSizeRate = 0.05f;        // 赤点の大きさの比率

    GenerateStage.Section[,] stageLayout;
    GameObject pointObj;

    GameObject[,] miniMap;              // ミニマップの配列

    [SerializeField]
    private Transform _cameraTransform;

    private readonly float _ITEM_DISTANCE = 0.5f;
    private readonly float _ITEM_HEIGHT = 0.5f;

    void Start()
    {
        _cameraTransform = Camera.main.transform;

        stageLayout = generateStage.GetStage();
        // 短いほうのサイズに合わせる
        if (stageLayout.GetLength(0) < stageLayout.GetLength(1))
        {
            miniMapSize = transform.localScale.x;
            edgeMargin = miniMapSize * edgeMarginRate;
            sectionSize = (1 - edgeMargin) / stageLayout.GetLength(1);
        }
        else
        {
            miniMapSize = transform.localScale.y;
            edgeMargin = miniMapSize * edgeMarginRate;
            sectionSize = (1 - edgeMargin) / stageLayout.GetLength(0);
        }

        miniMap = new GameObject[stageLayout.GetLength(0), stageLayout.GetLength(1)];

        GenerateMap();
    }

    void Update()
    {
        // プレイヤーの座標を取得
        Vector2Int sectionPos = generateStage.GetNowSection(player.GetPosition());

        // 通過した区画を表示
        DisplaySection(sectionPos);

        // 赤点の更新
        PointMiniMap(sectionPos);

        // 今いる区画を色付け
        //ColorMiniMap(sectionPos);

        FollowCamera();
    }

    private void FollowCamera()
    {
        float angle = _cameraTransform.localEulerAngles.y;
        Vector3 offset = Vector3.zero;
        offset.x = Mathf.Sin(angle * Mathf.Deg2Rad) * _ITEM_DISTANCE;
        offset.y = -_ITEM_HEIGHT;
        offset.z = Mathf.Cos(angle * Mathf.Deg2Rad) * _ITEM_DISTANCE;
        transform.position = _cameraTransform.position + offset;
        transform.localEulerAngles = new Vector3(0, angle, 0);
    }

    // ミニマップを生成する関数
    void GenerateMap()
    {
        // 赤点を生成
        /*
        pointObj = Instantiate(point, ChangeLocalPosition(player.GetNowSection()), Quaternion.identity, gameObject.transform);
        pointObj.transform.localScale = new Vector3(pointSizeRate, pointSizeRate, 1);
        */

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
                Vector3 genPos = ChangeLocalPosition(new Vector2Int(w, h));

                // 回転指定
                //Quaternion genRot = Quaternion.Euler(0, 0, -90 * (int)stageLayout[w, h].rotate);
                Quaternion genRot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -90 * (int)stageLayout[w, h].rotate);
                // 生成
                GameObject genObj = Instantiate(genImage, genPos, genRot, gameObject.transform);

                miniMap[w, h] = genObj;

                // 大きさ
                genObj.transform.localScale = new Vector3(sectionSize, sectionSize, 1);

                // 非表示にする
                genObj.SetActive(false);
            }
        }
    }

    // 区画の座標からローカルの座標に変換する関数
    Vector3 ChangeLocalPosition(Vector2Int pos)
    {
        // 座標指定
        Vector3 adjust =
              new Vector3(sectionSize / 2 + edgeMargin / 2, sectionSize / 2 + edgeMargin / 2, -0.01f)
            - new Vector3(1, 1, 0) / 2;
        Vector3 genPos = transform.TransformPoint(new Vector3(pos.x, pos.y, 0) * sectionSize + adjust);
        return genPos;
    }

    // 区画に入ったときに表示する関数
    void DisplaySection(Vector2Int pos)
    {
        // 配列外なら参照しない
        if (pos.x < 0 || pos.x > miniMap.GetLength(0) - 1 ||
            pos.y < 0 || pos.y > miniMap.GetLength(1) - 1)
            return;

        if (miniMap[pos.x, pos.y].activeSelf == false)
            miniMap[pos.x, pos.y].SetActive(true);
    }

    // 今いる場所に色を付ける関数
    void ColorMiniMap(Vector2Int pos)
    {
        // 配列外なら参照しない
        if (pos.x < 0 || pos.x > miniMap.GetLength(0) - 1 ||
            pos.y < 0 || pos.y > miniMap.GetLength(1) - 1)
            return;

        // すべての区画を見る
        for (int w = 0; w < miniMap.GetLength(0); w++)
        {
            for (int h = 0; h < miniMap.GetLength(1); h++)
            {
                Color color;

                // 今いる区画なら
                if (w == pos.x && h == pos.y)
                    color = Color.green;
                else
                    color = Color.white;

                // ミニマップがあるなら
                if (miniMap[w, h] != null)
                    // 色を変える
                    miniMap[w, h].GetComponent<Image>().color = color;
            }
        }
    }

    // 今いる場所を赤点で表示する関数
    void PointMiniMap(Vector2Int pos)
    {
        // プレイヤーの座標をマップ上の座標に変換
        Vector3 pointPos = ChangeLocalPosition(pos);

        //pointObj.transform.position = pointPos;
    }
}
