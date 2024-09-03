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

    float miniMapSize;                  // �~�j�}�b�v�̑傫��
    float edgeMargin;                   // �]���̑傫��
    float sectionSize;                  // ���̑傫��
    float edgeMarginRate = 0.1f;        // �]���̔䗦

    GenerateStage.Section[,] stageLayout;

    GameObject[,] miniMap;              // �~�j�}�b�v�̔z��

    RectTransform rectTransform;        // �p�l����Transform

    void Start()
    {
        stageLayout = generateStage.GetStage();
        rectTransform = GetComponent<RectTransform>();
        // �Z���ق��̃T�C�Y�ɍ��킹��
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
        // �ʉ߂�������\��
        DisplaySection(player.GetNowSection());

        // ���������F�t��
        ColorMiniMap(player.GetNowSection());
    }

    // �~�j�}�b�v�𐶐�����֐�
    void GenerateMap()
    {
        // ���Ԃɐ���
        for (int w = 0; w < stageLayout.GetLength(0); w++)
        {
            for (int h = 0; h < stageLayout.GetLength(1); h++)
            {
                // ��������摜
                GameObject genImage = null;
                // ���̎�ނɂ���ĕ���
                switch (stageLayout[w, h].type)
                {
                    // ����
                    case GenerateStage.SectionType.None:
                        continue;
                    // ����
                    case GenerateStage.SectionType.Room:
                    case GenerateStage.SectionType.StartRoom:
                    case GenerateStage.SectionType.GoalRoom:
                        genImage = room;
                        break;
                    // �L��
                    case GenerateStage.SectionType.CrossCorridor:
                    case GenerateStage.SectionType.OverCorridor:
                        // �L���̌`��ŕ���
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

                // ��]�w��
                Quaternion genRot = Quaternion.Euler(0, 0, -90 * (int)stageLayout[w, h].rotate);

                // ����
                GameObject genObj = Instantiate(genImage, Vector2.zero, genRot, gameObject.transform);

                miniMap[w, h] = genObj;

                // �����㒲��
                RectTransform genRect = genObj.GetComponent<RectTransform>();
                // ���W
                Vector2 adjust = 
                    new Vector2(sectionSize / 2 + edgeMargin / 2, sectionSize / 2 + edgeMargin / 2)
                    - rectTransform.sizeDelta / 2;
                Vector2 genPos =
                    new Vector2(w * sectionSize, h * sectionSize) + adjust;
                genRect.anchoredPosition = genPos;
                // �傫��
                genRect.sizeDelta = Vector2.one * sectionSize;

                // ��\���ɂ���
                genObj.SetActive(false);
            }
        }
    }

    // ���ɓ������Ƃ��ɕ\������֐�
    void DisplaySection(Vector2Int pos)
    {
        // �z��O�Ȃ�Q�Ƃ��Ȃ�
        if (pos.x < 0 || pos.x > miniMap.GetLength(0) - 1 ||
            pos.y < 0 || pos.y > miniMap.GetLength(1) - 1)
            return;

        if (miniMap[pos.x, pos.y].activeSelf == false)
            miniMap[pos.x, pos.y].SetActive(true);
    }

    // ������ꏊ�ɐF��t����֐�
    void ColorMiniMap(Vector2Int pos)
    {
        // �z��O�Ȃ�Q�Ƃ��Ȃ�
        if (pos.x < 0 || pos.x > miniMap.GetLength(0) - 1 ||
            pos.y < 0 || pos.y > miniMap.GetLength(1) - 1)
            return;

        // ���ׂĂ̋�������
        for (int w = 0; w < miniMap.GetLength(0); w++)
        {
            for (int h = 0; h < miniMap.GetLength(1); h++)
            {
                Color color;

                // ��������Ȃ�
                if (w == pos.x && h == pos.y)
                    color = Color.green;
                else
                    color = Color.white;

                // �~�j�}�b�v������Ȃ�
                if (miniMap[w, h] != null)
                    // �F��ς���
                    miniMap[w, h].GetComponent<Image>().color = color;
            }
        }
    }
}
