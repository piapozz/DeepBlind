using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// �X���[�N
// �h�A�ł�
// ���b�J�[���[�v

// �G�l�~�[�̌��ƂȂ�e�N���X

public abstract class EnemyBase : MonoBehaviour
{
    protected static System.Func<int, GameObject> _GetObject = null;

    public static void SetGetObjectCallback(System.Func<int, GameObject> setCallback)
    {
        _GetObject = setCallback;
    }

    public int ID               { get; protected set; } = -1;
    private int _masterID = -1;
    public Vector3 Position     { get; protected set; } = Vector3.zero;

    public float speed          { get; protected set; } = -1;       // �G�l�~�[�̑���
    public float speedDiameter  { get; protected set; } = -1;       // ���������̑����̔{��
    public float threatRange    { get; protected set; } = -1;       // ���Д͈�
    public float viewLength     { get; protected set; } = -1;       // ���E�̒���
    public float fieldOfView    { get; protected set; } = -1;       // ����p

    // ���ꂼ��̃X�e�[�g�N���X
    protected ISeach seach;
    protected IVigilance vigilance;
    protected ITracking tracking;

    // ���ꂼ��̃X�L���N���X
    protected ISkill skill;

    protected static System.Action<State> _ChangeState = null;

    private IEnemyState _state;

    public virtual void Setup(int setID, Vector3 position, int masterID)
    {
        ID = setID;
        _masterID = masterID;
        _GetObject(ID).transform.position = position;
        _GetObject(ID).SetActive(true);
        ResetStatus();
        _ChangeState = StateChange;
    }

    /// <summary>
    /// �X�e�[�^�X������
    /// </summary>
    public virtual void ResetStatus()
    {
        //var characterMaster = CharacterMasterUtility.GetCharacterMaster(_masterID);
        //if (characterMaster == null) return;

        //SetSpeed(characterMaster.Speed);
        //SetSpeedDiameter(characterMaster.SpeedDiameter);
        //SetThreatRange(characterMaster.ThreatRange);
        //SetViewLength(characterMaster.ViewLength);
        //SetFieldOfView(characterMaster.FieldOfView);
        //SetSeach(characterMaster.Seach);
        //SetVigilance(characterMaster.Vigilance);
        //SetTracking(characterMaster.Tracking);
        //SetSkill(characterMaster.Skill);
    }

    public void Teardown()
    {
        _GetObject(ID).SetActive(false);
        ID = -1;
    }

    public void SetSpeed(float setSpeed)
    {
        speed = setSpeed;
    }

    public void SetSpeedDiameter(float setSpeedDiameter)
    {
        speedDiameter = setSpeedDiameter;
    }

    public void SetThreatRange(float setThreatRange)
    {
        threatRange = setThreatRange;
    }

    public void SetViewLength(float setViewLength)
    {
        viewLength = setViewLength;
    }

    public void SetFieldOfView(float setFieldOfView)
    {
        fieldOfView = setFieldOfView;
    }

    public void SetSeach(ISeach setSeach)
    {
        seach = setSeach;
    }

    public void SetVigilance(IVigilance setVigilance)
    {
        vigilance = setVigilance;
    }

    public void SetTracking(ITracking setTracking)
    {
        tracking = setTracking;
    }

    public void SetSkill(ISkill setSkill)
    {
        skill = setSkill;
    }

    // �X�e�[�g
    public enum State
    {
        SEARCH,        // �T��
        VIGILANCE,     // �x��
        TRACKING,      // �ǐ�

        MAX
    }

    /// <summary>
    /// �i�r���b�V���ňړ�����
    /// </summary>
    public void MoveNavAgent()
    {
        // �ڕW�ʒu��ݒ�
        //_enemyAgent.SetDestination(myInfo.status.targetPos);
    }

    public void Active()
    {
        _state.Activity();

        skill.Ability();
    }

    /// <summary>
    /// �X�e�[�g�ƃX�L���̐؂�ւ�����
    /// </summary>
    /// <param name="state"></param>
    public void StateChange(State state)
    {
        // �X�e�[�g�A�X�L����؂�ւ���
        switch (state)
        {
            case State.SEARCH:

                _state = seach;
                break;

            case State.VIGILANCE:

                _state = vigilance;
                break;

            case State.TRACKING:

                _state = tracking;
                break;
        }

        // �X�e�[�g�A�X�L���̏�����
        skill.Init();
        _state.Init();
    }
}