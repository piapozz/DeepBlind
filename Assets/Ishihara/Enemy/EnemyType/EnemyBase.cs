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
        public Vector3 lostPos;             // ���������ʒu
        public bool isTargetLost;           // �������Ă��邩�ǂ���
        public Vector3 dir;                 // �i�s����
        public State state;                 // ���݂̃X�e�[�g
        public bool isAblity;               // �A�r���e�B��
        public Vector3 lostMoveVec;         // �����������̃v���C���[�̈ړ���
        public bool prediction;             // ����
        public List<ViaSeachData> viaData;  // �o�R�T���p�f�[�^
    }

    // �����̌o�R�T���p�̏��\����
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
        public Vector3 moveValue;   // �v���C���[�̈ړ���
    }

    // �X�e�[�g
    public enum State
    {
        SEARCH,        // �T��
        VIGILANCE,     // �x��
        TRACKING,      // �ǐ�

        MAX
    }

    // �A�j���[�V����
    public enum BoolAnimation
    {
        WALKING,        // ���s
        RUNNING,        // ���s
        SKILL,          // �X�L���g�p��

        MAX
    }

    // �A�j���[�V����
    public enum TriggerAnimation
    {
        SCREAM,         // ����
        LOOKING,        // ���n��

        MAX
    }

    [SerializeField] GameObject meshObject;     // ���b�V��

    protected EnemyInfo myInfo;             // �X�e�[�^�X
    private State oldState;                 // ��O�̃X�e�[�g
    protected IEnemyState enemyState;       // �X�e�[�g�N���X
    protected ISkill skill;                 // �X�L��
    private NavMeshAgent enemyAgent;        // �i�r���b�V��

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

    // ������
    private void BaseInit()
    {
        // �X�e�[�g������
        oldState = myInfo.status.state = State.SEARCH;

        // �i�r���b�V���擾
        enemyAgent = GetComponent<NavMeshAgent>();

        // �Q�ƃf�[�^�̏�����
        myInfo.status.position = this.transform.position;
        myInfo.status.nowSpeed = myInfo.pram.speed;
        myInfo.status.prediction = false;
        myInfo.status.viaData = new List<ViaSeachData>();
    }

    // �X�e�[�g�̐؂�ւ�����
    private void StateSwitching()
    {
        // �؂�ւ���Ă�����
        if (oldState == myInfo.status.state) return;

        // �X�V
        oldState = myInfo.status.state;

        // �؂�ւ�����
        StateChange(myInfo.status.state);
    }

    // �s��
    public void Active()
    {
        // �s��(����n���čX�V)
        myInfo = enemyState.Activity(myInfo, skill);
    }

    // �i�r���b�V���ňړ�����
    private void MoveNavAgent()
    {
        // �X�L���g�p���Ȃ�ړ����~
        if (myInfo.status.isAblity)
        {
            enemyAgent.velocity = Vector3.zero;
            return;
        }
        
        // �G�l�~�[���ڕW�ɑ΂��Đڋ߂���Ə�����������悤�ɂ���
        if (Vector3.Distance(enemyAgent.steeringTarget, myInfo.status.position) < 1.0f)
        {
            enemyAgent.speed = myInfo.status.nowSpeed / 2;
        }
        else
        {
            enemyAgent.speed = myInfo.status.nowSpeed;
        }

        enemyAgent.velocity = (enemyAgent.steeringTarget - myInfo.status.position).normalized * enemyAgent.speed;

        // �ڕW�ʒu��ݒ�
        enemyAgent.SetDestination(myInfo.status.targetPos);

        // ���x�̕ύX
        enemyAgent.acceleration = enemyAgent.speed * 8;

        // �����Ă���������擾
        myInfo.status.dir = Vector3.Normalize(enemyAgent.nextPosition - myInfo.status.position);

        // ���݂̍��W���擾
        myInfo.status.position = this.transform.position;
    }

    // ������
    public abstract void Init();

    // �X�e�[�g�ƃX�L���̐؂�ւ�����
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

        // �X�e�[�g�̏�����
        enemyState.Init();
    }

    // �R���W�������v���C���[�ɐڐG���Ă�����ڐG�t���O��|��
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            caught = true;
        }
    }

    // �v���C���[���������ꏊ�𐄑����邩�ǂ���
    public bool CheckPrediction() { return myInfo.status.prediction; }

    // �v���C���[���̍X�V
    public void SetPlayerStatus(PlayerStatus status) { myInfo.playerStatus = status; }

    // �ڕW�ʒu�̐ݒ�
    public void SetTargetPos(Vector3 pos) { myInfo.status.targetPos = pos; }

    // ���݂̃X�e�[�g
    public State GetNowState() { return myInfo.status.state; }

    // �ڕW�ʒu�ɂ��ǂ蒅�������ǂ���
    public bool CheckReachingPosition() { return (Vector3.Distance(myInfo.status.targetPos, myInfo.status.position) < 2.0f) && (!myInfo.status.isAblity); }

    // ���������n�_�̎擾
    public Vector3 GetLostPos() { return myInfo.status.lostPos; }

    // �����������̈ړ��ʂ��擾
    public Vector3 GetLostMoveVec() { return myInfo.status.lostMoveVec; }

    // �o�R�T���p�̏���ݒ�
    public void SetViaSeachData(List<ViaSeachData> vias) { myInfo.status.viaData = vias; }

    // �v���C���[��߂܂������ǂ���
    public bool CheckCaught() { return caught; }
}