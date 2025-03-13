using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public static GenerateStage instance { get; private set; } = null;

    [SerializeField]
    private SectionObjectAssign sectionObjectAssign = null;

    private List<Section> _sectionList = null;

    private Entity_StageData.Param _stageMaster = null;

    public void Initialize()
    {
        _stageMaster = StageMasterUtility.GetStageMaster();
        int sectionCount = _stageMaster.widthSize * _stageMaster.heightSize;
        _sectionList = new List<Section>(sectionCount);
        for (int i = 0; i < sectionCount; i++)
        {
            Section createSection = new Section();
            Vector2Int position = GetSquarePosition(i);
            createSection.Initialize(i, position, Section.SectionType.Invalid);
            _sectionList.Add(createSection);
        }

        CreateStage();

        GenerateStage();
    }

    /// <summary>
	/// ID��2�������W�ɕϊ�
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public Vector2Int GetSquarePosition(int ID)
    {
        Vector2Int result = new Vector2Int();
        result.x = ID % _stageMaster.widthSize;
        result.y = ID / _stageMaster.widthSize;
        return result;
    }

    private void CreateStage()
    {
        // �����̈ʒu����
        DecideRoomPos(_stageMaster.roomCount);
        // �����ƕ������Ȃ�

    }


    private void DecideRoomPos(int roomPos)
    {
        // �X�^�[�g��������

        for (int i = 0; i < roomPos; i++)
        {
            
        }
    }

    private void GenerateStage()
    {

    }
}
