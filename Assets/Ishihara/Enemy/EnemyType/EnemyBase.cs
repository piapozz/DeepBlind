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
    // ���\���� (�Q�Ɨp)
    public struct EnemyInfo
    {
        public EnemyPram pram;              // ��{�f�[�^
        public EnemyStatus status;          // �X�e�[�^�X
        public PlayerStatus playerStatus;   // �v���C���[�̃X�e�[�^�X
    }

    // �X�e�[�^�X�\���� (����p)
    public struct EnemyStatus
    {
        public float nowSpeed;              // �G�l�~�[�̌��݂̑���
        public Vector3 position;            // ���݈ʒu
        public Vector3 targetPos;           // �ڕW�ʒu
        public Vector3 dir;                 // �i�s����
        public State state;                 // ���݂̃X�e�[�g
        public bool isAblity;               // �A�r���e�B��
    }

    /// �����̌o�R�T���p�̏��\����
    public struct ViaSeachData
    {
        public Vector3 viaPosition;        // �o�R�n�_�̍��W
        public bool room;                  // �o�R�n�_���������ǂ���
    }

    // �v���C���[������炤���
    public struct PlayerStatus
    {
        public Camera cam;          // �J����
        public Vector3 playerPos;   // �v���C���[�̈ʒu
    }

    // �X�e�[�g
    public enum State
    {
        SEARCH,        // �T��
        VIGILANCE,     // �x��
        TRACKING,      // �ǐ�

        MAX
    }

    protected EnemyInfo myInfo;             // �X�e�[�^�X
    protected IEnemyState enemyState;       // �X�e�[�g�N���X
    protected ISkill skill;                 // �X�L��
    private State _oldState;                 // ��O�̃X�e�[�g
    private NavMeshAgent _enemyAgent;        // �i�r���b�V��
    private Animator _animator;             // �A�j���[�^�[

    // ���ꂼ��̃X�e�[�g�N���X
    protected ISeach seach;
    protected IVigilance vigilance;
    protected ITracking tracking;

    // ���ꂼ��̃X�L���N���X
    protected ISkill seachSkill;
    protected ISkill vigilanceSkill;
    protected ISkill trackingSkill;

    bool caught = false;

    void Start()
    {
        // ������
        Init();

        // ������
        BaseInit();
    }

    void Update()
    {
        // �i�r���b�V���̈ړ�
        MoveNavAgent();

        // �s��
        Active();

        // �X�e�[�g�̐؂�ւ�����
        StateSwitching();

        // �A�j���[�V�����̍X�V

    }

    /// <summary>
    /// ������
    /// </summary>
    private void BaseInit()
    {
        // �X�e�[�g������
        _oldState = myInfo.status.state = State.SEARCH;

        // �i�r���b�V���擾
        _enemyAgent = GetComponent<NavMeshAgent>();

        // �A�j���[�^�[�̎擾
        _animator = GetComponent<Animator>();

        // �Q�ƃf�[�^�̏�����
        myInfo.status.position = this.transform.position;
        myInfo.status.nowSpeed = myInfo.pram.speed;
    }

    /// <summary>
    /// �X�e�[�g�̐؂�ւ�����
    /// </summary>
    private void StateSwitching()
    {
        // �؂�ւ���Ă�����
        if (_oldState == myInfo.status.state) return;

        // �X�V
        _oldState = myInfo.status.state;

        // �؂�ւ�����
        StateChange(myInfo.status.state);
    }

    /// <summary>
    /// �s��
    /// </summary>
    public void Active()
    {
        // �s��(����n���čX�V)
        myInfo = enemyState.Activity(myInfo, skill);
    }

    /// <summary>
    /// �i�r���b�V���ňړ�����
    /// </summary>
    private void MoveNavAgent()
    {
        // �X�L���g�p���Ȃ�ړ����~
        if (myInfo.status.isAblity)
        {
            _enemyAgent.velocity = Vector3.zero;
            return;
        }
        
        // �G�l�~�[���ڕW�ɑ΂��Đڋ߂���Ə�����������悤�ɂ���
        if (Vector3.Distance(_enemyAgent.steeringTarget, myInfo.status.position) < 1.0f)
        {
            _enemyAgent.speed = myInfo.status.nowSpeed / 2;
        }
        else
        {
            _enemyAgent.speed = myInfo.status.nowSpeed;
        }

        _enemyAgent.velocity = (_enemyAgent.steeringTarget - myInfo.status.position).normalized * _enemyAgent.speed;

        // �ڕW�ʒu��ݒ�
        _enemyAgent.SetDestination(myInfo.status.targetPos);

        // ���x�̕ύX
        _enemyAgent.acceleration = _enemyAgent.speed * 8;

        // �����Ă���������擾
        myInfo.status.dir = Vector3.Normalize(_enemyAgent.nextPosition - myInfo.status.position);

        // ���݂̍��W���擾
        myInfo.status.position = this.transform.position;
    }

    /// <summary>
    /// ������
    /// </summary>
    public abstract void Init();

    /// <summary>
    /// �X�e�[�g�ƃX�L���̐؂�ւ�����
    /// </summary>
    /// <param name="state"></param>
    public void StateChange(State state)
    {
        // �X�e�[�g�A�X�L����؂�ւ���
        switch (myInfo.status.state)
        {
            case State.SEARCH:

                enemyState = seach;
                skill = seachSkill;

                myInfo.status.nowSpeed = myInfo.pram.speed;

                break;

            case State.VIGILANCE:

                enemyState = vigilance;
                skill = vigilanceSkill;

                myInfo.status.nowSpeed = myInfo.pram.speed * myInfo.pram.speedDiameter / 2;

                break;

            case State.TRACKING:

                enemyState = tracking;
                skill = trackingSkill;

                myInfo.status.nowSpeed = myInfo.pram.speed * myInfo.pram.speedDiameter;

                break;
        }

        // �X�e�[�g�A�X�L���̏�����
        skill.Init(_animator);
        enemyState.Init();
    }

    /// <summary>
    /// �R���W�������v���C���[�ɐڐG���Ă�����ڐG�t���O��|��
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            caught = true;
        }
    }

    /// <summary>
    /// �v���C���[���̍X�V
    /// </summary>
    /// <param name="status"></param>
    public void SetPlayerStatus(PlayerStatus status) { myInfo.playerStatus = status; }

    /// <summary>
    /// �v���C���[��߂܂������ǂ���
    /// </summary>
    /// <returns></returns>
    public bool CheckCaught() { return caught; }
}