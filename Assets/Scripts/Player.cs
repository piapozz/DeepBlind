using Cinemachine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.PostProcessing;
using static CommonModule;

public class Player : MonoBehaviour
{
    public static Player instance { get; private set; } = null;
    private ColorGrading colorGranding = null;

    [SerializeField] CharacterController characterController;
    [SerializeField] GameObject camera;
    [SerializeField] GameObject playerObject;
    [SerializeField] CinemachineVirtualCamera virtualCamera;                         // 制御対象のカメラ
    [SerializeField] private InputActionReference hold;             // 長押しを受け取る対象のAction
    [SerializeField] CinemachineImpulseSource impulseSource;
    [SerializeField] PostProcessVolume volume;
    [SerializeField] public Transform itemAnker = null;

    [SerializeField] bool isDebug = false;                          // 疲れないようにする
    [SerializeField] float rotationSpeed = 1.0f;

    private float _stepInterval = 0.0f;

    const float STAMINA_MAX = 10.0f;            // スタミナの最大値
    const float WALK_SPEED = 2.5f;              // 歩く速度
    const float DASH_SPEED = 5.0f;              // 走る速度
    const float TIRED_SPEED = WALK_SPEED;       // 疲弊しているときの速度

    private readonly float _STEP_INTERVAL_SEC = 0.5f;   // 音を出す感覚
    private readonly float _WALK_SOUND_VOLUME = 0.05f;  // 歩行時の環境音量
    private readonly float _DASH_SOUND_VOLUME = 0.2f;   // 走り時の環境音量

    // プレイヤーのステータス値
    public struct PlayerStatus
    {
        public float stamina;                                       // スタミナ量
        public float speed;                                         // 動く速さ
        public float fear;                                          // 怖気度
        public float soundRange;                                    // プレイヤーが出してしまう音の範囲
    }

    public PlayerStatus status;

    bool isTired = false;                                           // 疲れているかを管理
    public bool isLocker = false;                                   // ロッカーに入っているかを管理

    Vector3 offsetGenPos = new Vector3(0, 0, 0);                    // 初期位置

    Vector2 inputMove;                                              // InputSystemで得たWASDの入力値を管理する
    Vector2 inputCursor;                                            // InputSystemで得たマウスのカーソルの入力値を管理する
    float inputDash;                                                // InputSystemを使ってダッシュの管理

    Vector3 moveVec;                                                // 移動量

    private Color gamma;

    public Light selfLight { get; private set; } = null;

    private void Awake()
    {
        instance = this;

        if (hold == null) return;

        // InputActionReferenceのholdにハンドラを登録する
        hold.action.performed += OnDash;

        // 入力を受け取るために有効化
        hold.action.Enable();
    }

    void Start()
    {
        status.stamina = STAMINA_MAX;

        volume.profile.TryGetSettings(out colorGranding);
        Cursor.lockState = CursorLockMode.Locked;
        Transform startTransform = StageManager.instance.GetPlayerStartTransform();
        transform.position = startTransform.position + offsetGenPos;
        camera.transform.rotation = startTransform.rotation;
        gamma = colorGranding.gamma.value;
        selfLight = new Light();
        UnityEngine.Light light = GetComponentInChildren<UnityEngine.Light>();
        selfLight.Initialize(light);
    }

    void Update()
    {
        Move();
        selfLight.ConsumeBattery();

        float soundCounter = 0;
        soundCounter += Time.deltaTime;
        if (soundCounter > 2.0f)
        {
            AudioManager.instance.PlaySE(SE.WALK);
            soundCounter = 0;
        }
    }

    // カメラを考慮した移動(InputSystemとCharacterController使用)
    void Move()
    {
        // 入力でもらった値を使って移動量を計算
        moveVec = new Vector3(inputMove.x, 0, inputMove.y);

        // 値の正規化
        moveVec.Normalize();

        // カメラの角度分だけ移動量を回転
        moveVec = Quaternion.Euler(0, camera.gameObject.transform.eulerAngles.y, 0) * moveVec;

        // フレームごとの移動量を計算し動かす
        characterController.Move(moveVec * Time.deltaTime * status.speed);


        // スタミナを見て疲れていたら＆デバッグ状態ではなかったら、疲れてる状態に変える
        if (status.stamina <= 0.0f && isDebug != true) isTired = true;

        // 動いていたら実行
        if (inputMove.x != 0 || inputMove.y != 0)
        {
            // ノイズの値を変更
            NoiseValue(1.0f, 1.0f);

            // スタミナが足りなかったら
            if (isTired == true)
            {
                // 疲れている状態だったら歩く速度を変更
                status.speed = TIRED_SPEED;

                // 徐々にスタミナ回復
                status.stamina += Time.deltaTime * 2.0f;

                // 四分の位置回復したら疲れている状態を解除
                if (status.stamina >= STAMINA_MAX) isTired = false;
                return;
            }
            // スタミナが足りていたらただの歩き
            else
            {
                OccurrenceSound(_WALK_SOUND_VOLUME);

                // 歩く速さを変更
                status.speed = WALK_SPEED;

                // 徐々にスタミナ回復
                if (status.stamina <= STAMINA_MAX) status.stamina += Time.deltaTime * 2.0f;
            }

            // ダッシュのキーが押されたとき移動する速さを変更する
            if (inputDash != 0 && isTired == false)
            {
                OccurrenceSound(_DASH_SOUND_VOLUME);

                // 走っていたら速度変更
                status.speed = DASH_SPEED;

                // スタミナを消費
                status.stamina -= Time.deltaTime * 3.0f;

                // Noiseをダッシュ仕様に変更
                NoiseValue(1.5f, 1.5f);
            }
        }
        // 止まっていたらノイズの変更なし
        else
        {
            NoiseValue(0.5f, 1.0f);
            if (status.stamina <= STAMINA_MAX) status.stamina += Time.deltaTime * 1.0f;
        }
    }

    /// <summary>
    /// 音を出す
    /// </summary>
    /// <param name="volume"></param>
    /// <returns></returns>
    private void OccurrenceSound(float volume)
    {
        if (_stepInterval < _STEP_INTERVAL_SEC)
            _stepInterval += Time.deltaTime;
        else
        {
            _stepInterval = 0;
            SoundObjectManager.SetSound(transform.position, volume);
        }
    }

    // VirtualCameraにあるNoiseProfileの値をスクリプトから変更します
    // amplitudeGainには振幅を入力・frequencyGainには周波数(カメラが振動する速さ)を入力してください
    private void NoiseValue(float amplitudeGain, float frequencyGain)
    {
        // VirtualCameraのノイズの設定を変更
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = amplitudeGain;
        virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = frequencyGain;
    }

    // DeepBlindのアクションマップのMoveに登録されているキーが押されたときに入力値を取得
    public void OnMove(InputAction.CallbackContext context)
    {
        inputMove = context.ReadValue<Vector2>();
    }

    // ActionsのLookに割り当てられている入力があったなら実行
    public void OnLook(InputAction.CallbackContext context)
    {
        inputCursor = context.ReadValue<Vector2>();
    }

    // ActionsのDashに割り当てられている入力があったなら実行
    public void OnDash(InputAction.CallbackContext context)
    {
        inputDash = context.ReadValue<float>();
    }

    public void OnLight(InputAction.CallbackContext context)
    {
        selfLight.SwitchLight();
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        InventoryManager.instance.UseItemEffect();
    }

    public async void EnemyFound()
    {
        // 音を鳴らしてカメラを揺らす
        AudioManager.instance.PlaySE(SE.PLAYER_SURPRISE);
        impulseSource.GenerateImpulse();
        colorGranding.gamma.value = Color.red + gamma;
        await UniTask.Delay(1000);
        colorGranding.gamma.value = gamma;
    }

    public void EnemyCaught(CinemachineVirtualCamera vcam)
    {
        // 音を鳴らしてカメラを移動させる
        AudioManager.instance.PlaySE(SE.CAUGHT);
        vcam.Priority = 100;
        impulseSource.GenerateImpulseAt(vcam.transform.position, Vector3.up);
        UniTask task = WaitAction(2.0f, FadeChangeScene);
    }

    public void FadeChangeScene()
    {
        FadeSceneChange.instance.ChangeSceneEvent("GameResult");
        Destroy(StageManager.instance.gameObject);
        StageManager.instance.Teardown();
    }

    public float GetStamina() { return status.stamina / STAMINA_MAX; }

    // 現在自身が出している音の範囲を返す
    public float GetSoundRange() { return status.soundRange; }
    // 現在の座標を返す関数
    public Vector3 GetPosition() { return transform.position; }
    // トランスフォームを返す関数
    public Transform GetTransform() { return transform; }
    // カメラを返す関数
    public Camera GetCamera() { return Camera.main; }
    // 現在の座標を返す関数
    public Vector3 GetMoveVec() { return Quaternion.Euler(0, -camera.gameObject.transform.eulerAngles.y, 0) * moveVec; }
    public void SetPosition(Vector3 position) { transform.position = position; }
    public void SetCharaController(bool isActive) { characterController.enabled = isActive; }
}
