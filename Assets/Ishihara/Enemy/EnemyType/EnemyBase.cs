using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// �G�l�~�[�̌��ƂȂ�e�N���X
public class EnemyBase : MonoBehaviour
{
    private static System.Func<int, GameObject> _GetObject = null;

    public static void SetGetObjectCallback(Func<int, GameObject> setCallback)
    {
        _GetObject = setCallback;
    }

    public int ID { get; private set; } = -1;
    private int _masterID = -1;
    public Vector3 target { get; private set; }

    public float speed { get; private set; } = -1;       // �G�l�~�[�̑���
    public float speedDiameter { get; private set; } = -1;       // ���������̑����̔{��
    public float threatRange { get; private set; } = -1;       // ���Д͈�
    public float viewLength { get; private set; } = -1;       // ���E�̒���
    public float fieldOfView { get; private set; } = -1;       // ����p

    // ���ꂼ��̃X�e�[�g�N���X
    private ISeach _seach;
    private IVigilance _vigilance;
    private ITracking _tracking;

    // ���ꂼ��̃X�L���N���X
    private ISkill _skill;
    private List<IEnemyState> _state;
    [SerializeField]
    public State _nowState { get; private set; }

    // �i�r���b�V��
    private NavMeshAgent _agent;

    // �A�j���[�^�[
    private Animator _animator;

    // 3D�I�[�f�B�I�\�[�X
    private AudioSource _audioSource;

    public CinemachineVirtualCamera _camera { get; private set; } = null;

    public virtual void Setup(int setID, Vector3 position, int masterID)
    {
        ID = setID;
        _masterID = masterID;
        GameObject obj = _GetObject(ID);
        if (obj == null) return;

        obj.transform.position = position;
        obj.SetActive(true);
        ResetStatus();

        // �X�e�[�g�̏�����
        int stateMax = (int)State.MAX;
        _state = new List<IEnemyState>(stateMax);

        if (_seach != null) _state.Add(_seach);
        if (_vigilance != null) _state.Add(_vigilance);
        if (_tracking != null) _state.Add(_tracking);

        foreach (var state in _state)
        {
            state.Init(ID);
        }

        _nowState = State.SEARCH;
        _skill?.Init(ID);

        _agent = obj.GetComponent<NavMeshAgent>();
        _animator = obj.GetComponent<Animator>();
        _audioSource = obj.GetComponent<AudioSource>(); 
        _camera = obj.GetComponentInChildren<CinemachineVirtualCamera>();
        _agent.speed = speed;
        target = position;
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
        _GetObject(ID)?.SetActive(false);
        ID = -1;
        _state?.Clear();
    }

    public void SetSpeed(float setSpeed) => speed = setSpeed;
    public void SetSpeedDiameter(float setSpeedDiameter) => speedDiameter = setSpeedDiameter;
    public void SetThreatRange(float setThreatRange) => threatRange = setThreatRange;
    public void SetViewLength(float setViewLength) => viewLength = setViewLength;
    public void SetFieldOfView(float setFieldOfView) => fieldOfView = setFieldOfView;

    public void SetSeach(string setSeach)
    {
        Type type = Type.GetType(setSeach);
        if (type != null)
        {
            _seach = Activator.CreateInstance(type) as ISeach;
        }
    }

    public void SetVigilance(string setVigilance)
    {
        Type type = Type.GetType(setVigilance);
        if (type != null)
        {
            _vigilance = Activator.CreateInstance(type) as IVigilance;
        }
    }

    public void SetTracking(string setTracking)
    {
        Type type = Type.GetType(setTracking);
        if (type != null)
        {
            _tracking = Activator.CreateInstance(type) as ITracking;
        }
    }

    public void SetSkill(string setSkill)
    {
        Type type = Type.GetType(setSkill);
        if (type != null)
        {
            _skill = Activator.CreateInstance(type) as ISkill;
        }
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
        if (_agent != null)
        {
            target = targetPosition;
            _agent.SetDestination(target);
        }
    }

    public void Active()
    {
        _state?[(int)_nowState]?.Activity();
        _skill?.Ability();
    }

    /// <summary>
    /// �X�e�[�g�ƃX�L���̐؂�ւ�����
    /// </summary>
    public void StateChange(State state)
    {
        if (state == State.TRACKING) EnemyUtility.GetPlayer().EnemyFound();
        _nowState = state;
    }

    /// <summary>
    /// �A�j���[�V�����̑��x�ύX
    /// </summary>
    public void SetAnimationSpeed(float speed)
    {
        if (_animator != null)
        {
            _animator.speed = speed;
        }
    }

    /// <summary>
    /// �i�r���b�V���̑��x�ύX
    /// </summary>
    public void SetNavSpeed(float speed)
    {
        if (_agent != null)
        {
            _agent.speed = speed;
        }
    }

    // �v���C���[�ƐڐG�������ǂ���
    public bool ChackCaughtPlayer()
    {
        return Vector3.Distance(transform.position, EnemyUtility.GetPlayer().transform.position) < 1.5f;
    }
}
