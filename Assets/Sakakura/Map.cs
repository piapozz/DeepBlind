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

    private readonly float EDGE_MARGIN_RATE = 0.1f;        // �]���̔䗦
    private readonly float POINT_SIZE_RATE = 0.05f;        // �ԓ_�̑傫���̔䗦

    private float _miniMapSize;     // �~�j�}�b�v�̑傫��
    private float _edgeMargin;      // �]���̑傫��
    private float _sectionSize;     // ���̑傫��

    GenerateStage.Section[,] stageLayout;
    GameObject pointObj;

    GameObject[,] miniMap;          // �~�j�}�b�v�̔z��

    protected override void Init()
    {
        stageLayout = GenerateStage.instance.GetStage();
        // �Z���ق��̃T�C�Y�ɍ��킹��
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
        // �v���C���[�̍��W���擾
        Vector3 playerPos = Player.instance.GetPosition();
        Vector2Int sectionPos = GenerateStage.instance.GetNowSection(playerPos);

        // �ʉ߂�������\��
        DisplaySection(sectionPos);

        // �ԓ_�̍X�V
        PointMiniMap(playerPos);
    }

    // �~�j�}�b�v�𐶐�����֐�
    private void GenerateMap()
    {
        // �ԓ_�𐶐�
        Quaternion genPointRot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        pointObj = Instantiate(point, ChangeSectionToLocalPosition(Player.instance.GetNowSection(), _pointRoot), genPointRot, _pointRoot);
        pointObj.transform.localScale = new Vector3(POINT_SIZE_RATE, POINT_SIZE_RATE, 1);

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
                Vector3 genPos = ChangeSectionToLocalPosition(new Vector2Int(w, h), _sectionRoot);

                // ��]�w��
                Quaternion genRot = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, -90 * (int)stageLayout[w, h].rotate);
                // ����
                GameObject genObj = Instantiate(genImage, genPos, genRot, _sectionRoot);

                miniMap[w, h] = genObj;

                // �傫��
                genObj.transform.localScale = new Vector3(_sectionSize, _sectionSize, 1);

                // ��\���ɂ���
                genObj.SetActive(false);
            }
        }
    }

    // ���̍��W���烍�[�J���̍��W�ɕϊ�����֐�
    private Vector3 ChangeSectionToLocalPosition(Vector2Int pos, Transform parent)
    {
        // ���W�w��
        Vector3 adjust =
              new Vector3(_sectionSize / 2 + _edgeMargin / 2, _sectionSize / 2 + _edgeMargin / 2, 0)
            - new Vector3(1, 1, 0) / 2;
        Vector3 genPos = parent.TransformPoint(new Vector3(pos.x, pos.y, 0) * _sectionSize + adjust);
        return genPos;
    }

    // ���[���h���W���烍�[�J���̍��W�ɕϊ�����֐�
    private Vector3 ChangeWorldToLocalPosition(Vector3 pos, Transform parent)
    {
        // ���W�w��
        Vector3 adjust =
              new Vector3(_sectionSize / 2 + _edgeMargin / 2, _sectionSize / 2 + _edgeMargin / 2, 0)
            - new Vector3(1, 1, 0) / 2;
        Vector3 genPos = parent.TransformPoint(new Vector3(pos.x, pos.z, 0) * _sectionSize / 10 + adjust);
        return genPos;
    }

    // ���ɓ������Ƃ��ɕ\������֐�
    private void DisplaySection(Vector2Int pos)
    {
        // �z��O�Ȃ�Q�Ƃ��Ȃ�
        if (pos.x < 0 || pos.x > miniMap.GetLength(0) - 1 ||
            pos.y < 0 || pos.y > miniMap.GetLength(1) - 1)
            return;

        if (miniMap[pos.x, pos.y] == null) return;

        if (miniMap[pos.x, pos.y].activeSelf == false)
            miniMap[pos.x, pos.y].SetActive(true);
    }

    // ������ꏊ��ԓ_�ŕ\������֐�
    private void PointMiniMap(Vector3 pos)
    {
        // �v���C���[�̍��W���}�b�v��̍��W�ɕϊ�
        Vector3 pointPos = ChangeWorldToLocalPosition(pos, _pointRoot);

        pointObj.transform.position = pointPos;
    }
}
