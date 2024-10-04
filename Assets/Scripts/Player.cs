using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class Player : MonoBehaviour
{
    [SerializeField] GenerateStage generateStage;
    [SerializeField] CharacterController characterController;
    [SerializeField] GameObject camera;
    CinemachineVirtualCamera virtualCamera;                         // ����Ώۂ̃J����
    [SerializeField] private InputActionReference hold;             // ���������󂯎��Ώۂ�Action

    [SerializeField] bool isDebug = false;                          // ���Ȃ��悤�ɂ���

    const float STAMINA_MAX = 50.0f;                                // �X�^�~�i�̍ő�l
    const float WALK_SPEED = 1.0f;                                  // �������x
    const float DASH_SPEED = 5.0f;                                  // ���鑬�x
    const float TIRED_SPEED = WALK_SPEED;                           // �敾���Ă���Ƃ��̑��x

    // �v���C���[�̃X�e�[�^�X�l
    public struct PlayerStatus
    {
        public float stamina;                                       // �X�^�~�i��
        public float speed;                                         // ��������
        public float fear;                                          // �|�C�x
        public float soundRange;                                    // �v���C���[���o���Ă��܂����͈̔�
    }

    PlayerStatus status;

    bool isTired = false;                                           // ���Ă��邩���Ǘ�

    Vector3 offsetGenPos = new Vector3(0, 0, 0);                    // �����ʒu        

    Vector2 inputMove;                                              // InputSystem�œ���WASD�̓��͒l���Ǘ�����
    Vector2 inputCursor;                                            // InputSystem�œ����}�E�X�̃J�[�\���̓��͒l���Ǘ�����
    float inputDash;                                                // InputSystem���g���ă_�b�V���̊Ǘ�

    Vector3 moveVec;                                                // �ړ���

    private void Awake()
    {
        // Virtual Camera �擾
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (hold == null) return;

        // InputActionReference��hold�Ƀn���h����o�^����
        hold.action.performed += OnDash;

        // ���͂��󂯎�邽�߂ɗL����
        hold.action.Enable();
    }

    void Start()
    {
        status.stamina = STAMINA_MAX;

        transform.position = generateStage.GetStartPos() + offsetGenPos;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Move();
    }

    // �J�������l�������ړ�(InputSystem��CharacterController�g�p)
    void Move()
    {
        // ���͂ł�������l���g���Ĉړ��ʂ��v�Z
        moveVec = new Vector3(inputMove.x, 0, inputMove.y);

        // �l�̐��K��
        moveVec.Normalize();

        // �J�����̊p�x�������ړ��ʂ���]
        moveVec = Quaternion.Euler(0, camera.gameObject.transform.eulerAngles.y, 0) * moveVec;

        // �t���[�����Ƃ̈ړ��ʂ��v�Z��������
        characterController.Move(moveVec * Time.deltaTime * status.speed);

        // �X�^�~�i�����Ĕ��Ă����灕�f�o�b�O��Ԃł͂Ȃ�������A���Ă��Ԃɕς���
        if (status.stamina <= STAMINA_MAX * 0.1f && isDebug != true) isTired = true;

        // �����Ă�������s
        if (inputMove.x != 0 || inputMove.y != 0)
        {
            // �m�C�Y�̒l��ύX
            NoiseValue(1.0f, 1.0f);

            // �X�^�~�i������Ȃ�������
            if (isTired == true)
            {
                // ���Ă����Ԃ�������������x��ύX
                status.speed = TIRED_SPEED;

                // ���X�ɃX�^�~�i��
                status.stamina += Time.deltaTime * 1.0f;

                // �l���̈ʒu�񕜂�������Ă����Ԃ�����
                if (status.stamina >= STAMINA_MAX * 0.25f) isTired = false;
            }

            // �X�^�~�i������Ă����炽���̕���
            else
            {
                // ����������ύX
                status.speed = WALK_SPEED;

                // ���X�ɃX�^�~�i��
                status.stamina += Time.deltaTime * 1.0f;
            }

            // �_�b�V���̃L�[�������ꂽ�Ƃ��ړ����鑬����ύX����
            if (inputDash != 0 && isTired == false)
            {
                // �����Ă����瑬�x�ύX
                status.speed = DASH_SPEED;

                // �X�^�~�i������
                status.stamina -= Time.deltaTime * 2.0f;

                // Noise���_�b�V���d�l�ɕύX
                NoiseValue(1.5f, 1.5f);
            }
        }

        // �~�܂��Ă�����m�C�Y�̕ύX�Ȃ�
        else { NoiseValue(0.5f, 1.0f); }
    }

    // VirtualCamera�ɂ���NoiseProfile�̒l���X�N���v�g����ύX���܂�
    // amplitudeGain�ɂ͐U������́EfrequencyGain�ɂ͎��g��(�J�������U�����鑬��)����͂��Ă�������
    private void NoiseValue(float amplitudeGain, float frequencyGain)
    {
        // VirtualCamera�̃m�C�Y�̐ݒ��ύX
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitudeGain;
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequencyGain;
    }

    // DeepBlind�̃A�N�V�����}�b�v��Move�ɓo�^����Ă���L�[�������ꂽ�Ƃ��ɓ��͒l���擾
    public void OnMove(InputAction.CallbackContext context)
    {
        inputMove = context.ReadValue<Vector2>();
    }

    // Actions��Look�Ɋ��蓖�Ă��Ă�����͂��������Ȃ���s
    public void OnLook(InputAction.CallbackContext context)
    {
        inputCursor = context.ReadValue<Vector2>();
    }

    // Actions��Dash�Ɋ��蓖�Ă��Ă�����͂��������Ȃ���s
    public void OnDash(InputAction.CallbackContext context)
    {
        inputDash = context.ReadValue<float>();
    }

    // ��]
    void Rotate()
    {
        Quaternion temp = transform.rotation;

        // temp = Quaternion.Euler(0, inputCursor.x * rotateSpeed, 0) * temp * Quaternion.Euler(-inputCursor.y * rotateSpeed, 0, 0);

        Vector3 angle = temp.eulerAngles;

        float angleK = Mathf.Repeat(angle.x + 180, 360) - 180;

        Debug.Log(inputCursor.x);
        Debug.Log(angleK);

        if (!(angleK > -75.0f && angleK < 75.0f)) temp = transform.rotation;

    }

    // �ǂ̋��ɂ��邩��Ԃ��֐�
    public Vector2Int GetNowSection()
    {
        float width = transform.position.x / generateStage.GetSectionSize() + 0.5f;
        float height = transform.position.z / generateStage.GetSectionSize() + 0.5f;

        return new Vector2Int(Mathf.FloorToInt(width), Mathf.FloorToInt(height));
    }

    // ���ݎ��g���o���Ă��鉹�͈̔͂�Ԃ�
    public float GetSoundRange()
    {
        return status.soundRange;
    }

    // ���݂̍��W��Ԃ��֐�
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    // �J������Ԃ��֐�
    public Camera GetCamera()
    {
        return Camera.main;
    }

    // ���݂̍��W��Ԃ��֐�
    public Vector3 GetMoveVec()
    {
        return Quaternion.Euler(0, -camera.gameObject.transform.eulerAngles.y, 0) * moveVec; ;
    }
}
