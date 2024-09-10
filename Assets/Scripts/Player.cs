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

    [SerializeField] private InputActionReference hold;             // 長押しを受け取る対象のAction

    // プレイヤーのステータス値
    public struct PlayerStatus
    {   
        public float stamina;                                       // スタミナ量
        public float speed;                                         // 動く速さ
        public float fear;                                          // 怖気度
        public float soundRange;                                    // プレイヤーが出してしまう音の範囲
    }

    Vector3 offsetgenPos = new Vector3(0, 1.5f, 0);
    float moveSpeed = 10.0f;
    float rotateSpeed = 1.0f;
    Vector2 rotateVec;

    Vector2 inputMove;                                              // InputSystemで得たWASDの入力値を管理する
    Vector2 inputCursor;                                            // InputSystemで得たマウスのカーソルの入力値を管理する
    float inputDash;                                                // InputSystemを使ってダッシュの管理

    // 移動量
    Vector3 moveVec;

    private void Awake()
    {
        if (hold == null) return;

        // InputActionReferenceのholdにハンドラを登録する
        // hold.action.started += OnDash;
        hold.action.performed += OnDash;

        // 入力を受け取るために有効化
        hold.action.Enable();
    }

    void Start()
    {
        transform.position = generateStage.GetStartPos() + offsetgenPos;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {

        // カメラを考慮した移動(InputSystemとCharacterController使用)
        Move();

        // カメラの回転
        // Rotate();

    }

    // DeepBlindのアクションマップのMoveに登録されているキーが押されたときに入力値を取得
    public void OnMove(InputAction.CallbackContext context)
    {   
        inputMove = context.ReadValue<Vector2>();
    }

    // ActionsのLookに割り当てられている入力があったなら実行（未実装）
    public void OnLook(InputAction.CallbackContext context)
    {
        inputCursor = context.ReadValue<Vector2>();
    }

    // ActionsのDashに割り当てられている入力があったなら実行
    public void OnDash(InputAction.CallbackContext context)
    {
        inputDash = context.ReadValue<float>();
    }

    // 移動
    void Move()
    {
        // ダッシュのキーが押されたとき移動する速さを変更する
        if (inputDash != 0) moveSpeed = 5.0f;
        else moveSpeed = 2.0f;

        // 入力でもらった値を使って移動量を計算
        moveVec = new Vector3(inputMove.x, 0, inputMove.y);

        // 値の正規化
        moveVec.Normalize();

        // カメラの角度分だけ移動量を回転
        moveVec = Quaternion.Euler(0, camera.gameObject.transform.eulerAngles.y, 0) * moveVec;

        // フレームごとの移動量を計算し動かす
        characterController.Move(moveVec * Time.deltaTime * moveSpeed);
    }

    // 回転
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

    // どの区画にいるかを返す関数
    public Vector2Int GetNowSection()
    {
        float width = transform.position.x / generateStage.GetSectionSize() + 0.5f;
        float height = transform.position.z / generateStage.GetSectionSize() + 0.5f;

        return new Vector2Int(Mathf.FloorToInt(width), Mathf.FloorToInt(height));
    }

    // 現在の座標を返す関数
    public Vector3 GetPosition()
    {
        return transform.position;
    }

    // カメラを返す関数
    public Camera GetCamera()
    {
        return Camera.main;
    }
}
