using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.EventSystems.EventTrigger;

/// <summary>
/// �G�l�~�[�̊��N���X
/// </summary>
public class EnemyBase : MonoBehaviour
{
    public enum State
    {
        SEARCH,
        VIGILANCE,
        TRACKING,
        MAX
    }

    private static System.Func<int, GameObject> _GetObject = null;

    public static void SetGetObjectCallback(Func<int, GameObject> setCallback)
    {
        _GetObject = setCallback;
    }

    public int ID { get; private set; } = -1;               // ID
    private int _masterID = -1;                             // �}�X�^�[ID
    public Vector3 target { get; private set; }             // �^�[�Q�b�g���W

    public float speed { get; private set; } = -1;          // �ړ����x
    public float speedDiameter { get; private set; } = -1;  // �ړ����x�̔{��
    public float threatRange { get; private set; } = -1;    // ���Д͈�
    public float viewLength { get; private set; } = -1;     // ���E����
    public float fieldOfView { get; private set; } = -1;    // ����p�x
    public bool isAbility { get; private set; } = false;    // �X�L���g�p����

    private ISeach _seach;              // ���G
    private IVigilance _vigilance;      // �x��
    private ITracking _tracking;        // �ǐ�
    private ISkill _skill;              // �X�L��
    private List<IEnemyState> _state;   // �X�e�[�g

    private bool caught = false;        // �v���C���[��߂܂�����

    public State _nowState { get; private set; }    // ���݂̃X�e�[�g

    private NavMeshAgent _agent;        // �i�r���b�V��
    private Animator _animator;         // �A�j���[�^�[
    public CinemachineVirtualCamera _camera { get; private set; } = null;       // �ߊl���̃J����

    /// <summary>
    /// �����ݒ�
    /// </summary>
    /// <param name="setID"></param>
    /// <param name="position"></param>
    /// <param name="masterID"></param>
    public virtual void Setup(int setID, Vector3 position, int masterID)
    {
        // �����ݒ�
        ID = setID;
        _masterID = masterID;
        GameObject obj = _GetObject(ID);
        if (obj == null) return;
        // �ʒu�ݒ�
        obj.transform.position = position;
        obj.SetActive(true);
        ResetStatus();

        // �X�e�[�g�ݒ�
        int stateMax = (int)State.MAX;
        _state = new List<IEnemyState>(stateMax);
        if (_seach != null) _state.Add(_seach);
        if (_vigilance != null) _state.Add(_vigilance);
        if (_tracking != null) _state.Add(_tracking);
        // ������
        foreach (var state in _state)
        {
            state.Init(ID);
        }
        // ���݂̃X�e�[�g
        _nowState = State.SEARCH;
        _skill?.Init(ID);

        // �擾
        _agent = obj.GetComponent<NavMeshAgent>();
        _animator = obj.GetComponent<Animator>();
        _camera = obj.GetComponentInChildren<CinemachineVirtualCamera>();
        if (_agent != null) _agent.speed = speed;
        // �^�[�Q�b�g�ݒ�
        target = position;
    }

    /// <summary>
    /// �X�e�[�^�X���Z�b�g
    /// </summary>
    public virtual void ResetStatus()
    {
        // �}�X�^�[����擾
        var characterMaster = CharacterMasterUtility.GetCharacterMaster(_masterID);
        if (characterMaster == null) return;
        // �X�e�[�^�X�ݒ�
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

    /// <summary>
    /// �I������
    /// </summary>
    public void Teardown()
    {
        _GetObject(ID)?.SetActive(false);
        ID = -1;
        _state?.Clear();
    }

    /// <summary>
    /// �X�V����
    /// </summary>
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

    /// <summary>
    /// �^�[�Q�b�g�ݒ�
    /// </summary>
    /// <param name="targetPosition"></param>
    public void SetNavTarget(Vector3 targetPosition)
    {
        if (_agent != null)
        {
            target = targetPosition;
            _agent.SetDestination(target);
        }
    }

    /// <summary>
    /// ���s����
    /// </summary>
    public void Active()
    {
        _state?[(int)_nowState]?.Activity();
        _skill?.Ability();
    }

    /// <summary>
    /// �X�e�[�g�ύX
    /// </summary>
    /// <param name="state"></param>
    public void StateChange(State state)
    {
        if (state == State.TRACKING) EnemyUtility.GetPlayer().EnemyFound();
        _nowState = state;
    }

    /// <summary>
    /// �A�j���[�V�������x�ݒ�
    /// </summary>
    /// <param name="speed"></param>
    public void SetAnimationSpeed(float speed)
    {
        if (_animator != null)
        {
            _animator.speed = speed;
        }
    }

    /// <summary>
    /// �i�r���b�V�����x�ݒ�
    /// </summary>
    /// <param name="speed"></param>
    public void SetNavSpeed(float speed)
    {
        if (_agent != null)
        {
            if(speed == 0) _agent.velocity = Vector3.zero;
            _agent.speed = speed;
        }
    }

    /// <summary>
    /// �X�L���g�p���ݒ�
    /// </summary>
    public void SetIsAbility()
    {
        isAbility = !isAbility;
    }

    /// <summary>
    /// �v���C���[��߂܂�����
    /// </summary>
    /// <returns></returns>
    public bool CheckCaughtPlayer()
    {
        float length = EnemyUtility.EnemyToPlayerLength(ID);

        caught = (length < 0.7 && !isAbility) || caught;

        return caught;
    }

    /// <summary>
    /// �v���C���[��߂܂���
    /// </summary>
    public void CaughtPlayer()
    {
        SetAnimationSpeed(1);
        SetSpeed(0);
        SetScreamTrigger();
    }

    /// <summary>
    /// �ߊl�A�j���[�V�����Đ�
    /// </summary>
    public void SetScreamTrigger()
    {
        if (_animator != null)
        {
            _animator.SetTrigger("scream");
        }
    }
}
