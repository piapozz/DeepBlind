using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapItem : ItemBase
{
    [SerializeField]
    private Transform _sectionRoot = null;
    [SerializeField]
    private Transform _pointRoot = null;

    [SerializeField] GameObject room = null;
    [SerializeField] GameObject ICorridor = null;
    [SerializeField] GameObject LCorridor = null;
    [SerializeField] GameObject TCorridor = null;
    [SerializeField] GameObject XCorridor = null;
    [SerializeField] GameObject point = null;

    private readonly float EDGE_MARGIN_RATE = 0.1f;        // �]���̔䗦
    private readonly float POINT_SIZE_RATE = 0.05f;        // �ԓ_�̑傫���̔䗦

    private float _edgeMargin = 0;      // �]���̑傫��
    private float _sectionSize = 0;     // ���̑傫��

    private GameObject _pointObj = null;
    private List<GameObject> _mapSectionObject = null;

    public override void Initialize()
    {
        base.Initialize();
        AjustMapSize();
        int sectionCount = StageManager.instance.stageSectionCount;
        _mapSectionObject = new List<GameObject>(sectionCount);
        for (int i = 0; i < sectionCount; i++)
        {
            _mapSectionObject.Add(null);
        }
        GenerateMap();
    }

    public override void Proc()
    {
        // �v���C���[�̍��W���擾
        Vector3 playerPos = Player.instance.GetPosition();
        Vector2Int sectionPos = StageManager.instance.GetSectionPosition(playerPos);
        // �ʉ߂�������\��
        DisplaySection(sectionPos);
        // �ԓ_�̍X�V
        PointProc(playerPos);
    }

    public override bool Effect()
    {
        return false;
    }

    private void AjustMapSize()
    {
        float mapSize;
        Vector2Int stageSize = StageManager.instance.stageSize;
        // �Z���ق��̃T�C�Y�ɍ��킹��
        if (stageSize.x < stageSize.y)
        {
            mapSize = _sectionRoot.localScale.x;
            _edgeMargin = mapSize * EDGE_MARGIN_RATE;
            _sectionSize = (1 - _edgeMargin) / stageSize.y;
        }
        else
        {
            mapSize = _sectionRoot.localScale.y;
            _edgeMargin = mapSize * EDGE_MARGIN_RATE;
            _sectionSize = (1 - _edgeMargin) / stageSize.x;
        }
    }

    // �~�j�}�b�v�𐶐�����֐�
    private void GenerateMap()
    {
        // �ԓ_�𐶐�
        Vector3 genPointPos = ChangeWorldToLocalPosition(Player.instance.GetPosition(), _pointRoot);
        Quaternion genPointRot = Quaternion.Euler(_sectionRoot.eulerAngles.x, _sectionRoot.eulerAngles.y, 0);
        _pointObj = Instantiate(point, genPointPos, genPointRot, _pointRoot);
        _pointObj.transform.localScale = new Vector3(POINT_SIZE_RATE, POINT_SIZE_RATE, 1);

        for (int i = 0, max = StageManager.instance.stageSectionCount; i < max; i++)
        {
            SectionData section = StageManager.instance.GetSection(i);
            GameObject sectionObject = DecideSectionObject(section);
            if (sectionObject == null) continue;

            // ���W�w��
            Vector3 genPos = ChangeSectionToLocalPosition(section.position, _sectionRoot);
            // ��]�w��
            Quaternion genRot = Quaternion.Euler(_sectionRoot.eulerAngles.x, _sectionRoot.eulerAngles.y, -90 * (int)section.direction);
            // ����
            GameObject genObj = Instantiate(sectionObject, genPos, genRot, _sectionRoot);
            genObj.transform.localScale = new Vector3(_sectionSize, _sectionSize, 1);
            genObj.SetActive(false);
            _mapSectionObject[i] = genObj;
        }
    }

    /// <summary>
    /// ���̃I�u�W�F�N�g���擾
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    private GameObject DecideSectionObject(SectionData section)
    {
        GameObject sectionObject = null;
        if (section.IsRoom())
        {
            sectionObject = room;
        }
        else if (section.IsCorridor())
        {
            switch (section.corridorType)
            {
                case CorridorType.I:
                    sectionObject = ICorridor;
                    break;
                case CorridorType.L:
                    sectionObject = LCorridor;
                    break;
                case CorridorType.T:
                    sectionObject = TCorridor;
                    break;
                case CorridorType.X:
                    sectionObject = XCorridor;
                    break;
            }
        }
        return sectionObject;
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

    /// <summary>
    /// ���ɓ������Ƃ��ɕ\������
    /// </summary>
    /// <param name="pos"></param>
    private void DisplaySection(Vector2Int pos)
    {
        // ID�擾
        int sectionID = StageManager.instance.GetSectionID(pos);
        if (_mapSectionObject[sectionID] == null) return;

        // �\������
        if (_mapSectionObject[sectionID].activeSelf == false)
            _mapSectionObject[sectionID].SetActive(true);
    }

    // ������ꏊ��ԓ_�ŕ\������֐�
    private void PointProc(Vector3 pos)
    {
        // �v���C���[�̍��W���}�b�v��̍��W�ɕϊ�
        Vector3 pointPos = ChangeWorldToLocalPosition(pos, _pointRoot);

        _pointObj.transform.position = pointPos;
    }
}
