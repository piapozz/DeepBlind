using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Scripting.APIUpdating;
using System;
using System.Windows.Input;

// �G�l�~�[�̌��ƂȂ�e�N���X

public abstract class EnemyBase : MonoBehaviour
{
    // ���\���� (�Q�Ɨp)
    public struct EnemyInfo
    {
        public int id;                      // �G�l�~�[�̎��ʔԍ�
        public float spped;                 // �G�l�~�[�̑���
        public float threatRange;           // ���Д͈�
        public float viewLength;            // ���E�̒���
        public float fieldOfView;           // ����p
        public Animator animator;           // �A�j���[�^�[
        public Bounds bounds;               // �����̌`
        public EnemyStatus status;          // �X�e�[�^�X
        public PlayerStatus playerStatus;   // �v���C���[�̃X�e�[�^�X
    }

    // �X�e�[�^�X�\���� (����p)
    public struct EnemyStatus
    {
        public Vector3 position;    // ���݈ʒu
        public Vector3 targetPos;   // �ڕW�ʒu
        public Vector3 lostPos;     // ���������ʒu
        public bool isTargetLost;   // �������Ă��邩�ǂ���
        public Vector3 dir;         // �i�s����
        public State state;         // ���݂̃X�e�[�g
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
        SEACH,         // �T��
        VIGILANCE,     // �x��
        TRACKING,      // �ǐ�

        MAX
    }

    protected EnemyInfo myInfo;   // �X�e�[�^�X
    private State oldState;               // ��O�̃X�e�[�g
    protected IEnemyState enemyState;       // �X�e�[�g�N���X
    private NavMeshAgent enemyAgent;       // �i�r���b�V��

    // ���ꂼ��̃X�e�[�g�N���X
    protected ISeach seach;
    protected IVigilance vigilance;
    protected ITracking tracking;

    void Start()
    {
        // ������
        BaseInit();

        // ������
        Init();
    }

    void Update()
    {
        // �X�e�[�g�̐؂�ւ�����
        StateSwitching();

        // �s��
        Active();

        // �i�r���b�V���̈ړ�
        MoveNavAgent();
    }

    // ������
    private void BaseInit()
    {
        // �X�e�[�g������
        oldState = myInfo.status.state = State.SEACH;

        // �i�r���b�V���擾
        enemyAgent = GetComponent<NavMeshAgent>();

        // �A�j���[�^�[�擾
        myInfo.animator = GetComponent<Animator>();

        // ���g�̌`���擾
        myInfo.bounds = GetBounds(this.gameObject, new Bounds());

    }

    // ���g�̌`���擾
    private Bounds GetBounds(GameObject obj, Bounds bounds) 
    {
        // �S�Ă̎q�I�u�W�F�N�g���`�F�b�N����
        foreach (Transform child in transform)
        {
            // ���b�V���t�B���^�[�̑��݊m�F
            MeshFilter filter = child.gameObject.GetComponent<MeshFilter>();

            if (filter != null)
            {
                // �I�u�W�F�N�g�̃��[���h���W�ƃT�C�Y���擾����
                Vector3 ObjWorldPosition = child.position;
                Vector3 ObjWorldScale = child.lossyScale;

                // �t�B���^�[�̃��b�V����񂩂�o�E���h�{�b�N�X���擾����
                Bounds meshBounds = filter.mesh.bounds;

                // �o�E���h�̃��[���h���W�ƃT�C�Y���擾����
                Vector3 meshBoundsWorldCenter = meshBounds.center + ObjWorldPosition;
                Vector3 meshBoundsWorldSize = Vector3.Scale(meshBounds.size, ObjWorldScale);

                // �o�E���h�̍ŏ����W�ƍő���W���擾����
                Vector3 meshBoundsWorldMin = meshBoundsWorldCenter - (meshBoundsWorldSize / 2);
                Vector3 meshBoundsWorldMax = meshBoundsWorldCenter + (meshBoundsWorldSize / 2);

                // �擾�����ŏ����W�ƍő���W���܂ނ悤�Ɋg��/�k�����s��
                if (bounds.size == Vector3.zero)
                {
                    // ���o�E���h�̃T�C�Y���[���̏ꍇ�̓o�E���h����蒼��
                    bounds = new Bounds(meshBoundsWorldCenter, Vector3.zero);
                }
                bounds.Encapsulate(meshBoundsWorldMin);
                bounds.Encapsulate(meshBoundsWorldMax);
            }

            // �ċA����
            bounds = GetBounds(child.gameObject, bounds);
        }
        return bounds;
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
        myInfo = enemyState.Activity(myInfo);
    }

    // �i�r���b�V���ňړ�����
    private void MoveNavAgent()
    {
        // �ڕW�ʒu��ݒ�
        enemyAgent.SetDestination(myInfo.status.targetPos);

        // ���x�̕ύX
        enemyAgent.speed = myInfo.spped;

        // ���݂̍��W���擾
        myInfo.status.position = enemyAgent.nextPosition;
    }

    // ������
    public abstract void Init();

    // �X�e�[�g�̐؂�ւ�����
    public void StateChange(State state)
    {
        // �X�e�[�g��؂�ւ���
        switch (myInfo.status.state)
        {
            case State.SEACH:

                enemyState = (ISeach)seach;

                break;

            case State.VIGILANCE:

                enemyState = (IVigilance)vigilance;

                break;

            case State.TRACKING:

                enemyState = (ITracking)tracking;

                break;
        }

        // �X�e�[�g�̏�����
        enemyState.Init();
    }

    // ���̍X�V
    public void SetEnemyInfo(EnemyInfo info) { myInfo = info; }

    // �v���C���[���̍X�V
    public void SetPlayerStatus(PlayerStatus status) { myInfo.playerStatus = status; }

    // �ڕW�ʒu�̐ݒ�
    public void SetTargetPos(Vector3 pos) { myInfo.status.targetPos = pos; }
}
