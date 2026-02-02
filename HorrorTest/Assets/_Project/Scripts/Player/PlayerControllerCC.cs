using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerCC : NetworkBehaviour
{
    [Header("移動設定")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float gravity = -9.81f;

    [Header("視点設定（Look）")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float mouseSensitivity = 10f;
    [SerializeField] private float lookXLimit = 80f;

    private CharacterController _controller;
    private InputActions _inputActions;
    private float _xRotation = 0f;
    private Vector3 _velocity; 

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _inputActions = new InputActions();
    }

    public override void OnNetworkSpawn()
    {
        // 自分がオーナーでない場合はカメラを消して終了
        if (!IsOwner)
        {
            if (playerCamera != null) playerCamera.gameObject.SetActive(false);
            return;
        }

        // オーナー（自分）だけの初期設定
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _inputActions.Enable();
    }

    private void OnEnable() { if (IsOwner) _inputActions?.Enable(); }
    private void OnDisable() { _inputActions?.Disable(); }

    private void Update()
    {
        // 1. 重要：オーナー以外は計算しない
        if (!IsOwner) return;
        if (Cursor.lockState != CursorLockMode.Locked) return;

        // 2. 入力を取得
        Vector2 moveInput = _inputActions.Player.Move.ReadValue<Vector2>();
        Vector2 lookInput = _inputActions.Player.Look.ReadValue<Vector2>();
        bool isSprinting = _inputActions.Player.Sprint.IsPressed() && moveInput.y > 0;

        // 3. 視点操作（ローカルで実行 → ClientNetworkTransformが同期）
        HandleLook(lookInput);

        // 4. 移動処理（ローカルで実行 → ClientNetworkTransformが同期）
        HandleMove(moveInput, isSprinting);
    }

    private void HandleLook(Vector2 lookInput)
    {
        // 左右回転（本体を回す）
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity * Time.deltaTime);

        // 上下回転（カメラだけを回す）
        _xRotation -= lookInput.y * mouseSensitivity * Time.deltaTime;
        _xRotation = Mathf.Clamp(_xRotation, -lookXLimit, lookXLimit);

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        }
    }

    private void HandleMove(Vector2 moveInput, bool isSprinting)
    {
        // 地面判定と重力リセット
        if (_controller.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        // 移動方向の計算
        float speed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        // キャラクターを動かす（ServerRpcを通さず直接！）
        _controller.Move(move * speed * Time.deltaTime);

        // 重力の計算と適用
        _velocity.y += gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
}