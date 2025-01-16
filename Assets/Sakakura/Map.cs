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

    float miniMapSize;                  // �~�j�}�b�v�̑傫��
    float edgeMargin;                   // �]���̑傫��
    float sectionSize;                  // ���̑傫��
    float edgeMarginRate = 0.1f;        // �]���̔䗦
    float pointSizeRate = 0.05f;        // �ԓ_�̑傫���̔䗦

    GenerateStage.Section[,] stageLayout;
    GameObject pointObj;

    GameObject[,] miniMap;              // �~�j�}�b�v�̔z��

    [SerializeField]
    private Transform _cameraTransform;

    private readonly float _ITEM_DISTANCE = 0.5f;
    private readonly float _ITEM_HEIGHT = 0.5f;

    void Start()
    {
        _cameraTransform = Camera.main.transform;

        stageLayout = generateStage.GetStage();
        // �Z���ق��̃T�C�Y�ɍ��킹��
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
        // �v���C���[�̍��W���擾
        Vector2Int sectionPos = generateStage.GetNowSection(player.GetPosition());

        // �ʉ߂�������\��
        DisplaySection(sectionPos);

        // �ԓ_�̍X�V
        PointMiniMap(sectionPos);

        // ���������F�t��
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

    // �~�j�}�b�v�𐶐�����֐�
    void GenerateMap()
    {
        // �ԓ_�𐶐�
        /*
        pointObj = Instantiate(point, ChangeLocalPosition(player.GetNowSection()), Quaternion.identity, gameObject.transform);
        pointObj.transform.localScale = new Vector3(pointSizeRate, pointSizeRate, 1);
        */

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

                // ���W�w��
                Vector3 genPos = ChangeLocalPosition(new Vector2Int(w, h));

                // ��]�w��
                //Quaternion genRot = Quaternion.Euler(0, 0, -90 * (int)stageLayout[w, h].rotate);
                Quaternion genRot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -90 * (int)stageLayout[w, h].rotate);
                // ����
                GameObject genObj = Instantiate(genImage, genPos, genRot, gameObject.transform);

                miniMap[w, h] = genObj;

                // �傫��
                genObj.transform.localScale = new Vector3(sectionSize, sectionSize, 1);

                // ��\���ɂ���
                genObj.SetActive(false);
            }
        }
    }

    // ���̍��W���烍�[�J���̍��W�ɕϊ�����֐�
    Vector3 ChangeLocalPosition(Vector2Int pos)
    {
        // ���W�w��
        Vector3 adjust =
              new Vector3(sectionSize / 2 + edgeMargin / 2, sectionSize / 2 + edgeMargin / 2, -0.01f)
            - new Vector3(1, 1, 0) / 2;
        Vector3 genPos = transform.TransformPoint(new Vector3(pos.x, pos.y, 0) * sectionSize + adjust);
        return genPos;
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

    // ������ꏊ��ԓ_�ŕ\������֐�
    void PointMiniMap(Vector2Int pos)
    {
        // �v���C���[�̍��W���}�b�v��̍��W�ɕϊ�
        Vector3 pointPos = ChangeLocalPosition(pos);

        //pointObj.transform.position = pointPos;
    }
}
