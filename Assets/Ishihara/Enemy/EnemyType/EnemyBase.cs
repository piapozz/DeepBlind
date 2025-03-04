using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

// �X���[�N
// �h�A�ł�
// ���b�J�[���[�v

// �G�l�~�[�̌��ƂȂ�e�N���X

public  class EnemyBase : MonoBehaviour
{
    private static System.Func<int, GameObject> _GetObject = null;

    public static void SetGetObjectCallback(System.Func<int, GameObject> setCallback)
    {
        _GetObject = setCallback;
    }

    public int ID               { get; private set; } = -1;
    private int _masterID = -1;
    public Transform transform  { get; private set; } = null;
    
    public float speed          { get; private set; } = -1;       // �G�l�~�[�̑���
    public float speedDiameter  { get; private set; } = -1;       // ���������̑����̔{��
    public float threatRange    { get; private set; } = -1;       // ���Д͈�
    public float viewLength     { get; private set; } = -1;       // ���E�̒���
    public float fieldOfView    { get; private set; } = -1;       // ����p

    // ���ꂼ��̃X�e�[�g�N���X
    private ISeach _seach;
    private IVigilance _vigilance;
    private ITracking _tracking;

    // ���ꂼ��̃X�L���N���X
    private ISkill _skill;
    private List<IEnemyState> _state;
    private State _nowState;

    // �i�r���b�V��
    private NavMeshAgent _agent;

    private static System.Action<State> _ChangeState = null;

    public virtual void Setup(int setID, Vector3 position, int masterID)
    {
        ID = setID;
        _masterID = masterID;
        _GetObject(ID).transform.position = position;
        _GetObject(ID).SetActive(true);
        ResetStatus();
        _ChangeState = StateChange;

        //�X�e�[�g�̏�����
        int stateMax = (int)State.MAX;
        _state = new List<IEnemyState>(stateMax);
        _state.Add(_seach);
        _state.Add(_vigilance);
        _state.Add(_tracking);
        for (int i = 0, max = stateMax; i < max; i++)
        {
            _state[i].Init(ID);
        }
        _nowState = State.SEARCH;

        _skill.Init();

        _agent = _GetObject(ID).GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// �X�e�[�^�X������
    /// </summary>
    public virtual void ResetStatus()
    {
        var characterMaster = CharacterMasterUtility.GetCharacterMaster(_masterID);
        if (characterMaster == null) return;

        SetSpeed(characterMaster.Speed);
        SetSpeedDiameter(characterMaster.SpeedDiameter);
        SetThreatRange(characterMaster.ThreatRange);
        SetViewLength(characterMaster.ViewLength);
        SetFieldOfView(characterMaster.FieldOfView);
        SetSeach(characterMaster.Seach);
        SetVigilance(characterMaster.Vigilance);
        SetTracking(characterMaster.Tracking);
        SetSkill(characterMaster.Skill);
    }

    public void Teardown()
    {
        _GetObject(ID).SetActive(false);
        ID = -1;
        _state.Clear();
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

    public void SetSeach(string setSeach)
    {
        Type type = Type.GetType(setSeach);
        _seach = (ISeach)type;
    }

    public void SetVigilance(string setVigilance)
    {
        Type type = Type.GetType(setVigilance);
        _vigilance = (IVigilance)type;
    }

    public void SetTracking(string setTracking)
    {
        Type type = Type.GetType(setTracking);
        _tracking = (ITracking)type;
    }

    public void SetSkill(string setSkill)
    {
        Type type = Type.GetType(setSkill);
        _skill = (ISkill)type;
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
    public void SetNavTarget(Vector3 targetPosition)
    {
        // �ڕW�ʒu��ݒ�
        _agent.SetDestination(targetPosition);
    }

    public void Active()
    {
        _state[(int)_nowState].Activity();

        _skill.Ability();
    }

    /// <summary>
    /// �X�e�[�g�ƃX�L���̐؂�ւ�����
    /// </summary>
    /// <param name="state"></param>
    public void StateChange(State state)
    {
        _nowState = state;
    }
}