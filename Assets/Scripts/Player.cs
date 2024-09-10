using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.EventSystems.StandaloneInputModule;

public class Player : MonoBehaviour
{
    [SerializeField] GenerateStage generateStage;
    [SerializeField] CharacterController characterController;
    [SerializeField] GameObject camera;

    [SerializeField] private InputActionReference hold;             // ���������󂯎��Ώۂ�Action

    // �v���C���[�̃X�e�[�^�X�l
    public struct PlayerStatus
    {   
        public float stamina;                                       // �X�^�~�i��
        public float speed;                                         // ��������
        public float fear;                                          // �|�C�x
        public float soundRange;                                    // �v���C���[���o���Ă��܂����͈̔�
    }

    Vector3 offsetgenPos = new Vector3(0, 1.5f, 0);
    float moveSpeed = 10.0f;
    float rotateSpeed = 1.0f;
    Vector2 rotateVec;

    Vector2 inputMove;                                              // InputSystem�œ���WASD�̓��͒l���Ǘ�����
    Vector2 inputCursor;                                            // InputSystem�œ����}�E�X�̃J�[�\���̓��͒l���Ǘ�����
    float inputDash;                                                // InputSystem���g���ă_�b�V���̊Ǘ�

    // �ړ���
    Vector3 moveVec;

    private void Awake()
    {
        if (hold == null) return;

        // InputActionReference��hold�Ƀn���h����o�^����
        // hold.action.started += OnDash;
        hold.action.performed += OnDash;

        // ���͂��󂯎�邽�߂ɗL����
        hold.action.Enable();
    }

    void Start()
    {
        transform.position = generateStage.GetStartPos() + offsetgenPos;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        // �J�������l�������ړ�(InputSystem��CharacterController�g�p)
        Move();

        // �J�����̉�]
        // Rotate();

    }

    // DeepBlind�̃A�N�V�����}�b�v��Move�ɓo�^����Ă���L�[�������ꂽ�Ƃ��ɓ��͒l���擾
    public void OnMove(InputAction.CallbackContext context)
    {   
        inputMove = context.ReadValue<Vector2>();
    }

    // Actions��Look�Ɋ��蓖�Ă��Ă�����͂��������Ȃ���s�i�������j
    public void OnLook(InputAction.CallbackContext context)
    {
        inputCursor = context.ReadValue<Vector2>();
    }

    // Actions��Dash�Ɋ��蓖�Ă��Ă�����͂��������Ȃ���s
    public void OnDash(InputAction.CallbackContext context)
    {
        inputDash = context.ReadValue<float>();
    }

    // �ړ�
    void Move()
    {
        // �_�b�V���̃L�[�������ꂽ�Ƃ��ړ����鑬����ύX����
        if (inputDash != 0) moveSpeed = 5.0f;
        else moveSpeed = 2.0f;

        // ���͂ł�������l���g���Ĉړ��ʂ��v�Z
        moveVec = new Vector3(inputMove.x, 0, inputMove.y);

        // �l�̐��K��
        moveVec.Normalize();

        // �J�����̊p�x�������ړ��ʂ���]
        moveVec = Quaternion.Euler(0, camera.gameObject.transform.eulerAngles.y, 0) * moveVec;

        // �t���[�����Ƃ̈ړ��ʂ��v�Z��������
        characterController.Move(moveVec * Time.deltaTime * moveSpeed);
    }

    // ��]
    void Rotate()
    {
        Quaternion temp = transform.rotation;

        temp = Quaternion.Euler(0, inputCursor.x * rotateSpeed, 0) * temp * Quaternion.Euler(-inputCursor.y * rotateSpeed, 0, 0);

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
}
