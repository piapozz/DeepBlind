using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using static CommonModule;
using static UnityEditor.Rendering.CameraUI;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; } = null;
    private ColorGrading colorGranding = null;

    [SerializeField] CharacterController characterController;
    [SerializeField] GameObject camera;
    [SerializeField] GameObject playerObject;
    [SerializeField] CinemachineVirtualCamera virtualCamera;                         // ����Ώۂ̃J����
    [SerializeField] private InputActionReference hold;             // ���������󂯎��Ώۂ�Action
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] PostProcessVolume volume;

    [SerializeField] bool isDebug = false;                          // ���Ȃ��悤�ɂ���
    [SerializeField] float rotationSpeed = 1.0f;

    const float STAMINA_MAX = 10.0f;               // �X�^�~�i�̍ő�l
    const float WALK_SPEED = 2.5f;                                  // �������x
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

    public PlayerStatus status;

    bool isTired = false;                                           // ���Ă��邩���Ǘ�
    public bool isLocker = false;                                   // ���b�J�[�ɓ����Ă��邩���Ǘ�

    Vector3 offsetGenPos = new Vector3(0, 0, 0);                    // �����ʒu        

    Vector2 inputMove;                                              // InputSystem�œ���WASD�̓��͒l���Ǘ�����
    Vector2 inputCursor;                                            // InputSystem�œ����}�E�X�̃J�[�\���̓��͒l���Ǘ�����
    float inputDash;                                                // InputSystem���g���ă_�b�V���̊Ǘ�

    Vector3 moveVec;                                                // �ړ���

    private Color gamma;

    private void Awake()
    {
        instance = this;

        // Virtual Camera �擾
        //virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();

        if (hold == null) return;

        // InputActionReference��hold�Ƀn���h����o�^����
        hold.action.performed += OnDash;

        // ���͂��󂯎�邽�߂ɗL����
        hold.action.Enable();
    }

    void Start()
    {
        status.stamina = STAMINA_MAX;

        volume.profile.TryGetSettings(out colorGranding);
        transform.position = GenerateStage.instance.GetStartPos() + offsetGenPos;
        Cursor.lockState = CursorLockMode.Locked;
        gamma = colorGranding.gamma.value;
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
        if (status.stamina <= 0.0f && isDebug != true) isTired = true;

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
                status.stamina += Time.deltaTime * 2.0f;

                // �l���̈ʒu�񕜂�������Ă����Ԃ�����
                if (status.stamina >= STAMINA_MAX) isTired = false;
                return;
            }

            // �X�^�~�i������Ă����炽���̕���
            else
            {
                // ����������ύX
                status.speed = WALK_SPEED;

                // ���X�ɃX�^�~�i��
                if (status.stamina <= STAMINA_MAX) status.stamina += Time.deltaTime * 2.0f;
            }

            // �_�b�V���̃L�[�������ꂽ�Ƃ��ړ����鑬����ύX����
            if (inputDash != 0 && isTired == false)
            {
                // �����Ă����瑬�x�ύX
                status.speed = DASH_SPEED;

                // �X�^�~�i������
                status.stamina -= Time.deltaTime * 3.0f;

                // Noise���_�b�V���d�l�ɕύX
                NoiseValue(1.5f, 1.5f);
            }
        }

        // �~�܂��Ă�����m�C�Y�̕ύX�Ȃ�
        else
        {
            NoiseValue(0.5f, 1.0f);
            if (status.stamina <= STAMINA_MAX) status.stamina += Time.deltaTime * 1.0f;
        }
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

    public async void EnemyFound()
    {
        // ����炵�ăJ������h�炷
        AudioManager.instance.PlaySE(SE.PLAYER_SURPRISE);
        impulseSource.GenerateImpulse(); 
        colorGranding.gamma.value = Color.red + gamma;
        await UniTask.Delay(1000);
        colorGranding.gamma.value = gamma; 
    }

    public void EnemyCaught(CinemachineVirtualCamera vcam)
    {
        // ����炵�ăJ�������ړ�������
        AudioManager.instance.PlaySE(SE.CAUGHT);
        vcam.Priority = 100;
        impulseSource.GenerateImpulseAt(vcam.transform.position,Vector3.up);
        UniTask task = WaitAction(2.0f, FadeChangeScene);
    }

    public void FadeChangeScene()
    {
        FadeSceneChange.instance.ChangeSceneEvent("GameResult");
    }



    //public async UniTask Footsteps(int speed)
    //{
    //    while (true)
    //    {
    //        AudioManager.instance.PlaySE(SE.WALK);
    //        await UniTask.DelayFrame(500);
    //    }
    //}

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
        float width = transform.position.x / GenerateStage.instance.GetSectionSize() + 0.5f;
        float height = transform.position.z / GenerateStage.instance.GetSectionSize() + 0.5f;

        return new Vector2Int(Mathf.FloorToInt(width), Mathf.FloorToInt(height));
    }

    public float GetStamina() { return status.stamina / STAMINA_MAX; }

    // ���ݎ��g���o���Ă��鉹�͈̔͂�Ԃ�
    public float GetSoundRange() { return status.soundRange; }

    // ���݂̍��W��Ԃ��֐�
    public Vector3 GetPosition() { return transform.position; }
    // �g�����X�t�H�[����Ԃ��֐�
    public Transform GetTransform() { return transform; }
    // �J������Ԃ��֐�
    public Camera GetCamera() { return Camera.main; }

    // ���݂̍��W��Ԃ��֐�
    public Vector3 GetMoveVec() { return Quaternion.Euler(0, -camera.gameObject.transform.eulerAngles.y, 0) * moveVec; }

    public void SetPosition(Vector3 position) { transform.position = position; }
    public void SetRotate(Quaternion rotation) { camera.transform.rotation = rotation; }
    public void SetCharaController(bool isActive) { characterController.enabled = isActive; }
}
