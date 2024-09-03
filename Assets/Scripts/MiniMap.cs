using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    [SerializeField] GameObject room;
    [SerializeField] GameObject ICorridor;
    [SerializeField] GameObject LCorridor;
    [SerializeField] GameObject TCorridor;
    [SerializeField] GameObject XCorridor;
    [SerializeField] Player player;

    [SerializeField] GenerateStage generateStage;

    float miniMapSize;                  // ミニマップの大きさ
    float edgeMargin;                   // 余白の大きさ
    float sectionSize;                  // 区画の大きさ
    float edgeMarginRate = 0.1f;        // 余白の比率

    GenerateStage.Section[,] stageLayout;

    GameObject[,] miniMap;              // ミニマップの配列

    RectTransform rectTransform;        // パネルのTransform

    void Start()
    {
        stageLayout = generateStage.GetStage();
        rectTransform = GetComponent<RectTransform>();
        // 短いほうのサイズに合わせる
        if (stageLayout.GetLength(0) < stageLayout.GetLength(1))
        {
            miniMapSize = rectTransform.sizeDelta.x;
            edgeMargin = miniMapSize * edgeMarginRate;
            sectionSize = (miniMapSize - edgeMargin) / stageLayout.GetLength(1);
        }
        else
        {
            miniMapSize = rectTransform.sizeDelta.y;
            edgeMargin = miniMapSize * edgeMarginRate;
            sectionSize = (miniMapSize - edgeMargin) / stageLayout.GetLength(0);
        }

        miniMap = new GameObject[stageLayout.GetLength(0), stageLayout.GetLength(1)];

        GenerateMap();
    }

    void Update()
    {
        // 通過した区画を表示
        DisplaySection(player.GetNowSection());

        // 今いる区画を色付け
        ColorMiniMap(player.GetNowSection());
    }

    // ミニマップを生成する関数
    void GenerateMap()
    {
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

                // 回転指定
                Quaternion genRot = Quaternion.Euler(0, 0, -90 * (int)stageLayout[w, h].rotate);

                // 生成
                GameObject genObj = Instantiate(genImage, Vector2.zero, genRot, gameObject.transform);

                miniMap[w, h] = genObj;

                // 生成後調整
                RectTransform genRect = genObj.GetComponent<RectTransform>();
                // 座標
                Vector2 adjust = 
                    new Vector2(sectionSize / 2 + edgeMargin / 2, sectionSize / 2 + edgeMargin / 2)
                    - rectTransform.sizeDelta / 2;
                Vector2 genPos =
                    new Vector2(w * sectionSize, h * sectionSize) + adjust;
                genRect.anchoredPosition = genPos;
                // 大きさ
                genRect.sizeDelta = Vector2.one * sectionSize;

                // 非表示にする
                genObj.SetActive(false);
            }
        }
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
}
